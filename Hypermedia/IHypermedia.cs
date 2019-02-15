using System.Collections.Generic;

namespace WIM.Hypermedia
{
    public interface IHypermedia
    {
        #region Properties
        List<Link> Links { get; set; }
        #endregion
    }
    public enum HypermediaType
    {
        e_self = 0,
        e_collection = 1
    }
}
