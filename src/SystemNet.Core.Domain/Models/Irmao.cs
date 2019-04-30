using System;
using System.Collections.Generic;
using System.Text;

namespace SystemNet.Core.Domain.Models
{
    public class Irmao
    {
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
    }
}
