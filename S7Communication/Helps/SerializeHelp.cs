using Newtonsoft.Json;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7Communication
{
    public class SerializeHelp
    {
        public static byte[] SerializeToByteArray(object obj)
        {
            byte[] buff = null;
            try
            {
                if (obj is byte[] bytes)
                {
                    string json = Convert.ToBase64String(bytes);
                    buff = Encoding.UTF8.GetBytes(json);
                }
                else
                {
                    string json = JsonConvert.SerializeObject(obj);
                    buff = Encoding.UTF8.GetBytes(json);
                }
                return buff;
            }
            catch (Exception e)
            {
                ErrorLogEntity errorLogEntity = new ErrorLogEntity("序列化化异常", e);
                LogHelp.AddLog<ErrorLogEntity>(errorLogEntity);
                return null;
            }

        }
    }
}
