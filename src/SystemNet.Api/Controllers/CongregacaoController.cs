using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using SystemNet.Core.Domain.Contracts.Services;

namespace SystemNet.Api.Controllers
{
    public class CongregacaoController : BaseController
    {
        ICongregacaoServices _service;

        public CongregacaoController(ICongregacaoServices service)
        {
            _service = service;
        }

        /// <summary>
        /// Obter lista de congregações
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/congregacao")]
        [Authorize(Policy = "Member")]
        public async Task<IActionResult> Get()
        {
            try
            {
                var result = _service.ObterTodos();
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
