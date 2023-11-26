using S7.Net;
using S7.Net.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace S7Communication
{
    /// <summary>
    /// s7数据帮助类
    /// </summary>
    public static class S7DataItemHelp
    {
        /// <summary>
        /// DB块长度字典
        /// </summary>
        public static Dictionary<int, int> DictDbLength = new Dictionary<int, int>();

        private static readonly object m_LockDictDbLength = new object();

        /// <summary>
        /// 获取数据块长度
        /// </summary>
        /// <param name="db">数据库长度</param>
        /// <returns></returns>
        public static int? GetDbLength(int db)
        {
            if (!DictDbLength.TryGetValue(db, out var value))
            {
                return null;
            }
            return value;
        }

        /// <summary>
        /// 记录数据块
        /// </summary>
        /// <param name="db">数据块长度</param>
        /// <param name="value">数据库的值</param>
        public static void SetDBValue(int db, int value)
        {
            lock (m_LockDictDbLength)
            {
                if (DictDbLength.ContainsKey(db))
                {
                    return;
                }
                else
                {
                    DictDbLength.Add(db, value);
                }
            }
        }
        /// <summary>
        /// 根据Lambda表达式获取字段名称字符串
        /// </summary>
        /// <typeparam name="T">要获取的类型</typeparam>
        /// <typeparam name="F">数据类型</typeparam>
        /// <param name="expression">Lambda表达式树对象</param>
        /// <param name="f">返回的数据类型</param>
        /// <returns></returns>
        public static DataItem GetDataItem<T, F>(Expression<Func<T, F>> expression, F f) where T : BaseDB
        {
            string propertyName = PropertyHelp.GetPropertyName(expression);
            return GetDataItem<T, F>(propertyName, f);
        }

        /// <summary>
        /// 获取数据块
        /// </summary>
        /// <typeparam name="T">要获取的类型</typeparam>
        /// <typeparam name="F">数据类型</typeparam>
        /// <param name="expression">Lambda表达式树对象</param>
        /// <param name="f">返回的数据类型</param>
        /// <returns></returns>
        public static DataItem GetDataItem<T, F>(string propertyName, F f) where T : BaseDB
        {
            DataItem dataItem = new DataItem();
            dataItem.DataType = DataType.DataBlock;
            dataItem.DB = S7AttributeHelp.GetDB<T>();
            var adressInfo = S7AttributeHelp.GetAdress<T>(propertyName);
            dataItem.StartByteAdr = adressInfo.StartByteAdr;
            dataItem.BitAdr = adressInfo.BitAdr;
            dataItem.Value = f;
            dataItem.Count = 1;
            dataItem.VarType = GetVarType<F>(f);
            return dataItem;
        }

        /// <summary>
        /// 根据Lambda表达式读取
        /// </summary>
        /// <typeparam name="T">要获取的类型</typeparam>
        /// <typeparam name="F">数据类型</typeparam>
        /// <param name="expression">Lambda表达式树对象</param>
        /// <returns></returns>
        public static DataItem GetReadDataItem<T, F>(Expression<Func<T, F>> expression) where T : BaseDB
        {
            string propertyName = PropertyHelp.GetPropertyName(expression);
            return GetReadDataItem<T, F>(propertyName);
        }

        /// <summary>
        /// 读取数据块
        /// </summary>
        /// <typeparam name="T">要获取的类型</typeparam>
        /// <typeparam name="F">数据类型</typeparam>
        /// <param name="expression">Lambda表达式树对象</param>
        /// <returns></returns>
        public static DataItem GetReadDataItem<T, F>(string propertyName) where T : BaseDB
        {
            DataItem dataItem = new DataItem();
            dataItem.DataType = DataType.DataBlock;
            dataItem.DB = S7AttributeHelp.GetDB<T>();
            var adressInfo = S7AttributeHelp.GetAdress<T>(propertyName);
            dataItem.StartByteAdr = adressInfo.StartByteAdr;
            dataItem.BitAdr = adressInfo.BitAdr;
            dataItem.Value = new object();
            dataItem.Count = 1;
            dataItem.VarType = GetVarType(typeof(F));
            return dataItem;
        }

        public static KeyValuePair<string, DataItem> GetReadDataItemPare<T, F>(Expression<Func<T, F>> expression)
          where T : BaseDB
        {
            string propertyName = PropertyHelp.GetPropertyName(expression);
            return GetReadDataItemPare<T, F>(propertyName);
        }

        public static KeyValuePair<string, DataItem> GetReadDataItemPare<T, F>(string propertyName) where T : BaseDB
        {
            Type type = typeof(T);
            string contract = $"{type.FullName}_{propertyName}";
            DataItem dataItem = new DataItem();
            dataItem.DataType = DataType.DataBlock;
            dataItem.DB = S7AttributeHelp.GetDB<T>();
            var adressInfo = S7AttributeHelp.GetAdress<T>(propertyName);
            dataItem.StartByteAdr = adressInfo.StartByteAdr;
            dataItem.BitAdr = adressInfo.BitAdr;
            dataItem.Value = new object();
            dataItem.Count = 1;
            dataItem.VarType = GetVarType(typeof(F));
            return new(contract, dataItem);
        }


        /// <summary>
        /// 拿取整个类的DataItem
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<DataItem> GetReadDataItems<T>() where T : BaseDB
        {
            List<DataItem> result = new List<DataItem>();
            Type type = typeof(T);

            int db = S7AttributeHelp.GetDB<T>();
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                S7PropertyAttribute s7Property = (S7PropertyAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(S7PropertyAttribute));
                //不是S7的属性  直接跳过
                if (s7Property == null)
                {
                    continue;
                }
                DataItem dataItem = new DataItem();
                dataItem.DataType = DataType.DataBlock;
                dataItem.DB = db;
                dataItem.VarType = GetVarType(propertyInfo.PropertyType);
                dataItem.StartByteAdr = s7Property.StartByteAdr;
                dataItem.BitAdr = s7Property.BitAdr;
                dataItem.Count = 1;
                dataItem.Value = new object();
                result.Add(dataItem);
            }

            return result;
        }

        /// <summary>
        /// 拿取整个类的DataItem
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<PropertyDataItem> GetPropertyDataItems<T>() where T : BaseDB
        {
            List<PropertyDataItem> result = new List<PropertyDataItem>();
            Type type = typeof(T);

            int db = S7AttributeHelp.GetDB<T>();
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                S7PropertyAttribute s7Property = (S7PropertyAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(S7PropertyAttribute));
                //不是S7的属性  直接跳过
                if (s7Property == null)
                {
                    continue;
                }
                PropertyDataItem dataItem = new PropertyDataItem();
                dataItem.DataType = DataType.DataBlock;
                dataItem.DB = db;
                dataItem.VarType = GetVarType(propertyInfo.PropertyType);
                dataItem.StartByteAdr = s7Property.StartByteAdr;
                dataItem.BitAdr = s7Property.BitAdr;
                dataItem.Count = 1;
                dataItem.Value = new object();
                dataItem.PropertyInfo = propertyInfo;
                result.Add(dataItem);
            }

            return result;
        }

        /// <summary>
        /// 兼容旧DB块获取整个类的DataItem
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<PropertyDataItem> GetCompatiblePropertyDataItems<T>() where T : BaseDB
        {
            var type = typeof(T);
            var db = S7AttributeHelp.GetDB<T>();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            int? estimateLength = ((S7PropertyAttribute)Attribute.GetCustomAttribute(propertyInfos.Last(), typeof(S7PropertyAttribute)))?.StartByteAdr;
            int dbLength = estimateLength != null ? S7CommunicationHelp.Instance.PLC_ReadDbLength(db, (int)estimateLength) : S7CommunicationHelp.Instance.PLC_ReadDbLength(db, 0);
            var dataItems = (from propertyInfo in propertyInfos
                             let s7Property = (S7PropertyAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(S7PropertyAttribute))
                             where s7Property != null && s7Property.StartByteAdr + GetVarTypeLength(GetVarType(propertyInfo.PropertyType)) <= dbLength
                             select new PropertyDataItem
                             {
                                 DataType = DataType.DataBlock,
                                 DB = db,
                                 VarType = GetVarType(propertyInfo.PropertyType),
                                 StartByteAdr = s7Property.StartByteAdr,
                                 BitAdr = s7Property.BitAdr,
                                 Count = 1,
                                 Value = new object(),
                                 PropertyInfo = propertyInfo
                             }).ToList();

            return dataItems;
        }

        /// <summary>
        /// 获取属性列表所需的DB长度
        /// </summary>
        public static int GetPropertiesToDbLength(int db, List<PropertyDataItem> dataItems)
        {
            if (dataItems.Count == 0)
                return 0;
            var propertyDataItems = dataItems.Where(c => c.DB == db).ToList();
            if (!propertyDataItems.Any())
                return 0;
            return propertyDataItems.Last().StartByteAdr + GetVarTypeLength(propertyDataItems.Last().VarType);
        }

        /// <summary>
        /// 获取属性列表所需的DB长度
        /// </summary>
        public static int GetPropertiesToDbLength(int db, Dictionary<string, DataItem> dataItems)
        {
            if (dataItems.Count == 0)
                return 0;
            var propertyDataItems = dataItems.Where(c => c.Value.DB == db).ToList();
            if (!propertyDataItems.Any())
                return 0;
            return propertyDataItems.Last().Value.StartByteAdr + GetVarTypeLength(propertyDataItems.Last().Value.VarType);
        }

        /// <summary>
        /// 拿取整个类的DataItem
        /// </summary>
        public static Dictionary<string, DataItem> GetCompatibleReadDictDataItems<T>() where T : BaseDB
        {
            var result = new Dictionary<string, DataItem>(30);
            var type = typeof(T);

            var db = S7AttributeHelp.GetDB<T>();
            var propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            var estimateLength = ((S7PropertyAttribute)Attribute.GetCustomAttribute(propertyInfos.Last(), typeof(S7PropertyAttribute)))?.StartByteAdr;
            var dbLength = estimateLength != null ? S7CommunicationHelp.Instance.PLC_ReadDbLength(db, (int)estimateLength) : S7CommunicationHelp.Instance.PLC_ReadDbLength(db, 0);
            foreach (var propertyInfo in propertyInfos)
            {
                var s7Property = (S7PropertyAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(S7PropertyAttribute));
                //不是S7的属性  直接跳过
                if (s7Property == null)
                {
                    continue;
                }

                //只读取db长度内的DataItem
                if (s7Property.StartByteAdr + S7DataItemHelp.GetVarTypeLength(GetVarType(propertyInfo.PropertyType)) > dbLength)
                {
                    continue;
                }

                var dataItem = new DataItem
                {
                    DataType = DataType.DataBlock,
                    DB = db,
                    VarType = GetVarType(propertyInfo.PropertyType),
                    StartByteAdr = s7Property.StartByteAdr,
                    BitAdr = s7Property.BitAdr,
                    Count = 1,
                    Value = new object()
                };
                string contract = $"{type.FullName}_{propertyInfo.Name}";

                result.TryAdd(contract, dataItem);
            }

            return result;
        }

        /// <summary>
        /// 拿取整个类的DataItem
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static Dictionary<string, DataItem> GetReadDictDataItems<T>() where T : BaseDB
        {
            Dictionary<string, DataItem> result = new Dictionary<string, DataItem>(30);
            Type type = typeof(T);

            int db = S7AttributeHelp.GetDB<T>();
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                S7PropertyAttribute s7Property = (S7PropertyAttribute)Attribute.GetCustomAttribute(propertyInfo, typeof(S7PropertyAttribute));
                //不是S7的属性  直接跳过
                if (s7Property == null)
                {
                    continue;
                }
                DataItem dataItem = new DataItem();
                dataItem.DataType = DataType.DataBlock;
                dataItem.DB = db;
                dataItem.VarType = GetVarType(propertyInfo.PropertyType);
                dataItem.StartByteAdr = s7Property.StartByteAdr;
                dataItem.BitAdr = s7Property.BitAdr;
                dataItem.Count = 1;
                dataItem.Value = new object();
                string contract = $"{type.FullName}_{propertyInfo.Name}";
                result.TryAdd(contract, dataItem);
            }
            return result;
        }

        /// <summary>
        /// 拿取类中的部分信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static DataItem GetReadCalssItem<T>() where T : BaseDB
        {
            DataItem dataItem = new DataItem();
            Type type = typeof(T);

            int db = S7AttributeHelp.GetDB<T>();
            PropertyInfo[] propertyInfos = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static);
            S7PropertyAttribute s7Property = (S7PropertyAttribute)Attribute.GetCustomAttribute(propertyInfos[0], typeof(S7PropertyAttribute));
            //不是S7的属性  直接跳过
            if (s7Property != null)
            {
                dataItem.DataType = DataType.DataBlock;
                dataItem.DB = db;
                dataItem.VarType = GetVarType(propertyInfos[0].PropertyType);
                dataItem.StartByteAdr = s7Property.StartByteAdr;
                dataItem.BitAdr = s7Property.BitAdr;
                dataItem.Count = 1;
            }
            return dataItem;

        }


        public static VarType GetVarType<F>(F f)
        {
            VarType result = VarType.Byte;
            if (f is bool bol)
            {
                return VarType.Bit;
            }

            //短整型
            if (f is short)
            {
                return VarType.Int;
            }

            //浮点型
            if (f is float)
            {
                return VarType.Real;
            }

            //byte类型
            if (f is byte)
            {
                return VarType.Byte;
            }

            throw new Exception("不能解析的类型");
        }

        public static VarType GetVarType(Type type)
        {
            VarType result = VarType.Byte;
            if (type == typeof(bool))
            {
                return VarType.Bit;
            }

            //短整型
            if (type == typeof(short))
            {
                return VarType.Int;
            }

            //浮点型
            if (type == typeof(float))
            {
                return VarType.Real;
            }

            //byte类型
            if (type == typeof(byte))
            {
                return VarType.Byte;
            }

            throw new Exception("不能解析的类型");
        }


        public static Type GetSystemType(VarType? type)
        {
            if (type == VarType.Bit)
            {
                return typeof(bool);
            }

            //短整型
            if (type == VarType.Int)
            {
                return typeof(short);
            }

            //浮点型
            if (type == VarType.Real)
            {
                return typeof(float);
            }

            //byte类型
            if (type == VarType.Byte)
            {
                return typeof(byte);
            }

            throw new Exception("不能解析的类型");
        }

        public static VarType GetVarType(string type)
        {
            VarType result = VarType.Byte;
            if (type == "bool")
            {
                return VarType.Bit;
            }

            //短整型
            if (type == "short")
            {
                return VarType.Int;
            }

            //浮点型
            if (type == "float")
            {
                return VarType.Real;
            }

            //byte类型
            if (type == "byte")
            {
                return VarType.Byte;
            }

            throw new Exception("不能解析的类型");
        }

        public static int GetVarTypeLength(VarType type)
        {
            if (type == VarType.Bit)
            {
                return 1;
            }

            //短整型
            if (type == VarType.Int)
            {
                return 2;
            }

            //浮点型
            if (type == VarType.Real)
            {
                return 4;
            }

            //byte类型
            if (type == VarType.Byte)
            {
                return 1;
            }

            throw new Exception("不能解析的类型");
        }



        public static int GetVarTypeLength<T>(T t)
        {
            if (t is VarType.Bit)
            {
                return 1;
            }

            //短整型
            if (t is VarType.Int)
            {
                return 2;
            }

            //浮点型
            if (t is VarType.Real)
            {
                return 4;
            }

            //byte类型
            if (t is VarType.Byte)
            {
                return 1;
            }

            throw new Exception("不能解析的类型");
        }

    }
}
