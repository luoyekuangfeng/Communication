using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7Communication
{
   /// <summary>
   /// S7协议用到的枚举
   /// </summary>
    public class S7Enums
    {
        /// <summary>
        /// 数据类型
        /// </summary>
        public enum DataTypes
        {
            Bool,
            Byte,
            Short,
            UShort,
            Int,
            UInt,
            Float,
            Double,
            Long,
            ULong,
            String
        }

        /// <summary>
        /// 读取方式
        /// </summary>
        public enum ReadMethodType
        {
            Read,
            ReadBytes,
            ReadClass,
            ReadStruct
        }

        /// <summary>
        /// 写入方式
        /// </summary>
        public enum WriteMothodType
        {
            Write,
            WriteBytes,
            WriteClass,
            WriteStruct
        }

        /// <summary>
        /// 可支持的PLC类型
        /// </summary>
        public enum PLC_CPUType
        {
            //
            // 摘要:
            //     S7 200 cpu type
            S7200 = 0,
            //
            // 摘要:
            //     Siemens Logo 0BA8
            Logo0BA8 = 1,
            //
            // 摘要:
            //     S7 200 Smart
            S7200Smart = 2,
            //
            // 摘要:
            //     S7 300 cpu type
            S7300 = 10,
            //
            // 摘要:
            //     S7 400 cpu type
            S7400 = 20,
            //
            // 摘要:
            //     S7 1200 cpu type
            S71200 = 30,
            //
            // 摘要:
            //     S7 1500 cpu type
            S71500 = 40
        }
    }
}
