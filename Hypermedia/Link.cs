namespace WIM.Hypermedia
{
    public class Link:WIM.Resources.Link
    {

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
        public Link(string baseUri, string rel, string href, WIM.Resources.refType httpMethod)
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
}
