//------------------------------------------------------------------------------
//----- Link -------------------------------------------------------------------
//------------------------------------------------------------------------------

//-------1---------2---------3---------4---------5---------6---------7---------8
//       01234567890123456789012345678901234567890123456789012345678901234567890
//-------+---------+---------+---------+---------+---------+---------+---------+

// copyright:   2012 WiM - USGS

//    authors:  Jeremy K. Newson USGS Wisconsin Internet Mapping
//              
//  
//   purpose:   A generic link resource for implementing hypermedia. Each link element
//              points to a related resource.
//
//discussion:   Benefits of hypermedia are that it allows for the server to change
//              it's URI scheme without breaking clients, It helps developers explore the protocols, and
//              it allows the server team to advertise new capabilities 
//              http://martinfowler.com/articles/richardsonMaturityModel.html
//
//              Hypermedia provides a means of navigation from one resource to another.
//
//     
#region Comments
// 06.06.12 - JKN - Created
// 10.08.14 - JKN - Moved to WiMServices and simplified
#endregion

namespace WiM.Hypermedia
{
    public class Link
    {
        #region Properties

        /// <summary>
        /// conveys the semantics of the link, 
        /// such as what resource does the URI refer to,
        /// what is the significance of the link
        /// what kind of action can a client perform on the resource at the URI, and
        /// what are the supported representation formats for requests and responses for that resource
        /// </summary>
        public string rel { get; set; }

        /// <summary>
        /// Absolute URI, or location of the resource
        /// </summary>
        public string Href { get; set; }

        /// <summary>
        /// Method
        /// </summary>
        public string method { get; set; }

        #endregion

        #region Constructors

        public Link() { }

        /// <summary>
        /// overloaded construcor   
        /// </summary>
        /// <param name="rel">indicates the type of the link</param>
        /// <param name="href">the link's URI</param>
        public Link(string rel, string href)
        {
            this.rel = rel;
            this.Href = href;
            this.method = string.Empty;


        }//end link
        public Link(string baseUri, string rel, string href, refType httpMethod)
        {
            //insure base uri has /
            if (!baseUri[baseUri.Length - 1].Equals('/'))
                baseUri = baseUri.Insert(baseUri.Length, "/");

            this.rel = rel;
            this.Href = baseUri + href;
            this.method = httpMethod.ToString();
        }// end link

        #endregion

    }//end class Link
   
    public enum refType
        {
            GET,
            PUT,
            POST,
            DELETE
        }    
}//end namespace