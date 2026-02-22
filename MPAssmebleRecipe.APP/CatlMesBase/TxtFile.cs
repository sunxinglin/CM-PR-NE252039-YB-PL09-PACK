using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CatlMesBase
{
    public class TxtFile
    {
        //不能定义成static
        private static ReaderWriterLockSlim readerWriterLock = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);
        private static string rootPath;
        private static string folder;
        public TxtFile(string root)
        {
            TxtFile.rootPath = root;
        }

        /// <summary>
        /// 判断文件是否存在,若不存在则创建
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private void ExistFile(string folder)
        {
            try
            {
              
                string filePath = Path.Combine(rootPath, folder);
                if (!File.Exists(filePath))
                {
                 FileStream fs=   File.Create(filePath);
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void DeleteFile(string path, int days)
        {
            try
            {
                if (!string.IsNullOrEmpty(path))
                {
                    string[] files = Directory.GetFiles(path);
                    string[] dictionarys = Directory.GetDirectories(path);
                    if (files != null && files.Length > 0)
                    {
                        for (int i = 0; i < files.Length; i++)
                        {
                            if (File.GetCreationTime(files[i]).AddDays(days) < DateTime.Now)
                            {
                                File.Delete(files[i]);
                            }
                        }
                    }
                    if (dictionarys != null && dictionarys.Length > 0)
                    {
                        for (int i = 0; i < dictionarys.Length; i++)
                        {
                            DeleteFile(dictionarys[i], days);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// 写入文本文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        public void WriteFile(string folder, string content)
        {
            object locker = new object();
            lock (locker)
            {
            string fileName = DateTime.Now.ToString("yyyy_MM_dd");
            string fullName = Path.Combine(rootPath, folder, folder + fileName+".csv");
            string path = Path.Combine(rootPath, folder);
            try
            {
                readerWriterLock.TryEnterWriteLock(2000);
                using (FileStream fs = new FileStream(fullName, FileMode.Append, FileAccess.Write,FileShare.ReadWrite))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.Default))
                    {
                        sw.Write(content+"\r\n");
                        sw.Close();
                        sw.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                readerWriterLock.ExitWriteLock();
                }
            }
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public string ReadFile(string filePath)
        {
            try
            {
                //readerWriterLock.TryEnterReadLock(1000);
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read,FileShare.ReadWrite))
                {
                    using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return string.Empty;
            }
            finally
            {
                //readerWriterLock.EnterReadLock();
            }
        }

        /// <summary>
        /// 检查文件目录是否存在,不存在则创建
        /// </summary>
        /// <param name="path"></param>
        public static void ExistsPath(string path)
        {
            TxtFile.folder = path;
            string filePath = Path.Combine(TxtFile.rootPath, folder);
            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }
        }
    }
}
