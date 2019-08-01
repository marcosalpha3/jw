using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using SystemNet.Api.Models.Quadro;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Models;

namespace SystemNet.Api.Controllers
{
    public class EventoController : BaseController
    {
        IDataEventoServices _service;

        public EventoController(IDataEventoServices service)
        {
            _service = service;
        }

        /// <summary>
        /// Obter Eventos por congregação
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/evento/congregacao/{id}")]
        [Authorize(Policy = "Member")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var result = _service.ObterEventosPorCongregacao(id);
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
        /// Gera um novo evento
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/evento")]
        [Authorize(Policy = "Member")]
        public async Task<IActionResult> Post([FromBody]NovoEvento model)
        {
            try
            {
                var result = _service.Adicionar(new DataEvento(int.MinValue, model.Data, model.Descricao, model.CongregacaoId, model.TipoEvento));
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
        /// Apagar um evento
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/v1/evento/{id}")]
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
    }
}
