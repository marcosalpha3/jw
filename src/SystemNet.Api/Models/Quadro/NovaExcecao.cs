using System;

namespace SystemNet.Api.Models.Quadro
{
    public class NovaExcecao
    {
        public DateTime Data { get; set; }
        public int IrmaoId { get; set; }
        public string Motivo { get; set; }
    }
}
