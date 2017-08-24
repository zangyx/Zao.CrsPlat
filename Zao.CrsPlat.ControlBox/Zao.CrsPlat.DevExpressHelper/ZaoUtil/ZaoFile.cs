using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace Zao.CrsPlat.DevExpressHelper.ZaoUtil
{
    /// <summary>
    /// 常用的文件操作辅助类FileUtil
    /// </summary>
    public class ZaoFile {


        /// <summary>
        /// 将流读取到缓冲区中
        /// </summary>
        /// <param name="stream">原始流</param>
        public static byte[] StreamToBytes(Stream stream) {
            try {
                var buffer = new byte[stream.Length];


                stream.Read(buffer, 0, Convert.ToInt32(stream.Length));


                return buffer;
            } catch (IOException ex) {
                throw ex;
            }
            finally {
                stream.Close();
            }
        }

        /// <summary>
        /// 将 byte[] 转成 Stream
        /// </summary>
        public static Stream BytesToStream(byte[] bytes) {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        /// <summary>
        /// 将 Stream 写入文件
        /// </summary>
        public static void StreamToFile(Stream stream, string fileName) {
            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            stream.Seek(0, SeekOrigin.Begin);

            var fs = new FileStream(fileName, FileMode.Create);
            var bw = new BinaryWriter(fs);
            bw.Write(bytes);
            bw.Close();
            fs.Close();
        }

        /// <summary>
        /// 从文件读取 Stream
        /// </summary>
        public static Stream FileToStream(string fileName) {
            var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);

            var bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();

            Stream stream = new MemoryStream(bytes);
            return stream;
        }

        /// <summary>
        /// 将文件读取到缓冲区中
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static byte[] FileToBytes(string filePath) {
            var fileSize = GetFileSize(filePath);


            var buffer = new byte[fileSize];


            var fi = new FileInfo(filePath);
            var fs = fi.Open(FileMode.Open);

            try {
                fs.Read(buffer, 0, fileSize);

                return buffer;
            } catch (IOException ex) {
                throw ex;
            }
            finally {
                fs.Close();
            }
        }

        /// <summary>
        /// 将文件读取到字符串中
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static string FileToString(string filePath) {
            return FileToString(filePath, Encoding.Default);
        }

        /// <summary>
        /// 将文件读取到字符串中
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="encoding">字符编码</param>
        public static string FileToString(string filePath, Encoding encoding) {
            try {
                using (var reader = new StreamReader(filePath, encoding)) {
                    return reader.ReadToEnd();
                }
            } catch (IOException ex) {
                throw ex;
            }
        }

        /// <summary>
        /// 从嵌入资源中读取文件内容(e.g: xml).
        /// </summary>
        /// <param name="fileWholeName">嵌入资源文件名，包括项目的命名空间.</param>
        /// <returns>资源中的文件内容.</returns>
        public static string ReadFileFromEmbedded(string fileWholeName) {
            var result = string.Empty;

            using (TextReader reader = new StreamReader(
                Assembly.GetExecutingAssembly().GetManifestResourceStream(fileWholeName))) {
                result = reader.ReadToEnd();
            }
            return result;
        }





        /// <summary>
        /// 获取文件编码
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <returns></returns>
        public static Encoding GetEncoding(string filePath) {
            return GetEncoding(filePath, Encoding.Default);
        }

        /// <summary>
        /// 获取文件编码
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <param name="defaultEncoding">找不到则返回这个默认编码</param>
        /// <returns></returns>
        public static Encoding GetEncoding(string filePath, Encoding defaultEncoding) {
            var targetEncoding = defaultEncoding;
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4)) {
                if (fs != null && fs.Length >= 2) {
                    var pos = fs.Position;
                    fs.Position = 0;
                    var buffer = new int[4];


                    buffer[0] = fs.ReadByte();
                    buffer[1] = fs.ReadByte();
                    buffer[2] = fs.ReadByte();
                    buffer[3] = fs.ReadByte();

                    fs.Position = pos;

                    if (buffer[0] == 0xFE && buffer[1] == 0xFF) {
                        targetEncoding = Encoding.BigEndianUnicode;
                    }
                    if (buffer[0] == 0xFF && buffer[1] == 0xFE) {
                        targetEncoding = Encoding.Unicode;
                    }
                    if (buffer[0] == 0xEF && buffer[1] == 0xBB && buffer[2] == 0xBF) {
                        targetEncoding = Encoding.UTF8;
                    }
                }
            }

            return targetEncoding;
        }






        /// <summary>
        /// 获取一个文件的长度,单位为Byte
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static int GetFileSize(string filePath) {
            var fi = new FileInfo(filePath);


            return (int)fi.Length;
        }

        /// <summary>
        /// 获取一个文件的长度,单位为KB
        /// </summary>
        /// <param name="filePath">文件的路径</param>
        public static double GetFileSizeKB(string filePath) {
            var fi = new FileInfo(filePath);


            return ZaoDataType.ToDouble(Convert.ToDouble(fi.Length) / 1024, 1);
        }

        /// <summary>
        /// 获取一个文件的长度,单位为MB
        /// </summary>
        /// <param name="filePath">文件的路径</param>
        public static double GetFileSizeMB(string filePath) {
            var fi = new FileInfo(filePath);


            return ZaoDataType.ToDouble(Convert.ToDouble(fi.Length) / 1024 / 1024, 1);
        }


        /// <summary>
        /// 向文本文件中写入内容
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="content">写入的内容</param>
        public static void WriteText(string filePath, string content) {
            File.WriteAllText(filePath, content, Encoding.Default);
        }

        /// <summary>
        /// 向文本文件的尾部追加内容
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="content">写入的内容</param>
        public static void AppendText(string filePath, string content) {
            File.AppendAllText(filePath, content, Encoding.Default);
        }

        /// <summary>
        /// 将源文件的内容复制到目标文件中
        /// </summary>
        /// <param name="sourceFilePath">源文件的绝对路径</param>
        /// <param name="destFilePath">目标文件的绝对路径</param>
        public static void Copy(string sourceFilePath, string destFilePath) {
            File.Copy(sourceFilePath, destFilePath, true);
        }

        /// <summary>
        /// 将文件移动到指定目录
        /// </summary>
        /// <param name="sourceFilePath">需要移动的源文件的绝对路径</param>
        /// <param name="descDirectoryPath">移动到的目录的绝对路径</param>
        public static void Move(string sourceFilePath, string descDirectoryPath) {
            var sourceFileName = GetFileName(sourceFilePath);

            if (Directory.Exists(descDirectoryPath)) {
                if (IsExistFile(descDirectoryPath + "\\" + sourceFileName)) {
                    DeleteFile(descDirectoryPath + "\\" + sourceFileName);
                }

                File.Move(sourceFilePath, descDirectoryPath + "\\" + sourceFileName);
            }
        }

        /// <summary>
        /// 检测指定文件是否存在,如果存在则返回true。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static bool IsExistFile(string filePath) {
            return File.Exists(filePath);
        }

        /// <summary>
        /// 创建一个文件。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void CreateFile(string filePath) {
            try {
                if (!IsExistFile(filePath)) {
                    File.Create(filePath).Close();
                }
            } catch (IOException ex) {
                throw ex;
            }
        }

        /// <summary>
        /// 创建一个文件,并将字节流写入文件。
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        /// <param name="buffer">二进制流数据</param>
        public static void CreateFile(string filePath, byte[] buffer) {
            try {
                if (!IsExistFile(filePath)) {
                    using (var fs = File.Create(filePath)) {
                        fs.Write(buffer, 0, buffer.Length);
                    }
                }
            } catch (IOException ex) {
                throw ex;
            }
        }

        /// <summary>
        /// 获取文本文件的行数
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static int GetLineCount(string filePath) {
            var rows = File.ReadAllLines(filePath);


            return rows.Length;
        }

        /// <summary>
        /// 从文件的绝对路径中获取文件名( 包含扩展名 )
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static string GetFileName(string filePath) {
            var fi = new FileInfo(filePath);
            return fi.Name;
        }

        /// <summary>
        /// 从文件的绝对路径中获取文件名( 不包含扩展名 )
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static string GetFileNameNoExtension(string filePath) {
            var fi = new FileInfo(filePath);
            return fi.Name.Substring(0, fi.Name.LastIndexOf('.'));
        }

        /// <summary>
        /// 从文件的绝对路径中获取扩展名
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static string GetExtension(string filePath) {
            var fi = new FileInfo(filePath);
            return fi.Extension;
        }

        /// <summary>
        /// 清空文件内容
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void ClearFile(string filePath) {
            File.Delete(filePath);
            CreateFile(filePath);
        }

        /// <summary>
        /// 删除指定文件
        /// </summary>
        /// <param name="filePath">文件的绝对路径</param>
        public static void DeleteFile(string filePath) {
            if (IsExistFile(filePath)) {
                File.Delete(filePath);
            }
        }

        /// <summary>
        /// 文件是否存在或无权访问
        /// </summary>
        /// <param name="path">相对路径或绝对路径</param>
        /// <returns>如果是目录也返回false</returns>
        public static bool FileIsExist(string path) {
            return File.Exists(path);
        }

        /// <summary>
        /// 文件是否只读
        /// </summary>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static bool FileIsReadOnly(string fullpath) {
            var file = new FileInfo(fullpath);
            if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly) {
                return true;
            } else {
                return false;
            }
        }

        /// <summary>
        /// 设置文件是否只读
        /// </summary>
        /// <param name="fullpath"></param>
        /// <param name="flag">true表示只读，反之</param>
        public static void SetFileReadonly(string fullpath, bool flag) {
            var file = new FileInfo(fullpath);

            if (flag) {
                file.Attributes |= FileAttributes.ReadOnly;
            } else {
                file.Attributes &= ~FileAttributes.ReadOnly;
            }
        }

        /// <summary>
        /// 取文件名
        /// </summary>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static string GetFileName(string fullpath, bool removeExt) {
            var fi = new FileInfo(fullpath);
            var name = fi.Name;
            if (removeExt) {
                name = name.Remove(name.IndexOf('.'));
            }
            return name;
        }

        /// <summary>
        /// 取文件创建时间
        /// </summary>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static DateTime GetFileCreateTime(string fullpath) {
            var fi = new FileInfo(fullpath);
            return fi.CreationTime;
        }

        /// <summary>
        /// 取文件最后存储时间
        /// </summary>
        /// <param name="fullpath"></param>
        /// <returns></returns>
        public static DateTime GetLastWriteTime(string fullpath) {
            var fi = new FileInfo(fullpath);
            return fi.LastWriteTime;
        }

        /// <summary>
        /// 创建一个零字节临时文件
        /// </summary>
        /// <returns></returns>
        public static string CreateTempZeroByteFile() {
            return Path.GetTempFileName();
        }

        /// <summary>
        /// 创建一个随机文件名，不创建文件本身
        /// </summary>
        /// <returns></returns>
        public static string GetRandomFileName() {
            return Path.GetRandomFileName();
        }

        /// <summary>
        /// 判断两个文件的哈希值是否一致
        /// </summary>
        /// <param name="fileName1"></param>
        /// <param name="fileName2"></param>
        /// <returns></returns>
        public static bool CompareFilesHash(string fileName1, string fileName2) {
            using (var hashAlg = HashAlgorithm.Create()) {
                using (FileStream fs1 = new FileStream(fileName1, FileMode.Open), fs2 = new FileStream(fileName2, FileMode.Open)) {
                    byte[] hashBytes1 = hashAlg.ComputeHash(fs1);
                    byte[] hashBytes2 = hashAlg.ComputeHash(fs2);


                    return (BitConverter.ToString(hashBytes1) == BitConverter.ToString(hashBytes2));
                }
            }
        }

        /// <summary>
        /// 从XML文件转换为Object对象类型.
        /// </summary>
        /// <param name="path">XML文件路径</param>
        /// <param name="type">Object对象类型</param>
        /// <returns></returns>
        public static object LoadObjectFromXml(string path, Type type) {
            object obj = null;
            using (var reader = new StreamReader(path)) {
                var content = reader.ReadToEnd();
                obj = ZaoXml.XmlToObject(content, type);
            }
            return obj;
        }

        /// <summary>
        /// 保存对象到特定格式的XML文件
        /// </summary>
        /// <param name="path">XML文件路径.</param>
        /// <param name="obj">待保存的对象</param>
        public static void SaveObjectToXml(string path, object obj) {
            var xml = ZaoXml.ObjectToXml(obj, true);
            using (var writer = new StreamWriter(path)) {
                writer.Write(xml);
            }
        }
    }
}
