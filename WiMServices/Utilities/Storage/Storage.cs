//------------------------------------------------------------------------------
//----- Storage ----------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2014 WiM - USGS

//    authors:  Jeremy K. Newson USGS Wisconsin Internet Mapping
//              
//  
//   purpose:   Provide basic storage/packaging functionality.
//           
//discussion: 
//

#region "Comments"
//03.26.2014 jkn - Created
#endregion

#region "Imports"
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Web;
using Ionic.Zip;
#endregion
namespace WiM.Utilities.Storage
{
    public class Storage
    {
        #region "Properties"
        public string ParentDirectory { get; set; }
        #endregion
        #region "Constructor and IDisposable Support"
        public Storage(string parentDirectory)
        {
            ParentDirectory = parentDirectory;
        }//end Storage
        #region "IDisposable Support"
        #endregion
        #endregion
        #region "Methods"
        public Stream GetZipFile (string ObjectName)
        {
            string FileLocation = string.Empty;
            MemoryStream ms = null;
            try
            {
                FileLocation = Path.Combine(ParentDirectory, ObjectName);
                if (!isDirectory(FileLocation)) throw new Exception("Does not exist");
                using (ZipFile zip = new ZipFile())
                {
                    //Note: if empty folder exists, folder will not be included.
                    zip.AddSelectedFiles("*",FileLocation,ObjectName,false);
                    zip.Comment = "Downloaded: " + System.DateTime.Now.ToString("G");
                    ms = new MemoryStream();
                    zip.Save(ms);
                    ms.Seek(0,SeekOrigin.Begin);
                    ms.Flush();
                }//end using

                //Note: Below saves stream to parent directory.
                //byte[] data = ms.ToArray();
                //File.WriteAllBytes( Path.Combine(ParentDirectory, "data.zip"), data);
                return ms;
            }
            catch (Exception ex)
            {
                throw;
            }//end try
            
        }//end Package
        public void PutObject(String ObjectName, Stream aStream)
        {
            string directory = Path.Combine(ParentDirectory,Path.GetDirectoryName(ObjectName));
            try
            {
                if (!Directory.Exists(Path.Combine(directory)))
                    Directory.CreateDirectory(directory);

                using (var fileStream = File.Create(Path.Combine(ParentDirectory,ObjectName)))
                {
                    //reset stream position to 0 prior to copying to filestream;
                    aStream.Position = 0;
                    aStream.CopyTo(fileStream);
                }//end using

            }
            catch (Exception)
            {
                
            }
        }
                     

        //Download Object
        public Stream GetObject(String ObjectName)
        {
            string objfile = Path.Combine(ParentDirectory, ObjectName);
            try
            {
                return File.OpenRead(objfile);
            }
            catch (Exception)
            {
                return null;
            }

        }


        //Delete Object
        public Boolean DeleteObject(String ObjectName)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception)
            {
               return false;
            }
        }

        //List Objects in Bucket
     
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
    }//end class Storage
}//end namespace
