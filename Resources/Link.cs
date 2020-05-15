namespace WIM.Resources
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

    }//end class Link

    public enum refType
    {
        GET,
        PUT,
        POST,
        DELETE
    }
}
