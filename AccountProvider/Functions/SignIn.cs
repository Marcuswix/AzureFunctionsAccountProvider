using AccountProvider.Models;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace AccountProvider.Functions
{
    public class SignIn
    {
        private readonly ILogger<SignIn> _logger;
        private readonly SignInManager<UserAccount> _signInManager;

        public SignIn(ILogger<SignIn> logger, SignInManager<UserAccount> signInManager)
        {
            _logger = logger;
            _signInManager = signInManager;
        }

        [Function("SignIn")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
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
                UserLogiInRequest ulr = null!;

                try
                {
                    ulr = JsonConvert.DeserializeObject<UserLogiInRequest>(body)!;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"StreamReader :: {ex.Message}");
                }

                if (ulr != null && !string.IsNullOrEmpty(ulr.Password))
                {
                    try
                    {
                        var result = await _signInManager.PasswordSignInAsync(ulr.Email, ulr.Password, ulr.IsPersistnet, false);
                        if(result.Succeeded)
                        {
                            //Get token from TokenProvider

                            return new OkObjectResult("accesstoken");
                        }

                        return new UnauthorizedResult();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"StreamReader :: {ex.Message}");
                    }
                }


            }
                return new BadRequestResult();
        }
    }
}
