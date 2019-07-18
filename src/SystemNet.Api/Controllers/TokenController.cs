using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using SystemNet.Api.Models.Token;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Security.Bearer;
using SystemNet.Shared;

namespace SystemNet.Api.Products.Controllers
{
    [AllowAnonymous]
    public class TokenController : Controller
    {

        private const int expiredminutes = 240;
        IIrmaoServices _service;

        public TokenController(IIrmaoServices service)
        {
            _service = service;
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
               Irmao ret = _service.Autenticar(inputModel.Username, inputModel.Password);

                //TODO LOGIN
                if (ret.Notifications.Any())
                    return Unauthorized(
                        );

                var member = (ret.AcessoAdmin) ? "MembershipId" : "Brother";

                var token = new JwtTokenBuilder()
                                    .AddSecurityKey(JwtSecurityKey.Create(Runtime.SecreteKey))
                                    .AddSubject(ret.Nome)
                                    .AddIssuer("SystemNet.Security")
                                    .AddAudience("SystemNet.Security")                                   
                                    .AddClaim(member,ret.Codigo.ToString())
                                    .AddClaim((member != "Brother") ? "Brother" : "Patner", ret.Codigo.ToString())
                                    .AddClaim("UserName", ret.Email)
                                    .AddExpiry(expiredminutes)
                                    .Build();
                return Ok(new
                {
                    access_token = token.Value,
                    changePassword = ret.AlterarSenha,
                    userid = ret.Codigo,
                    username = ret.Nome,
                    congregation = ret.CongregacaoNome,
                    congregationId = ret.CongregacaoId,
                    admin = ret.AcessoAdmin,
                    email = ret.Email
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