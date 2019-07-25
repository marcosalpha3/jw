using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Models;

namespace SystemNet.Api.Controllers
{
    public class AssistenciaController : BaseController
    {
        IAssistenciaReuniaoServices _service;

        public AssistenciaController(IAssistenciaReuniaoServices service)
        {
            _service = service;
        }

        /// <summary>
        /// Obter assistencias às reuniões por congregação e datas
        /// </summary>
        /// <param name="congregacaoId"></param>
        /// <param name="dataInicial"></param>
        /// <param name="dataFinal"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/assistencia/congregacao/{congregacaoid}/datainicial/{datainicial}/datafinal/{datafinal}")]
        [Authorize(Policy = "Indicator")]
        public async Task<IActionResult> Get(int congregacaoId, DateTime dataInicial, DateTime dataFinal)
        {
            try
            {
                var result = _service.ObterAssistenciasPorCongregacao(congregacaoId, dataInicial, dataFinal);
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
        /// Apagar uma assistência
        /// </summary>
        /// <param name="congregacaoId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/v1/assistencia/congregacao/{congregacaoid}/data/{data}")]
        [Authorize(Policy = "Brother")]
        public async Task<IActionResult> Apagar(int congregacaoId, DateTime data)
        {
            try
            {
                var result = _service.Apagar(congregacaoId, data);
                return await Response(result, result);
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
        /// Atualiza a assistencia de uma reunião
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/assistencia")]
        [Authorize(Policy = "Indicator")]
        public async Task<IActionResult> Post([FromBody]AssistenciaReuniao model)
        {
            try
            {
                var result = _service.Atualizar(new AssistenciaReuniao(model.Data, model.CongregacaoId, model.AssistenciaParte1, model.AssistenciaParte2));
                return await Response(model, result);
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
