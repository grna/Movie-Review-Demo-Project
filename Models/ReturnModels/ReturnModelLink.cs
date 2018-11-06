namespace SciFiReviewsApi.Models.ReturnModels
{
    public class ReturnModelLink
    {
        public string Href { get; private set; }

        public string Rel { get; private set; }

        public string Method { get; private set; }

        public ReturnModelLink(string href, string rel, string method)
        {
            Href = href;
            Rel = rel;
            Method = method;
        }
    }
}
