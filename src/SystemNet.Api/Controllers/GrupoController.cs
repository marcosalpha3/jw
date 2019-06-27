using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemNet.Core.Domain.Contracts.Services;

namespace SystemNet.Api.Controllers
{
    public class GrupoController : BaseController
    {
        IGrupoServices _service;
        public GrupoController(IGrupoServices service)
        {
            _service = service;
        }

        /// <summary>
        /// Obter Grupos por congregação
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/grupo/congregacao/{id}")]
        [Authorize(Policy = "Member")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = _service.ObterGruposPorCongregacao(id);
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
