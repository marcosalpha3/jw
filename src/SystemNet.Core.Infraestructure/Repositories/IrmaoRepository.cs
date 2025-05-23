﻿using Dapper;
using SendGrid;
using SendGrid.Helpers.Mail;
using SendGrid.Helpers.Mail.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using SystemNet.Core.Domain.Contracts.Repositories;
using SystemNet.Core.Domain.Models;
using SystemNet.Core.Domain.Querys;
using SystemNet.Core.Domain.Querys.Grupo;
using SystemNet.Practice.Common.Resources;
using SystemNet.Practice.Common.Values;
using SystemNet.Practices.Data;
using SystemNet.Practices.Data.Uow;
using SystemNet.Shared;

namespace SystemNet.Core.Infraestructure.Repositories
{
    public class IrmaoRepository : IIrmaoRepository
    {
        public void Ativar(ref IUnitOfWork unitOfWork, int codigo)
        {
            unitOfWork.Connection.Execute(@" UPDATE Irmao set Ativo = 1, AtivarProximaLista = 1, DesativarProximaLista = 0, AtualizarDesignacao = 1 where Codigo = @IrmaoId ",
            param: new
            {
                @IrmaoId = codigo
            },
            transaction: unitOfWork.Transaction);
        }

        public IEnumerable<GetGrupoIrmao> ObterGruposComIrmaos(ref IUnitOfWork unitOfWork, int congregacaoId)
        {
            var grupoDictionary = new Dictionary<int, GetGrupoIrmao>();

            unitOfWork.Connection.Query<GetGrupoIrmao, GetIrmaoGrupo, GetGrupoIrmao>
            (@" select G.Codigo, G.Nome, ID.Nome As Dirigente, ID.Codigo As DirigenteId, G.Local, I.Codigo As IrmaoCodigo, 
                I.Nome As Irmao
                from Grupo G
                inner join Irmao ID ON ID.Codigo = G.DirigenteId
                left join Irmao I ON I.GrupoId = G.Codigo
                WHERE  G.CongregacaoId = @CongregacaoId 
                and I.Ativo = 1 
                order By G.Nome, I.Nome",
            (pd, pp) =>
            {
                if (!grupoDictionary.TryGetValue(pd.Codigo, out GetGrupoIrmao grupoEntry))
                {
                    grupoEntry = pd;
                    grupoEntry.Irmaos = new List<GetIrmaoGrupo>();
                    grupoDictionary.Add(grupoEntry.Codigo, grupoEntry);
                }
                grupoEntry.Irmaos.Add(pp);
                return grupoEntry;
            }
            , splitOn: "IrmaoCodigo ",
            param: new { @CongregacaoId = congregacaoId }
            );

            return grupoDictionary.Values.ToList();
        }

        public void ReiniciarSenha(ref IUnitOfWork unitOfWork, Irmao model)
        {
            unitOfWork.Connection.Execute(
              "UPDATE Irmao SET Senha = @Senha, Tentativas = @Tentativas, StatusId = @StatusId, AlterarSenha = 1 where Codigo = @Id ",
                param: new { model.Senha, @Id = model.Codigo, model.Tentativas, model.StatusId },
                transaction: unitOfWork.Transaction);
        }

        public void AtualizaAtivarProximaLista(ref IUnitOfWork unitOfWork, int id)
        {
            unitOfWork.Connection.Execute("UPDATE [dbo].[Irmao]  SET [Ativo] = 1, [AtivarProximaLista] = 0  WHERE Codigo = @id",
               param: new
               {
                   id
               },
               transaction: unitOfWork.Transaction);
        }

        public void AtualizaDesativarProximaLista(ref IUnitOfWork unitOfWork, int id)
        {
            unitOfWork.Connection.Execute(@"UPDATE [dbo].[Irmao]  SET [Ativo] = 0, [DesativarProximaLista] = 0, Indicador = 0, Microfonista = 0, LeitorSentinela = 0,  
            LeitorEstudoLivro = 0, SistemaSonoro = 0, OracaoFinal = 0, PresidenteConferencia = 0, Carrinho = 0 WHERE Codigo = @id",
            param: new
            {
                id
            },
            transaction: unitOfWork.Transaction);
        }

        public void Atualizar(ref IUnitOfWork unitOfWork, Irmao model)
        {
            unitOfWork.Connection.Execute(@"UPDATE [dbo].[Irmao]
                                           SET [Nome] = @Nome, [Email] = @Email, [Telefone] = @Telefone, [Sexo] = @Sexo, [Indicador] = @Indicador, [Microfonista] = @Microfonista
                                               ,[LeitorSentinela] = @LeitorSentinela ,[LeitorEstudoLivro] = @LeitorEstudoLivro, [SistemaSonoro] = @SistemaSonoro, [OracaoFinal] = @OracaoFinal
                                               ,[PresidenteConferencia] = @PresidenteConferencia, [Carrinho] = @Carrinho, [GrupoId] = @GrupoId, [CongregacaoId] = @CongregacaoId, 
                                               [AtualizarDesignacao] = @AtualizarDesignacao, [AcessoAdmin] = @AcessoAdmin, [DataUltimaAlteracao] = GETDATE(), SubirQuadro = @SubirQuadro,
                                               [AtualizarAssistencia] = @AtualizarAssistencia, IndicadorAuditorio = @IndicadorAuditorio
                                                WHERE Codigo = @Codigo ",
                                        param: new
                                        {
                                            model.Nome,
                                            model.Email,
                                            model.Telefone,
                                            model.Sexo,
                                            model.Indicador,
                                            model.Microfonista,
                                            model.LeitorSentinela,
                                            model.LeitorEstudoLivro,
                                            model.SistemaSonoro,
                                            model.OracaoFinal,
                                            model.PresidenteConferencia,
                                            model.Carrinho,
                                            model.GrupoId,
                                            model.CongregacaoId,
                                            model.AtualizarDesignacao,
                                            model.AcessoAdmin,
                                            model.Codigo,
                                            model.SubirQuadro,
                                            model.AtualizarAssistencia,
                                            model.IndicadorAuditorio
                                        },
                                        transaction: unitOfWork.Transaction);
        }

        public void Desativar(ref IUnitOfWork unitOfWork, int codigo)
        {
            unitOfWork.Connection.Execute(@" UPDATE Irmao set Ativo = 0, Indicador = 0, Microfonista = 0, LeitorSentinela = 0, LeitorEstudoLivro = 0,
                                             SistemaSonoro = 0, OracaoFinal = 0, PresidenteConferencia = 0, Carrinho = 0, DesativarProximaLista = 1,
                                             AtivarProximaLista = 0, AtualizarDesignacao = 1 where Codigo = @IrmaoId",
            param: new
            {
                @IrmaoId = codigo
            },
            transaction: unitOfWork.Transaction);
        }

        public void UpdateGrupoCampo(ref IUnitOfWork unitOfWork, int grupoAtual, int novoGrupo)
        {
            unitOfWork.Connection.Execute(@" UPDATE Irmao set GrupoId = @NovoGrupoId  where GrupoId = @GrupoAtualId",
            param: new
            {
                @NovoGrupoId = novoGrupo,
                @GrupoAtualId = grupoAtual

            },
            transaction: unitOfWork.Transaction);
        }


        public Irmao FindById(ref IUnitOfWork unitOfWork, int id)
        {
            return unitOfWork.Connection.Query<Irmao>("Select * from dbo.Irmao where Codigo = @id",
                    param: new
                    {
                        id
                    }
                    , transaction: unitOfWork.Transaction
                ).FirstOrDefault();
        }

        public int Inserir(ref IUnitOfWork unitOfWork, Irmao model)
        {
            return Convert.ToInt32(unitOfWork.Connection.ExecuteScalar(@" INSERT INTO [dbo].[Irmao] ([Nome], [Email], [Telefone], [Sexo], [Ativo], [Indicador], [Microfonista]
                                                                          ,[LeitorSentinela], [LeitorEstudoLivro], [SistemaSonoro], [OracaoFinal], [PresidenteConferencia]
                                                                          ,[Carrinho], [GrupoId], [CongregacaoId], [Senha], [DesativarProximaLista], [AtivarProximaLista], 
                                                                          [AtualizarDesignacao], [AcessoAdmin], [StatusId], [AlterarSenha], [Tentativas], [SubirQuadro],
                                                                          [AtualizarAssistencia], [IndicadorAuditorio])
                                                                         VALUES
                                                                        (@Nome, @Email, @Telefone, @Sexo, @Ativo, @Indicador, @Microfonista, @LeitorSentinela, @LeitorEstudoLivro, @SistemaSonoro
                                                                        ,@OracaoFinal, @PresidenteConferencia, @Carrinho, @GrupoId, @CongregacaoId, @Senha, 0, 1, 1, @AcessoAdmin, @StatusId, 
                                                                        @AlterarSenha, @Tentativas, @SubirQuadro, @AtualizarAssistencia, @IndicadorAuditorio);
                                                                         SELECT SCOPE_IDENTITY() ",
                               param: new
                               {
                                   model.Nome,
                                   model.Email,
                                   model.Telefone,
                                   model.Sexo,
                                   @Ativo = 1,
                                   model.Indicador,
                                   model.Microfonista,
                                   model.LeitorSentinela,
                                   model.LeitorEstudoLivro,
                                   model.SistemaSonoro,
                                   model.OracaoFinal,
                                   model.PresidenteConferencia,
                                   model.Carrinho,
                                   model.GrupoId,
                                   model.CongregacaoId,
                                   model.Senha,
                                   model.AcessoAdmin,
                                   model.StatusId,
                                   model.AlterarSenha,
                                   model.Tentativas,
                                   model.SubirQuadro,
                                   model.AtualizarAssistencia,
                                   model.IndicadorAuditorio
                               },
                               transaction: unitOfWork.Transaction));
        }

        public IEnumerable<Irmao> ObterIrmaosADesativarOuAtivar(ref IUnitOfWork unitOfWork, int congregacaoId)
        {
            return unitOfWork.Connection.Query<Irmao>("Select * from dbo.Irmao where CongregacaoId = @congregacao and ([AtivarProximaLista] = 1 or [DesativarProximaLista] = 1 or [AtualizarDesignacao] = 1) ",
                param: new 
                {
                    @congregacao = congregacaoId
                }
                   , transaction: unitOfWork.Transaction
               );
        }

        public void AlterarSenha(ref IUnitOfWork unitOfWork, int Id, string senha)
        {
            unitOfWork.Connection.Execute("UPDATE dbo.[Irmao] SET Senha = @Senha, AlterarSenha = 0 where Codigo = @Id",
                param: new { @Senha = senha, Id },
                transaction: unitOfWork.Transaction);
        }

        public IEnumerable<GetIrmao> ObterIrmaosPorCongregacao(ref IUnitOfWork unitOfWork, int congregacaoId)
        {
            return unitOfWork.Connection.Query<GetIrmao>("Select * from dbo.Irmao where CongregacaoId = @congregacao order by nome",
                param: new
                {
                    @congregacao = congregacaoId
                }
               );
        }

        public Irmao PesquisarporEmail(ref IUnitOfWork unitOfWork, string email, int? id = null)
        {
            return unitOfWork.Connection.Query<Irmao>(
                      (id != null) ? @"SELECT I.*, C.Nome As CongregacaoNome, C.ExibirOracaoInicial FROM dbo.[Irmao] I 
                                     INNER JOIN Congregacao C ON C.Codigo = I.CongregacaoId
                                     WHERE Email = @Email  and I.Codigo <> @Codigo " :
                                     @" SELECT I.*, C.Nome As CongregacaoNome,  C.ExibirOracaoInicial FROM 
                                        dbo.[Irmao] I 
                                        INNER JOIN Congregacao C ON C.Codigo = I.CongregacaoId
                                        WHERE Email = @Email ",
                      param: new
                      {
                          @Email = email,
                          @Codigo = (IntValues.IsNullorDefault(id)) ? (int?)null : id
                      },
                      transaction: unitOfWork.Transaction
                  ).FirstOrDefault();
        }

        public Irmao PesquisarporNome(ref IUnitOfWork unitOfWork, string nome, int? id = null)
        {
            return unitOfWork.Connection.Query<Irmao>(
                      (id != null) ? @"SELECT * FROM dbo.[Irmao] WHERE UPPER(Nome) = Upper(@Nome)  and Codigo <> @Codigo " : "SELECT * FROM dbo.[Irmao] WHERE UPPER(Nome) = Upper(@Nome) ",
                      param: new
                      {
                          @Nome = nome,
                          @Codigo = (IntValues.IsNullorDefault(id)) ? (int?)null : id
                      },
                      transaction: unitOfWork.Transaction
                  ).FirstOrDefault();
        }

        public void Login(ref IUnitOfWork unitOfWork, Irmao model)
        {
            unitOfWork.Connection.Execute("update Irmao set StatusId = @StatusId, Tentativas = @Tentativas, UltimoLogin = @UltimoLogin where Codigo = @Codigo",
                param: new { model.StatusId, model.Tentativas, model.UltimoLogin, model.Codigo },
                transaction: unitOfWork.Transaction);
        }

        public void AtualizaFlagDesignacao(ref IUnitOfWork unitOfWork, int Id)
        {
            unitOfWork.Connection.Execute("UPDATE dbo.[Irmao] SET AtualizarDesignacao = 0 where Codigo = @Id",
                param: new { Id },
                transaction: unitOfWork.Transaction);
        }

        #region [Email]  

        public void SendEmail(Irmao model, string password, bool newUser)
        {

            string urlEnvironment = Runtime.UrlSystem;

            string HtmlHelloMessage = (newUser) ? Errors.HtmlBody_1 : Errors.HtmlBody_1b;

            var mailMsg = new MailMessage();
            var endereco = new MailAddress(Runtime.Sender, "JW");

            string htmlBody = "<html> <body> <p> " + String.Format(HtmlHelloMessage, model.Nome) + "</p> <p>" +
                string.Format(Errors.HtmlBody_5, urlEnvironment) + " </p> <p>" +
                Errors.HtmlBody_2 + "<b> " + model.Email + "</b> </p> <p>" +
                Errors.HtmlBody_3 + "<b> " + password + "</b> </p> <p>" +
             "</p> <p>" + Errors.HtmlBody_4 + " </p> </body></html>";

            if (!String.IsNullOrEmpty(model.Email))
            {
                var smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential(Runtime.Sender, Runtime.KeySendGrid), // Use a App Password aqui!
                    EnableSsl = true,
                };

                var subject = (newUser) ? Errors.SubjectEmail : Errors.SubjectEmail1;
                var msg = new MailMessage() { From = endereco, Body = htmlBody, IsBodyHtml = true, Subject = subject };
                msg.To.Add(model.Email);
                smtpClient.Send(msg);

                //var client = new SendGridClient(Runtime.KeySendGrid);
                //var from = new EmailAddress(Runtime.Sender);
                //var subject = "Teste de entrega Gmail"; // (newUser) ? Errors.SubjectEmail : Errors.SubjectEmail1;
                //var to = new EmailAddress(model.Email, model.Nome);
                //var msg = MailHelper.CreateSingleEmail(from, to, subject, htmlBody, "");
                //// var msg = MailHelper.CreateSingleEmail(from, to, subject,"", htmlBody);
                //var response = await client.SendEmailAsync(msg);
            }
        }

        #endregion

    }
}
