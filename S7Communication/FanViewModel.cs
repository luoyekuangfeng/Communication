using S7.Net.Types;
using S7Communication.DBData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7Communication
{
    /// <summary>
    /// 风扇类
    /// </summary>
    public class FanViewModel
    {
        public static FanViewModel Instance=new FanViewModel();

        private FanViewModel()
        {
            //读取逻辑
        }

        /// <summary>
        /// 风扇1功率百分比值（左前）
        /// </summary>
        public float FanPowerFront { get; set; }

        /// <summary>
        /// 风扇2功率百分比值（左后）
        /// </summary>
        public float FanPowerRear { get; set; }



        /// <summary>
        /// 启停前面的风扇
        /// </summary>
        /// <param name="isOpen">true:开启 false:关闭</param>
        /// <param name="PowerLeftFront">功率</param>
        public void FanPowerFrontOpen(bool isOpen,float powerValue=0)
        {
            if (isOpen)
            {
                S7CommunicationHelp.Instance.PLC_Write<FanDB_W, float>(x => x.FanPowerFront, powerValue);
            }
            else
            {
                S7CommunicationHelp.Instance.PLC_Write<FanDB_W, float>(x => x.FanPowerFront, 0);
            }

        }

        /// <summary>
        /// 启停后面的风扇
        /// </summary>
        /// <param name="isOpen">true:开启 false:关闭</param>
        /// <param name="PowerLeftFront">功率</param>
        public void FanPowerLeftRearOpen(bool isOpen, float powerValue = 0)
        {
            if (isOpen)
            {
                S7CommunicationHelp.Instance.PLC_Write<FanDB_W, float>(x => x.FanPowerFront, powerValue);
            }
            else
            {
                S7CommunicationHelp.Instance.PLC_Write<FanDB_W, float>(x => x.FanPowerFront, 0);
            }

        }
    }
}
