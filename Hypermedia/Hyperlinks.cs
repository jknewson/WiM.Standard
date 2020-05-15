using WIM.Resources;

namespace WIM.Hypermedia
{
    public class Hyperlinks
    {
        public static Link Generate(string rel, string href) {

            return new Resources.Link() {
                rel = rel,
                Href = href,
                method = string.Empty
            };

        }

        public static Link Generate(string baseUri, string rel, string href, refType httpMethod)
        {
            //insure base uri has /
            if (!baseUri[baseUri.Length - 1].Equals('/'))
                baseUri = baseUri.Insert(baseUri.Length, "/");
            if (href[0].Equals('/'))
                href = href.Substring(1);

            return new Resources.Link()
            {
                rel = rel,
                Href = baseUri + href,
                method = httpMethod.ToString()
            };
        }// end link

    }//end class Link
}
