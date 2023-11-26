using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7Communication.DBData
{
    /// <summary>
    /// 风扇
    /// </summary>
    [S7DB(DB = 101)]
    public class FanDB_W : BaseDB
    {
        /// <summary>
        /// 风扇1功率百分比值（前）
        /// </summary>
        [S7Property(StartByteAdr = 0, BitAdr = 0)]
        public float FanPowerFront { get; set; }

        /// <summary>
        /// 风扇2功率百分比值（后）
        /// </summary>
        [S7Property(StartByteAdr = 4, BitAdr = 1)]
        public float FanPowerLeftRear { get; set; }

    }
}
