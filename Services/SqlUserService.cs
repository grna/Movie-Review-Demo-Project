using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SciFiReviewsApi.Models;
using SciFiReviewsApi.Models.DtoModels;
using SciFiReviewsApi.Models.EntityModels;

namespace SciFiReviewsApi.Services
{
    public class SqlUserService : IUserService
    {
        private SciFiReviewsDbContext _dbContext;

        public SqlUserService(SciFiReviewsDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddUser(SignUpModel signUpModel)
        {
            User user = _dbContext.Users.FirstOrDefault(u => u.Username == signUpModel.Username);

            if (user == null)
            {
                var reviewer = new Reviewer
                {
                    Username = signUpModel.Username,
                    User = new User
                    {
                        FirstName = signUpModel.FirstName,
                        LastName = signUpModel.LastName,
                        MiddleName = signUpModel.MiddleName,
                        Username = signUpModel.Username,
                        EmailAddress = signUpModel.EmailAddress,
                        DateOfBirth = signUpModel.DateOfBirth,
                        PasswordHash = BCrypt.Net.BCrypt.HashPassword(signUpModel.Password)
                    }
                };

                _dbContext.Reviewers.Add(reviewer);
                await _dbContext.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public Task<bool> ValidateCredentials(SignInModel signInModel)
        {
            User user = _dbContext.Users.FirstOrDefault(u => u.Username == signInModel.Username);

            if(user != null)
            {
                if (BCrypt.Net.BCrypt.Verify(signInModel.Password, user.PasswordHash))
                    return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }
    }
}
