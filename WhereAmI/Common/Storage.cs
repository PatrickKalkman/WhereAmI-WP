using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace WhereAmI.Common
{
    public class Storage
    {
        public string Read(string fileName)
        {
            IsolatedStorageFile appIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
            string contents = string.Empty;
            if (appIsolatedStorage.FileExists(fileName))
            {
                IsolatedStorageFileStream fileStream = appIsolatedStorage.OpenFile(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                using (var reader = new StreamReader(fileStream))
                {
                    contents = reader.ReadToEnd();
                }
            }
            return contents;
        }

        public void Write(string fileName, string contents)
        {
            IsolatedStorageFile appIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
            IsolatedStorageFileStream fileStream = appIsolatedStorage.CreateFile(fileName);
            using (var writer = new StreamWriter(fileStream))
            {
                writer.Write(contents);
            }
        }

        public void SaveFileToIsolatedStorage(Uri fileUri, string fileName)
        {
            IsolatedStorageFile appIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication();
            var FileName = appIsolatedStorage.CreateFile(fileName);
            var FileData = Application.GetResourceStream(fileUri);
            byte[] bytes = new byte[4096];
            int Count;
            while ((Count = FileData.Stream.Read(bytes, 0, 4096)) > 0)
            {
                FileName.Write(bytes, 0, Count);
            }
            FileName.Close();
        }
    }
}
