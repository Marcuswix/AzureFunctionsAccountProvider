using AccountProvider.Models;
using Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text;

namespace AccountProvider.Functions
{
    public class Verify
    {
        private readonly ILogger<Verify> _logger;
        private readonly UserManager<UserAccount> _userManager;

        public Verify(ILogger<Verify> logger, UserManager<UserAccount> userManager)
        {
            _logger = logger;
            _userManager = userManager;
        }

        [Function("Verify")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            string body = null!;

            try 
            {
                body = await new StreamReader(req.Body).ReadToEndAsync();
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
            }

            if(body != null)
            {
                VerificationRequest vr = null!;

                try
                {
                    vr = JsonConvert.DeserializeObject<VerificationRequest>(body)!;
                }
                catch (Exception ex)
                {
                    _logger.LogError("VerificationRequest" + ex.Message);
                }

                if(vr != null && !string.IsNullOrEmpty(vr.VerificationCode) && !string.IsNullOrEmpty(vr.Email))
                {
                    var isVerified = true;

                    try
                    {
                        using var http = new HttpClient();
                        StringContent content = new StringContent(JsonConvert.SerializeObject(vr), Encoding.UTF8, "application/json");
                        //var response = await http.PostAsync("https//verificationprovider....", content);
                    }
                    catch (Exception ex)
                    {

                    }



                    if (isVerified)
                    {
                        var userAccount = await _userManager.FindByEmailAsync(vr.Email);
                        if(userAccount != null)
                        {
                            userAccount.EmailConfirmed = true;
                            await _userManager.UpdateAsync(userAccount);

                            if(await _userManager.IsEmailConfirmedAsync(userAccount))
                            return new OkResult();
                        }
                    }
                }
            }

            return new UnauthorizedResult();
        }
    }
}
