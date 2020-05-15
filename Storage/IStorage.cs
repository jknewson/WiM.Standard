using System;
using System.IO;
using System.Threading.Tasks;

namespace WIM.Storage
{
    public interface IStorage
    {
        string ParentDirectory { get; set; }

        Task<Stream> GetObject(String ObjectName);
        Task AddObject(String ObjectName, Stream aStream);
        Task<Boolean> DeleteObject(String ObjectName);

    }
}
