using S7.Net;
using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static S7Communication.S7Enums;
using DateTime = System.DateTime;

namespace S7Communication
{
    public class S7CommunicationHelp
    {
        //限制长度
        private const int LimitReadDataItemsCount = 18;

        public static S7CommunicationHelp Instance;

        private S7CommunicationHelp()
        {
            Instance = new S7CommunicationHelp();
        }

        /// <summary>
        /// 定义一个plc
        /// </summary>
        private Plc _plc;

        /// <summary>
        ///连接
        /// </summary>
        public bool IsConnected = false;

        /// <summary>
        /// 连接PLC
        /// </summary>
        /// <param name="type">PLC CPU类型</param>
        /// <param name="IP">PLC服务器IP</param>
        /// <param name="Rack">机台号</param>
        /// <param name="Slot">插槽号</param>
        /// <returns></returns>
        public BaseResult PLCConnect(PLC_CPUType type, string IP, short Rack, short Slot)
        {
            var cpuType = (CpuType)Enum.Parse(typeof(CpuType), type.ToString());
            //实例化
            _plc = new Plc(cpuType, IP, Rack, Slot);
            IsConnected = false;
            try
            {
                _plc.Open();
                IsConnected = _plc.IsConnected;
                LogHelp.AddLog<InfoLogEntity>(IsConnected ?
                                  $"TCP服务器 IP:{IP} 机台号{Rack} 插槽号{Slot} 连接成功！ " :
                                  $"TCP服务器 IP:{IP} 机台号{Rack} 插槽号{Slot} 连接失败！ ");
            }
            catch (Exception ex)
            {
                LogHelp.AddLog<InfoLogEntity>($"PLC连接失败：{ex.Message} ");
            }

            return IsConnected;
        }

        /// <summary>
        /// 断开PLC
        /// </summary>
        public void PLCDisConnect()
        {
            if (_plc != null)
            {
                _plc.Close();
                LogHelp.AddLog<InfoLogEntity>($"PLC服务器断开连接！ ");
            }
        }

        /// <summary>
        /// 按照DataItem方式读取PLC数据
        /// </summary>
        /// <param name="dataItem"></param>
        /// <param name="type">Plc类型</param>
        public void PLC_Read(DataItem dataItem)
        {
            var type = S7DBAttribute.GetPlcType(dataItem.DB);
            var lis = new List<DataItem> { dataItem };
            _plc.ReadMultipleVars(lis);
        }

        /// <summary>
        /// 根据Lambda表达式读取Plc
        /// </summary>
        public F PLC_Read<T, F>(Expression<Func<T, F>> expression) where T : BaseDB where F: struct   
        {
            var dataItem = S7DataItemHelp.GetReadDataItem(expression);
            var lis = new List<DataItem> { dataItem };
            var plcType = S7DBAttribute.GetPlcType(dataItem.DB);
            _plc.ReadMultipleVars(lis);

            var f = (F)lis[0].Value;
            return f;
        }


        /// <summary>
        /// 开始读取
        /// </summary>
        /// <param name="dictDataItems">内容</param>
        /// <param name="sleepTimes"></param>
        public void StartRead(Dictionary<string, DataItem> dictDataItems, int sleepTimes = 300)
        {
            var type = S7DBAttribute.GetPlcType(dictDataItems.First().Value.DB);
            //备份的上次记录
            var dictDataItemsBackup = DeepCopy.DeepCopyByJson(dictDataItems);
            var lisDataItems = dictDataItems.Values.ToList();
            Task.Factory.StartNew(() =>
            {
                bool t = false;
                while (true)
                {
                    if (t)
                    {
                        Thread.Sleep(sleepTimes);
                        continue;
                    }

                    var start1 = DateTime.Now;
                    //单次同时读Plc有限制，超过限制得分多次读
                    if (lisDataItems.Count > LimitReadDataItemsCount)
                    {
                        var tempList = new List<DataItem>();
                        int count = lisDataItems.Count / LimitReadDataItemsCount;
                        for (var j = 0; j <= count; j++)
                        {
                            var items = lisDataItems.Skip(j * LimitReadDataItemsCount).Take(LimitReadDataItemsCount)
                                .ToList();
                            if (items.Count <= 0)
                                continue;
                            _plc.ReadMultipleVars(items);
                            tempList.AddRange(items);
                        }

                        lisDataItems = tempList;
                    }
                    else
                    {
                        _plc.ReadMultipleVars(lisDataItems);
                    }

                    var start2 = DateTime.Now;
                    var dictDataItemsTemp =
                        new Dictionary<string, DataItem>(lisDataItems.Count);
                    int i = 0;
                    foreach (var keyValuePair in dictDataItemsBackup)
                    {
                        try
                        {
                            DataItem currentDataItem = lisDataItems[i];
                            dictDataItemsTemp.Add(keyValuePair.Key, currentDataItem);
                            //不变更的属性不发布
                            if (Equals(keyValuePair.Value.Value, currentDataItem.Value))
                            {
                                continue;
                            }
                        }
                        catch (Exception e)
                        {
                            //to do 
                        }
                        finally
                        {
                            i++;
                        }
                    }

                    //保留备份数据
                    dictDataItemsBackup.Clear();
                    dictDataItemsBackup = DeepCopy.DeepCopyByJson(dictDataItemsTemp);

                    var ts1 = start2 - start1;
                    var ts2 = DateTime.Now - start2;
                    Thread.Sleep(20);

                }
            });
        }


        /// <summary>
        /// 通过初始偏移量试出DB块长度
        /// </summary>
        /// <param name="db">数据块地址</param>
        /// <param name="startByteAdr">建议设置为db块的估计长度，这样试探的次数最小</param>
        /// <param name="type">Plc类型</param>
        /// <returns></returns>
        public int PLC_ReadDbLength(int db, int startByteAdr)
        {
            var type = S7DBAttribute.GetPlcType(db);
            int? dbLength = S7DataItemHelp.GetDbLength(db);
            if (dbLength != null)
            {
                return dbLength.Value;
            }

            var catchError = false;
            var readSuccess = false;
            dbLength = 0;
            while (startByteAdr >= 0)
            {
                try
                {
                    var result =  _plc.ReadBytes(S7.Net.DataType.DataBlock, db, startByteAdr, 1);
                    if (catchError)
                    {
                        dbLength = startByteAdr + 1;
                        break;
                    }

                    readSuccess = true;
                    startByteAdr++;
                }
                catch (Exception)
                {
                    if (readSuccess)
                    {
                        dbLength = startByteAdr;
                        break;
                    }

                    startByteAdr--;
                    catchError = true;
                }
            }

            //记住这个值,避免反复试探
            S7DataItemHelp.SetDBValue(db, dbLength.Value);

            return dbLength.Value;
        }


        /// <summary>
        /// 写单个属性
        /// </summary>
        /// <typeparam name="T">DB类类型</typeparam>
        /// <typeparam name="F">DB类属性类型</typeparam>
        /// <param name="expression">表达式树</param>
        /// <param name="f">属性的值</param>
        public bool PLC_Write<T, F>(Expression<Func<T, F>> expression, F f) where T : BaseDB
        {
            var dataItem = S7DataItemHelp.GetDataItem(expression, f);
            try
            {
                var startTime = DateTime.Now;
                PLC_Write(dataItem);
                Debug.WriteLine($"{(DateTime.Now - startTime).TotalMilliseconds}   ");
                return true;
            }
            catch (Exception e)
            {
                //日志处理逻辑
                return false;
            }

        }

        /// <summary>
        /// 多个属性写入
        /// </summary>
        /// <param name="dataItems"></param>
        public bool PLC_Write(DataItem[] dataItems)
        {
            try
            {
                var type = S7DBAttribute.GetPlcType(dataItems.First().DB);
                var startTime = DateTime.Now;
                _plc.Write(dataItems);

                Debug.WriteLine($"{(DateTime.Now - startTime).TotalMilliseconds}   ");
                return true;
            }
            catch (Exception e)
            {
                //日志处理逻辑
                return false;
            }

        }


        /// <summary>
        /// 写DataItem
        /// </summary>
        public void PLC_Write(DataItem dataItem)
        {
            var type = S7DBAttribute.GetPlcType(dataItem.DB);
             _plc.Write(dataItem);
        }

    }
}
