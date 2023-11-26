using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7Communication
{
    /// <summary>
    /// S7数据块特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false)]
    public class S7DBAttribute : Attribute
    {
        /// <summary>
        /// 数据块地址
        /// </summary>
        public int DB;
        /// <summary>
        /// 存放所有数据块地址和Plc类型对应关系
        /// </summary>
        public static Dictionary<int, PlcPropertyType> S7DBDictionary = new Dictionary<int, PlcPropertyType>();

        /// <summary>
        /// 根据DB获取对应的DB类型
        /// </summary>
        /// <param name="db">数据块地址</param>
        /// <returns></returns>
        public static PlcPropertyType GetPlcType(int db)
        {
            return S7DBDictionary.ContainsKey(db) ? S7DBDictionary[db] : PlcPropertyType.MainPlc;
        }

        private PlcPropertyType _plcType = PlcPropertyType.MainPlc;

        /// <summary>
        /// Plc类型
        /// </summary>
        public PlcPropertyType PlcType
        {
            get
            {
                return _plcType;
            }
            set
            {
                _plcType = value;
                if (!S7DBDictionary.ContainsKey(DB))
                {
                    S7DBDictionary[DB] = _plcType;
                }
            }
        }
    }

    /// <summary>
    /// DB块字段属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class S7PropertyAttribute : Attribute
    {
        /// <summary>
        /// 起点地址
        /// </summary>
        public int StartByteAdr;

        /// <summary>
        /// 偏移量
        /// </summary>
        public byte BitAdr;
    }

    public enum PlcPropertyType
    {
        /// <summary>
        /// 主Plc
        /// </summary>
        MainPlc,
        /// <summary>
        /// 副Plc1
        /// </summary>
        SidePlcOne,
        //SidePlcTwo,
    }
}
