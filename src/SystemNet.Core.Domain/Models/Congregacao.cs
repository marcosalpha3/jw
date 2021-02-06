using System;

namespace SystemNet.Core.Domain.Models
{
    public class Congregacao
    {
        public int Codigo { get; set; }
        public string Nome { get; set; }
        public DayOfWeek DiaReuniaoServico { get; set; }
        public DayOfWeek DiaReuniaoSentinela { get; set; }
        public byte QuantidadeIndicadores { get; set; }
        public byte QuantidadeMicrofonistas { get; set; }
        public DateTime DataPrimeiraLista { get; set; }
        public int DiasAntecedenciaGerarLista { get; set; }
        public bool FolgaParticipacao { get; set; }
        public int QuantidadeSistemaSonoro { get; set; }
    }
}
