using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7Communication
{
    /// <summary>
    /// 初始化日志
    /// </summary>
    [Serializable]
    public class InfoLogEntity : LogEntity
    {
        public InfoLogEntity(string message)
        {
            Content = message;
        }

        public static implicit operator InfoLogEntity(string message)
        {
            return new InfoLogEntity(message);
        }

        public override string GetLogFiledName()
        {
            return $"信息.txt";
        }
    }

}
