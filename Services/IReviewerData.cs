using SciFiReviewsApi.Models.EntityModels;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SciFiReviewsApi.Services
{
    public interface IReviewerData
    {
        Task<List<Reviewer>> GetReviewers();

        Task<Reviewer> GetReviewer(string username, int? id);

        Task<Reviewer> CreateReviewer(int userId);

        Task<bool> DeleteReviewer(int id);
    }
}
