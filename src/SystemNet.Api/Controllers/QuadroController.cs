using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SystemNet.Api.Models.Quadro;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Models;
using SystemNet.Practices.Data.Storage.Models;

namespace SystemNet.Api.Controllers
{
    public class QuadroController : BaseController
    {
        private readonly IQuadroServices _service;
        private readonly StorageConfig _storageConfig = null;

        public QuadroController(IQuadroServices service, 
            IOptions<StorageConfig> config)
        {
            _service = service;
            _storageConfig = config.Value;
        }

        /// <summary>
        /// Gera nova lista 
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/quadro")]
        [Authorize(Policy = "Member")]
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

        [HttpPost]
        [Route("api/v1/quadro/regerar/congregacao/{id}")]
        [Authorize(Policy = "Member")]
        public async Task<IActionResult> RegerarQuadro(int id)
        {
            try
            {
                var result = _service.RegerarListaAtual(id);
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

        [HttpPost]
        [Route("api/v1/quadro/congregacao/{id}/expirationdate/{expirationdate}/initialdate/{initialDate}/titulo/{titulo}")]
        [Authorize(Policy = "Member")]
        public async Task<IActionResult> NovoQuadroPesonalizado(int id, string expirationdate, string initialDate, string titulo, IFormFile file)
        {
            try
            {
                var result = await _service.NovoQuadroPersonalizado(new QuadroPersonalizado("", titulo, id, Convert.ToDateTime(expirationdate), 
                    Convert.ToDateTime(initialDate), true), _storageConfig, file);
                return await Response(result, result.Notifications);
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
        [Route("api/v1/quadro/expurgo")]
        [Authorize(Policy = "Member")]
        public async Task<IActionResult> Expurgo()
        {
            try
            {
                var result = await _service.ApagarStorageNaoUtilizado(_storageConfig);
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
        [Route("api/v1/quadro/personalizado/congregacao/{congregacaoId}")]
        [Authorize(Policy = "Brother")]
        public async Task<IActionResult> GetQuadroPersonalizado(int congregacaoId)
        {
            try
            {
                var result = _service.ObterQuadrosPersonalizados(congregacaoId);
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

        [HttpPost]
        [Route("api/v1/quadro/personalizado")]
        [Authorize(Policy = "Member")]
        public async Task<IActionResult> ApagarPersonalizado([FromBody]ApagarQuadroPersonalizado model)
        {
            try
            {
                var result = await _service.ApagarQuadroPersonalizado(model.CongregacaoId, model.Url, _storageConfig);
                return await Response(result, result.Notifications);
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
        [Route("api/v1/quadro/congregacao/{id}")]
        [Authorize(Policy = "Brother")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = _service.ObterListaDesignacoesMecanicas(id);
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
