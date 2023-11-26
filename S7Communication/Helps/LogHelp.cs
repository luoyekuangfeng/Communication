using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7Communication
{
    /// <summary>
    /// 日志帮助类
    /// </summary>
    public class LogHelp
    {
        public static ConcurrentQueue<LogEntity> m_LogEntityCache = new ConcurrentQueue<LogEntity>();

        private static bool m_HaveStartLogServer = false;

        public static bool IsContinueWrite = true;

        public static readonly string LogFolderPath = $"{AppDomain.CurrentDomain.BaseDirectory}Logs";

        public static Dictionary<string, StreamWriter> DictStreamWriters = new Dictionary<string, StreamWriter>();

        //private static StreamWriterHelp m_StreamWriterHelp = new StreamWriterHelp();



        static LogHelp()
        {
            //1-2个线程写日志
            Task.Factory.StartNew(DealLog, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(DealLog, TaskCreationOptions.LongRunning);
        }

        /// <summary>
        /// 收到日志消息
        /// </summary>
        /// <param name="logEntity"></param>
        public static void OnLogEntity(LogEntity logEntity)
        {
            //写日志
            LogHelp.WriteLog(logEntity);
        }

        public static void Init()
        {

        }


        public static void Dispose()
        {

        }

        private static void DealLog()
        {
            while (true)
            {
                try
                {

                    if (!m_LogEntityCache.TryDequeue(out LogEntity logEntity))
                    {
                        Thread.Sleep(100);
                        continue;
                    }

                    LogHelp.WriteLog(logEntity);
                }
                catch (Exception e)
                {

                }
            }
        }

        private static Byte[] GetEntityByte(object logEntity)
        {
            byte[] sendBytes = SerializeHelp.SerializeToByteArray(logEntity);
            Byte[] lengthBytes = BitConverter.GetBytes(sendBytes.Length);

            byte[] finBytes = new byte[sendBytes.Length + lengthBytes.Length];
            //合并两个byte流
            Buffer.BlockCopy(lengthBytes, 0, finBytes, 0, lengthBytes.Length);//这种方法仅适用于字节数组
            Buffer.BlockCopy(sendBytes, 0, finBytes, lengthBytes.Length, sendBytes.Length);

            return finBytes;
        }


        /// <summary>
        /// 添加日志(将日志加入缓冲区)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logEntity"></param>
        public static void AddLog<T>(T logEntity) where T : LogEntity
        {
            if (!IsContinueWrite)
            {
                return;
            }
            m_LogEntityCache.Enqueue(logEntity);
        }

        /// <summary>
        /// 正式写日志
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="logEntity"></param>
        public static void WriteLog<T>(T logEntity) where T : LogEntity
        {
            string writeValue = logEntity.ToString();
            Type logType = logEntity.GetType();
            List<string> lisFolderPath = logEntity.GetLogFiledPath();
            string logFolderPath = @$"{LogFolderPath}/{DateTime.Now.ToString("yyyyMMdd")}";
            foreach (string s in lisFolderPath)
            {
                logFolderPath += @$"/{s}";
            }
            if (!Directory.Exists(logFolderPath))
            {
                Directory.CreateDirectory(logFolderPath);
            }
            string logFilePath = @$"{logFolderPath}/{logEntity.GetLogFiledName()}";
            FileInfo fileInfo = new FileInfo(logFilePath);

            //大于10 M压缩并删除文件

            lock (logType)
            {
                if (fileInfo.Exists && fileInfo.Length > 10_000_000)
                {
                    if (DictStreamWriters.TryGetValue(logFilePath, out StreamWriter streamWriter))
                    {
                        streamWriter.Dispose();
                        streamWriter.Close();
                        DictStreamWriters.Remove(logFilePath);
                    }
                    string zipFilePath = @$"{logFolderPath}/{logEntity.GetLogFiledName().TrimEnd(".txt".ToCharArray())}_{DateTime.Now.ToString("HHmmssfff")}.zip";

                    ZipHelp.ZipFile(fileInfo, zipFilePath);
                    fileInfo.Delete();
                }
            }
            lock (logType)
            {
                if (!DictStreamWriters.TryGetValue(logFilePath, out StreamWriter streamWriter))
                {
                    streamWriter = new StreamWriter(logFilePath, true, Encoding.UTF8);
                    DictStreamWriters.Add(logFilePath, streamWriter);
                }

                streamWriter.Write(writeValue);
                streamWriter.Flush();
            }
            //}
        }

        //
        public static void GetLogFilePath<T>() where T : LogEntity
        {

        }

    }
}
