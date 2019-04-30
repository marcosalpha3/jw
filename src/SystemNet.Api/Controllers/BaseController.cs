using FluentValidator;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using SystemNet.Api.Enums;

namespace SystemNet.Api.Controllers
{
    /// <summary>
    /// Controller Base 
    /// </summary>
    public class BaseController : Controller
    {

        /// <summary>
        /// Return the result of API
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public async Task<IActionResult> Response(object result)
        {
                try
                {
                    return Ok(result
                    );
                }
                catch
                {
                    // Logar o erro (Elmah) 
                    return BadRequest(new
                    {
                        success = false,
                        errors = new[] { "Ocorreu uma falha interna no servidor." }
                    });
                }
        }

        public async Task<IActionResult> Response(object result, IEnumerable<Notification> notifications)
        {
            if (!notifications.Any())
            {
                try
                {
                    return Ok(new
                    {
                        success = true,
                        data = result
                    });
                }
                catch
                {
                    // Logar o erro (Elmah)
                    return BadRequest(new
                    {
                        success = false,
                        errors = new[] { "Ocorreu uma falha interna no servidor." }
                    });
                }
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    errors = notifications
                });
            }
        }

        public int GetUserToken()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            return Convert.ToInt32(identity.Claims.ElementAt((int)EClaims.UserId).Value);           
        }

        public string GetLoginToken()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            return identity.Claims.ElementAt((int)EClaims.Login).Value;
        }

        public int GetProfileToken()
        {
            var identity = (ClaimsIdentity)User.Identity;
            IEnumerable<Claim> claims = HttpContext.User.Claims;
            return Convert.ToInt32(identity.Claims.ElementAt((int)EClaims.Profile).Value);
        }

        public EnLanguage GetLanguageID()
        {
            string language = System.Globalization.CultureInfo.CurrentCulture.Name;
            EnLanguage idLanguage;
            switch (language)
            {
                case "pt-BR":
                    idLanguage = EnLanguage.Portuguese;
                    break;
                case "es-ES":
                    idLanguage = EnLanguage.Spanish;
                    break;
                default:
                    idLanguage = EnLanguage.English;
                    break;
            }
            return idLanguage;
        }


        public enum EnLanguage
        {
            Portuguese = 1,
            Spanish = 2,
            English = 3

        }


    }
}
