using SciFiReviewsApi.Models.DtoModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SciFiReviewsApi.Services
{
    public interface IUserService
    {
        Task<bool> ValidateCredentials(SignInModel signInModel);

        Task<bool> AddUser(SignUpModel signUpModel);
    }
}
