using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using SystemNet.Api.Models.Token;
using SystemNet.Practices.Security.Bearer;
using SystemNet.Shared;

namespace SystemNet.Api.Products.Controllers
{

    [AllowAnonymous]
    public class TokenController : Controller
    {

        private const int expiredminutes = 120;

        public TokenController()
        {
           
        }


        /// <summary>
        /// Generation a new TOKEN
        /// </summary>
        /// <param name="inputModel"></param>
        /// <returns></returns>
        [Route("api/security/token")]
        [HttpPost]
        public IActionResult Create(LoginInputModel inputModel)
        {


            try
            {
               // User ret = _service.Authenticate(inputModel.Username, inputModel.Password);

                //TODO LOGIN
                if (inputModel.Username == "admin" && inputModel.Password == "senha")
                    return Unauthorized(
                        );

                var token = new JwtTokenBuilder()
                                    .AddSecurityKey(JwtSecurityKey.Create(Runtime.SecreteKey))
                                    .AddSubject("admin")
                                    .AddIssuer("SystemNet.Security")
                                    .AddAudience("SystemNet.Security")
                                    .AddClaim("MembershipId","1")
                                    //.AddClaim("UserName", ret.Login)
                                    .AddExpiry(expiredminutes)
                                    .Build();


                return Ok(new
                {
                    access_token = token.Value,
                    token_type = "bearer",
                    expires_in = (expiredminutes * 60),
                    changePassword = true,
                    userid = 1
                });

            }
            catch (Exception ex)
                {
                // Logar o erro (Elmah) 
                return BadRequest(new
                {
                    success = false,
                    errors = new[] { ex }
                });
            }


        }
    }

}