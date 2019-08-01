using System;

namespace SystemNet.Api.Models.Quadro
{
    public class NovoEvento
    {
        public DateTime Data { get; set; }
        public string Descricao { get; set; }
        public int CongregacaoId { get; set; }
        public string TipoEvento { get; set; }
    }
}
