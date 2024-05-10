using AccountProvider.Models;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace AccountProvider.Functions
{
    public class SignUp
    {
        private readonly ILogger<SignUp> _logger;
        private readonly UserManager<UserAccount> _userManager;

        public SignUp(ILogger<SignUp> logger, UserManager<UserAccount> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [Function("SignUp")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            string body = null!;
            try 
            {
                body = await new StreamReader(req.Body).ReadToEndAsync();
            }
            catch (Exception ex) 
            {
                _logger.LogError($"StreamReader :: {ex.Message}");
            }

            if (body != null)
            {
                UserRegistrationRequest userRegistrationRequest = null!;

                try
                {
                    userRegistrationRequest = JsonConvert.DeserializeObject<UserRegistrationRequest>(body)!;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"JsonConvert.DeserializeObject<UserRegistrationRequest>(body) :: {ex.Message}");
                }

                if (userRegistrationRequest != null && !string.IsNullOrEmpty(userRegistrationRequest.Password))
                {
                    if(! await _userManager.Users.AnyAsync(x => x.Email == userRegistrationRequest.Email))
                    {
                        var userAccount = new UserAccount
                        {
                            FirstName = userRegistrationRequest.FirstName,
                            LastName = userRegistrationRequest.LastName,
                            Email = userRegistrationRequest.Email,
                            UserName = userRegistrationRequest.Email,
                        };

                        var result = await _userManager.CreateAsync(userAccount);
                        if(result.Succeeded)
                        {
                            using var http = new HttpClient();
                            StringContent content = new StringContent(JsonConvert.SerializeObject(new {Email = userAccount.Email}), Encoding.UTF8, "application/json");
                            //var response = await http.PostAsync("https//verificationprovider....", content);

                            return new OkResult();
                        }
                    }
                    else
                    {
                        return new ConflictResult();
                    }
                }

            }

            return new BadRequestResult();
        }
    }
}
