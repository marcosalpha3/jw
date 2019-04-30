using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using SystemNet.Core.Domain.Contracts.Services;

namespace SystemNet.Api.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class IrmaoController : BaseController
    {
        private readonly IIrmaoServices _service;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="service"></param>
        public IrmaoController(IIrmaoServices service)
        {
            _service = service;
        }

        /// <summary>
        /// Obter Irmaos por congregacao
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/irmao/congregacao/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = _service.ObterIrmaosPorCongregacao(id);
                return (result == null || !result.Any()) ? NoContent() : await Response(result);
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
