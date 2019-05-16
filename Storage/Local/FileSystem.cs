using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace WIM.Storage.Local
{
    public class FileSystem : IStorage
    {
        #region Properties
        public string ParentDirectory { get; set; }
        #endregion
        #region Constructor
        public FileSystem(string parentDirectory)
        {
            ParentDirectory = parentDirectory;
        }
        #endregion
        #region Methods
        public Task AddObject(string ObjectName, Stream aStream)
        {
            throw new NotImplementedException();
            //try
            //{
            //    string directory = Path.Combine(ParentDirectory, Path.GetDirectoryName(ObjectName));

            //    if (!Directory.Exists(Path.Combine(directory)))
            //        Directory.CreateDirectory(directory);

            //    using (var fileStream = File.Create(Path.Combine(ParentDirectory, ObjectName)))
            //    {
            //        //reset stream position to 0 prior to copying to filestream;
            //        aStream.Position = 0;
            //        aStream.CopyTo(fileStream);
            //    }//end using

            //}
            //catch (Exception)
            //{
            //    
            //}
        }
        public Task<bool> DeleteObject(string ObjectName)
        {
            throw new NotImplementedException();
        }
        public Task<Stream> GetObject(string ObjectName)
        {
            throw new NotImplementedException();
            //string objfile = Path.Combine(ParentDirectory, ObjectName);
            //try
            //{
            //    return File.OpenRead(objfile);
            //}
            //catch (Exception)
            //{
            //    return null;
            //}
        }
        #endregion
        #region "Helper Methods"
        private Boolean isDirectory(string filelocation)
        {
            try
            {
                FileAttributes attr = File.GetAttributes(filelocation);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    return true;

                return false;
            }
            catch (Exception)
            {
                return false;
            }//end try
        }
        #endregion
    }
}
