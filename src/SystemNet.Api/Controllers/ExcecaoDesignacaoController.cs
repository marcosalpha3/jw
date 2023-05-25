using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System;
using System.Linq;
using System.Threading.Tasks;
using SystemNet.Api.Models.Quadro;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Models;

namespace SystemNet.Api.Controllers
{
    public class ExcecaoDesignacaoController : BaseController
    {
        readonly IExcecaoDesignacaoServices _service;

        public ExcecaoDesignacaoController(IExcecaoDesignacaoServices service)
        {
            _service = service;
        }

        /// <summary>
        /// Obter Exceções por congregacao
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/excecaodesignacao/congregacao/{id}")]
        [Authorize(Policy = "Member")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = _service.ObterExcecaoPorCongregacao(id);
                return (result == null || !result.Any()) ? NoContent() : await Response(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = new[] { ex }
                });
            }
        }

        /// <summary>
        /// Gera uma nova exceção de designação para um irmão em uma determinada data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/excecaodesignacao")]
        [Authorize(Policy = "Member")]
        public async Task<IActionResult> Post([FromBody]NovaExcecao model)
        {
            try
            {
                var result = _service.Inserir(new ExcecaoDesignacao(int.MinValue, model.Data, model.IrmaoId, model.Motivo));
                return await Response(result, result.Notifications);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = new[] { ex }
                });
            }
        }

        /// <summary>
        /// Apaga uma excecao
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/v1/excecaodesignacao/{id}")]
        [Authorize(Policy = "Member")]
        public async Task<IActionResult> Apagar(int id)
        {
            try
            {
                var result = _service.Apagar(id);
                return await Response(result, result.Notifications);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = new[] { ex }
                });
            }
        }

        /// <summary>
        /// Adiciona uma exceção até determinada data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/excecaodesignacao/data")]
        [Authorize(Policy = "Member")]
        public async Task<IActionResult> AdicionarAteUmaData([FromBody] ExcecaoAteData model)
        {
            try
            {
                _service.InserirExcecaoAteData(model.IrmaoId, model.Dia, model.AteData);
                return await Response(new ExcecaoDesignacao(model.IrmaoId, model.AteData, model.IrmaoId, String.Empty ));
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    errors = new[] { ex }
                });
            }
        }
    }
}
