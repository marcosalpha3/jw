using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemNet.Api.Models.Grupo;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Models;

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

        /// <summary>
        /// Gera um novo grupo de campo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/grupo")]
        [Authorize(Policy = "Member")]
        public async Task<IActionResult> Post([FromBody]NovoGrupo model)
        {
            try
            {
                var result = _service.Adicionar(new Grupo(int.MinValue, model.Nome, model.DirigenteId, model.CongregacaoId, model.Local));
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
        /// Apagar um grupo e campo e transferir os irmãos para outro grupo
        /// </summary>
        /// <param name="id"></param>
        /// <param name="novogrupo"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/v1/grupo/{id}/novogrupo/{novogrupo}")]
        [Authorize(Policy = "Member")]
        public async Task<IActionResult> Apagar(int id, int novogrupo)
        {
            try
            {
                var result = _service.Apagar(id, novogrupo);
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
        /// Altera um grupo de campo existente
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/v1/grupo")]
        [Authorize(Policy = "Member")]
        public async Task<IActionResult> Put([FromBody]AtualizaGrupo model)
        {
            try
            {
                var result = _service.Atualizar(new Grupo(model.Codigo, model.Nome, model.DirigenteId, 1, model.Local));
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
