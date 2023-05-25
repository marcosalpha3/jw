using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using SystemNet.Api.Models.Irmao;
using SystemNet.Core.Domain.Contracts.Services;
using SystemNet.Core.Domain.Models;

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
        [Authorize(Policy = "Member")]
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

        /// <summary>
        /// Obter Irmaos separados por Grupo de campo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/v1/irmao/grupos/congregacao/{id}")]
        [Authorize(Policy = "Brother")]
        public async Task<IActionResult> GetIrmaosporGrupo(int id)
        {
            try
            {
                var result = _service.ObterGruposComIrmaos(id);
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
        /// Gera um novo irmão / usuário e envia os dados com as credenciais
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/v1/irmao")]
        [Authorize(Policy = "Member")]
        //[AllowAnonymous]
        public async Task<IActionResult> Post([FromBody]NovoIrmao model)
        {
            try
            {
                var result = _service.Adicionar(new Irmao(int.MinValue, model.Nome, model.Email, model.Telefone, model.Sexo, model.Indicador, model.Microfonista, model.LeitorSentinela,
                    model.LeitorEstudoLivro, model.SistemaSonoro, model.OracaoFinal, model.PresidenteConferencia, model.Carrinho, model.GrupoId, model.CongregacaoId, model.AcessoAdmin,
                    model.AtualizarAssistencia, model.SubirQuadro, model.IndicadorAuditorio), 
                    GetUserToken());
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

        /// <summary>
        /// Desabilita um irmão da lista
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/v1/irmao/{id}/desativar")]
        [Authorize(Policy = "Member")]
        public async Task<IActionResult> Desabilitar(int id)
        {
            try
            {
                var result = _service.Desativar(id, GetUserToken());
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

        /// <summary>
        /// Reativar um irmão da lista
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/v1/irmao/{id}/reativar")]
        [Authorize(Policy = "Member")]
        public async Task<IActionResult> Reativar(int id)
        {
            try
            {
                var result = _service.Ativar(id, GetUserToken());
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
        [Route("api/v1/irmao/esquecersenha/{login}")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string login)
        {
            try
            {
                var result = _service.EsquecerSenha(login);
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
        /// Altera um irmão existente
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/v1/irmao")]
        [Authorize(Policy = "Member")]
        public async Task<IActionResult> Put([FromBody]AlteraIrmao model)
        {
            try
            {
                var result = _service.Atualizar(new Irmao(model.Codigo, model.Nome, model.Email, model.Telefone, model.Sexo, model.Indicador, model.Microfonista, model.LeitorSentinela,
                    model.LeitorEstudoLivro, model.SistemaSonoro, model.OracaoFinal, model.PresidenteConferencia, model.Carrinho, model.GrupoId, model.CongregacaoId, model.AcessoAdmin,
                    model.AtualizarAssistencia, model.SubirQuadro, model.IndicadorAuditorio)
                    ,GetUserToken());
                return await Response(model, result);
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
        /// Alterar Senha do Usuario
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/v1/irmao/alterarsenha")]
        [Authorize(Policy = "Brother")]
        public async Task<IActionResult> AlterarSenha([FromBody]AlterarSenhaIrmao model)
        {
            try
            {
                var result = _service.AlterarSenha(model.Email, GetLoginToken(), model.Senha, model.NovaSenha, model.ConfirmacaoNovaSenha);
                return (result == null) ? NoContent() : await Response(result, result.Notifications);
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
