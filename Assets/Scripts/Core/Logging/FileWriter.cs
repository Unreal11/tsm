using System;
using System.Globalization;
using System.IO;
using System.Text;

namespace Core.Logging
{
    public class FileWriter
    {
        private readonly string _folder;
        private string _filePath;
        public FileWriter(string folder)
        {
            _folder = folder;
            ManagePath();
        }

        private void ManagePath()
        {
            _filePath = $"{_folder}/{DateTime.UtcNow.ToString(CultureInfo.CurrentCulture)}.log";
        }

        public void Write(string message)
        {
            using (FileStream fs = File.Open(_filePath, FileMode.Append, FileAccess.Write, FileShare.Read))
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                
                fs.Write(bytes, offset:0, bytes.Length);
            }
        }
    }
}