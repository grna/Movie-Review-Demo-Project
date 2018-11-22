using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SciFiReviewsApi.Models;
using SciFiReviewsApi.Models.DtoModels;
using SciFiReviewsApi.Models.EntityModels;
using SciFiReviewsApi.Models.ReturnModels;

namespace SciFiReviewsApi.Repository
{
    public class SqlReviewerData : IReviewerData
    {
        private SciFiReviewsDbContext _dbContext;

        public SqlReviewerData(SciFiReviewsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Reviewer>> GetReviewers()
        {
            return await _dbContext.Reviewers
                .ToListAsync();
        }

        public async Task<Reviewer> GetReviewer(string username, int? id)
        {
            if (id != null)
            {
                return await _dbContext.Reviewers
                    .FindAsync(id);
            }

            return await _dbContext.Reviewers
                    .FirstOrDefaultAsync(r => r.Username == username);
        }

        public async Task<Reviewer> CreateReviewer(int userId)
        {
            Reviewer reviewer = new Reviewer
            {
                //UserId = userId
            };

            await _dbContext.Reviewers.AddAsync(reviewer);
            await _dbContext.SaveChangesAsync();

            return reviewer;
        }

        public async Task<bool> DeleteReviewer(int id)
        {
            Reviewer reviewer = await _dbContext.Reviewers.FindAsync(id);

            if (reviewer == null)
                return false;

            _dbContext.Reviewers.Remove(reviewer);
            await _dbContext.SaveChangesAsync();

            return true;
        }
 
    }
}
