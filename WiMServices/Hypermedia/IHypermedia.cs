using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiM.Hypermedia
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
