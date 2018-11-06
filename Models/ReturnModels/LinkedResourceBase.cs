using System.Collections.Generic;

namespace SciFiReviewsApi.Models.ReturnModels
{
    public abstract class LinkedResourceBase
    {
        public List<ReturnModelLink> Links { get; set; } =
            new List<ReturnModelLink>();
    }
}
