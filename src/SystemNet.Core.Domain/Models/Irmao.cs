
using FluentValidator;
using FluentValidator.Validation;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using SystemNet.Practice.Common.Resources;

namespace SystemNet.Core.Domain.Models
{
    public class Irmao : Notifiable
    {
        public Irmao(int codigo, string nome, string email, string telefone, string sexo, bool indicador, bool microfonista, bool leitorSentinela, bool leitorEstudoLivro, bool sistemaSonoro, bool oracaoFinal, 
            bool presidenteConferencia, bool carrinho, int grupoId, int congregacaoId, bool acessoAdmin, bool atualizarAssistencia, bool subirQuadro)
        {
            Codigo = codigo;
            Nome = nome;
            Email = email;
            Telefone = telefone;
            Sexo = sexo;
            Indicador = indicador;
            Microfonista = microfonista;
            LeitorSentinela = leitorSentinela;
            LeitorEstudoLivro = leitorEstudoLivro;
            SistemaSonoro = sistemaSonoro;
            OracaoFinal = oracaoFinal;
            PresidenteConferencia = presidenteConferencia;
            Carrinho = carrinho;
            GrupoId = grupoId;
            CongregacaoId = congregacaoId;
            AcessoAdmin = acessoAdmin;
            AtualizarAssistencia = atualizarAssistencia;
            SubirQuadro = subirQuadro;

            AddNotifications(new ValidationContract()
                           .Requires()
                           .HasMinLen(Nome, 3, nameof(Nome), String.Format(Errors.MinName, 3))
                           .HasMaxLen(Nome, 100, nameof(Nome), String.Format(Errors.MaxName, 100))
                           .IsEmail(Email, nameof(Email), Errors.EmailInvalido)

                           .HasMaxLen(Telefone, 50, nameof(Telefone), String.Format(Errors.MaxPhone, 50))
                           .IsTrue((Sexo == "M" || Sexo == "F"), nameof(Sexo), Errors.SexInvalid)

                           .IsGreaterThan(GrupoId, 0, nameof(GrupoId), Errors.GroupRequired)
                           .IsGreaterThan(CongregacaoId, 0, nameof(CongregacaoId), Errors.CongreagationRequired)
                           );
        }

        public Irmao()  {}


        public int Codigo { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Sexo { get; set; }
        public bool Ativo { get; set; }
        public bool Indicador { get; set; }
        public bool Microfonista { get; set; }
        public bool LeitorSentinela { get; set; }
        public bool LeitorEstudoLivro { get; set; }
        public bool SistemaSonoro { get; set; }
        public bool OracaoFinal { get; set; }
        public bool PresidenteConferencia { get; set; }
        public bool Carrinho { get; set; }
        public int GrupoId { get; set; }
        public int CongregacaoId { get; set; }
        public string Senha { get; set; }
        public bool DesativarProximaLista { get; set; }
        public bool AtivarProximaLista { get; set; }
        public bool AtualizarDesignacao { get; set; }
        public bool AcessoAdmin { get; set; }
        public int StatusId { get; set; }
        public bool AlterarSenha { get; set; }
        public byte Tentativas { get; set; }
        public DateTime UltimoLogin { get; set; }
        public string CongregacaoNome { get; set; }
        public bool SubirQuadro { get; set; }
        public bool AtualizarAssistencia { get; set; }


        public void VerificaDesignacoes(Irmao modelAtual)
        {
            if ((modelAtual.Indicador != this.Indicador) || (modelAtual.LeitorEstudoLivro != this.LeitorEstudoLivro) ||
                (modelAtual.LeitorSentinela != this.LeitorSentinela) || (modelAtual.Microfonista != this.Microfonista) ||
                (modelAtual.OracaoFinal != this.OracaoFinal) || (modelAtual.PresidenteConferencia != this.PresidenteConferencia) ||
                (modelAtual.SistemaSonoro != this.SistemaSonoro))
                this.AtualizarDesignacao = true;
            else
                this.AtualizarDesignacao = false;
        }

        public bool ValidarSenha(string senha)
        {
            this.UltimoLogin = DateTime.Now;

            if (this.StatusId != (int)Status.Ativo)
            {
                AddNotification(nameof(Nome), Errors.BlockedUser);
                return false;
            }

            if (!String.Equals(CriptografarSenha(senha), this.Senha))
            {
                this.Tentativas++;
                AddNotification(nameof(Nome), Errors.InvalidPassword);
                if (this.Tentativas >= 10)
                {
                    this.StatusId = (int)Status.BloqueadoporSenha;
                    AddNotification(nameof(Nome), Errors.BlockedUser);
                }
                return false;
            }

            this.StatusId = (int)Status.Ativo;
            this.Tentativas = 0;
            return true;
        }

        public bool AlteraSenhaAtual(int userCod, string passworddatabase, string password, string newpassword, string confirmNewPass, int userconnect)
        {
            AddNotifications(new ValidationContract()
                .HasMinLen(newpassword, 8, nameof(this.Senha), String.Format(Errors.MinPassword, 8))
                .HasMaxLen(newpassword, 20, nameof(this.Senha), String.Format(Errors.MaxPassword, 20))
                .AreEquals(passworddatabase, CriptografarSenha(password), nameof(this.Senha), Errors.InvalidPassword)
                .AreNotEquals(CriptografarSenha(password), CriptografarSenha(newpassword), nameof(this.Senha), Errors.NewPasswordEqualPrevious)
                .AreEquals(newpassword, confirmNewPass, nameof(this.Senha), Errors.PasswordDoesNotMatch)
                .IsTrue(VerificaSenhaForte(newpassword), nameof(this.Senha), Errors.RequirementsPassword)
                .AreEquals(userCod, userconnect, nameof(Irmao.Email), Errors.InvalidCredentials)
                );

            if (Valid)
            {
                this.Senha = CriptografarSenha(newpassword);
                return true;
            }
            else
                return false;
        }

        private static bool VerificaSenhaForte(string password)
        {
            return Regex.IsMatch(password, @"^(?=.{8,})(?=.*[a-z])(?=.*[A-Z])(?!.*\s).*$");
        }

        public string GeraNovaSenha(int tamanho, bool desbloquearUsuario)
        {
            const string valid = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
            const string validUpper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            StringBuilder res = new StringBuilder();
            Random rnd = new Random();
            res.Append(validUpper[rnd.Next(validUpper.Length)]);
            tamanho -= 1; 
            while (0 < tamanho--)
            {
                res.Append(valid[rnd.Next(valid.Length)]);
            }
            SenhaParaNovoUsuario(res.ToString(), desbloquearUsuario);
            return res.ToString();
        }

        private void SenhaParaNovoUsuario(string password, bool desbloquear)
        {
            this.Senha = CriptografarSenha(password);
            if (desbloquear) this.StatusId = (int)Status.Ativo;
            this.AlterarSenha = true;
            if (desbloquear) this.Tentativas = 0;
        }

        public string CriptografarSenha(string senha)
        {
            if (string.IsNullOrEmpty(senha)) return "";
            var password = (senha += "91e3a161-ab82-5d64-9111-f3fbb6ah5f9b");
            var md5 = MD5.Create();
            var data = md5.ComputeHash(Encoding.Default.GetBytes(password));
            var sbString = new StringBuilder();
            foreach (var t in data)
                sbString.Append(t.ToString("x2"));

            return sbString.ToString();
        }

        public enum Status : int
        {
            Ativo = 0,
            BloqueadoporSenha = 1,
            BloqueadoPorRequisicao = 2
        }
    }
}
