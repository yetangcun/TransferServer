using Ionic.Zip;
using System.IO;

namespace TransferUtility
{
    public class ZipUtility
    {
        #region ZIP
        /// <summary>
        /// 压缩文件夹 都是绝对路径
        /// </summary>
        /// <param name="directory">要压缩的文件夹</param>
        /// <param name="zipName">压缩后的压缩包文件名</param>
        /// <returns></returns>
        public static string Comparess2ZIP(string directory, string zipName)
        {
            ZipFile zf = new ZipFile();
            zf.AddDirectory(directory);
            string zipFullName = string.Format("{0}\\{1}.zip", directory, zipName);
            zf.Save(zipFullName);
            if (File.Exists(zipFullName))
            {
                return zipFullName;
            }
            return null;
        }
        #endregion
    }
}
