using System;

namespace SystemNet.Core.Domain.Models
{
    public class QuadroDetalhe
    {
        public int CodigoQuadroDetalhe { get; set; }
        public int QuadroId { get; set; }
        public DateTime Data { get; set; }
        public int IrmaoId { get; set; }
        public int EventoId { get; set; }
    }
}
