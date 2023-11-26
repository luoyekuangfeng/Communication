using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7Communication
{
    /// <summary>
    /// 异常日志
    /// </summary>
    [Serializable]
    public class ErrorLogEntity : LogEntity
    {

        public ErrorLogEntity(Exception exception)
        {
            Content = exception.StackTrace;
        }

        public ErrorLogEntity(string content)
        {
            Content = content;
        }

        public ErrorLogEntity(string content, Exception exception)
        {
            string s = exception.StackTrace;
            if (string.IsNullOrEmpty(s))
            {
                s = exception.Message;
            }
            Content = $"{content}  异常信息   {s}";
        }

        public static implicit operator ErrorLogEntity(Exception exception)
        {
            return new ErrorLogEntity(exception);
        }

        public static implicit operator ErrorLogEntity(string content)
        {
            return new ErrorLogEntity(content);
        }

        public override string GetLogFiledName()
        {
            return $"异常.txt";
        }
    }
}
