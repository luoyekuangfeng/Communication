using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace S7Communication
{
    /// <summary>
    /// 压缩文件帮助类
    /// </summary>
    public class ZipHelp
    {
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="info"></param>
        /// <param name="zipFlie"></param>
        public static void ZipFile(FileInfo info,string zipFlie)
        {
            using (var archive=ZipArchive.Create())
            {
                FileStream fileStream=info.OpenRead();
                archive.AddEntry(info.Name,fileStream);
                FileStream fs_scratchPath=new FileStream(zipFlie,FileMode.OpenOrCreate,FileAccess.Write);
                archive.SaveTo(fs_scratchPath, CompressionType.Deflate);
                fs_scratchPath.Close();
                fileStream.Dispose();
            }
        }

        /// <summary>
        /// 压缩文件夹
        /// </summary>
        /// <param name="targetFile">压缩文件夹路径</param>
        /// <param name="zipFile">压缩后路径</param>
        public static void Zips(string targetFile, string zipFile)
        {
            using (var archive = ZipArchive.Create())
            {
                ZipRecursion(targetFile, archive);
                FileStream fs_scratchPath = new FileStream(zipFile, FileMode.OpenOrCreate, FileAccess.Write);
                archive.SaveTo(fs_scratchPath, CompressionType.Deflate);
                fs_scratchPath.Close();
            }
        }

        /// <summary>
        /// 压缩递归
        /// </summary>
        /// <param name="fullName">压缩文件夹目录</param>
        /// <param name="archive">压缩实体</param>
        public static void ZipRecursion(string fullName, ZipArchive archive)
        {
            DirectoryInfo di = new DirectoryInfo(fullName);//获取需要压缩的文件夹信息
            foreach (var fi in di.GetDirectories())
            {
                if (Directory.Exists(fi.FullName))
                {
                    ZipRecursion(fi.FullName, archive);
                }
            }
            foreach (var f in di.GetFiles())
            {
                archive.AddEntry(f.FullName, f.OpenRead());//添加文件夹中的文件
            }
        }

        /// <summary>
        /// 解压文件
        /// </summary>
        /// <param name="targetFile">解压文件路径</param>
        /// <param name="zipFile">解压文件后路径</param>
        public static void Decompression(string targetFile, string zipFile)
        {
            var archive = ArchiveFactory.Open(targetFile);
            foreach (var entry in archive.Entries)
            {
                if (!entry.IsDirectory)
                {
                    entry.WriteToDirectory(zipFile);
                }
            }
        }
    }
}
