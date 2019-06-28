using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Models;

namespace SystemNet.Api.Controllers
{
    public class QuadroController : BaseController
    {
        private readonly IQuadroServices _service;

        public QuadroController(IQuadroServices service)
        {
            _service = service;
        }

        /// <summary>
        /// Gera nova lista 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/quadro")]
        [AllowAnonymous]
        public async Task<IActionResult> Post()
        {
            try
            {
                var result = _service.GeraLista();
                return await Response(new Quadro { Codigo = 1 }, result);
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

        [HttpGet]
        [Route("api/v1/quadro/congregacao/{id}/atual")]
        [Authorize(Policy = "Brother")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = _service.ObterListaAtualDesignacoesMecanicas(id);
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

        [HttpGet]
        [Route("api/v1/quadro/congregacao/{id}/proxima")]
        [Authorize(Policy = "Brother")]
        public async Task<IActionResult> GetProxima(int id)
        {
            try
            {
                var result = _service.ObterProximaListaDesignacoesMecanicas(id);
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
