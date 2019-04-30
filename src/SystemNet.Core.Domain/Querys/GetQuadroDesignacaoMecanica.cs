using System;
using System.Collections.Generic;

namespace SystemNet.Core.Domain.Querys
{
    public class GetQuadroDesignacaoMecanica
    {
        public DateTime Data { get; set; }
        public string DataFormatada { get; set; }
        public List<string> Indicadores { get; set; }
        public List<string> Microfonistas { get; set; }
        public List<string> SomVideo { get; set; }
        public string Leitor { get; set; }
        public string OracaoFinal { get; set; }
        public string Evento { get; set; }
    }
}
