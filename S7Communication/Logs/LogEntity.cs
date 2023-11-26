using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7Communication
{
    /// <summary>
    /// 日志的实体
    /// </summary>
    public abstract class LogEntity
    {
        /// <summary>
        /// 日志记录时间
        /// </summary>
        public DateTime LogTime = DateTime.Now;

        /// <summary>
        /// 日志内容
        /// </summary>
        public string Content;

        /// <summary>
        /// 日志文件路径
        /// </summary>
        /// <returns></returns>
        public virtual List<string> GetLogFiledPath()
        {
            List<string> listPaths=new List<string>();
            return listPaths;
        }

        /// <summary>
        /// 日志文件名称
        /// </summary>
        /// <returns></returns>
        public virtual string GetLogFiledName()
        {
            return $"{this.GetType().Name}.txt";
        }

        public new virtual string ToString()
        {
            return $"{{ {LogTime.ToString("yyyyMMdd HH: mm:ss: fff")}   {Content}}} \r\n";
        }
    }
}
