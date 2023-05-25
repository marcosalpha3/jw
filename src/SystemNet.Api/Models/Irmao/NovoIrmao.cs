namespace SystemNet.Api.Models.Irmao
{
    public class NovoIrmao
    {
        public string Nome { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Sexo { get; set; }
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
        public bool AcessoAdmin { get; set; }
        public bool SubirQuadro { get; set; }
        public bool AtualizarAssistencia { get; set; }
        public bool IndicadorAuditorio { get; set; }
    }
}
