using System;
using System.Collections.Generic;
using System.Text;
using SystemNet.Core.Domain.enums;

namespace SystemNet.Core.Domain.Models
{
    public class TipoLista
    {
        public eTipoLista Codigo { get; set; }
        public string Descricao { get; set; }
        public bool PodeRepetirDia { get; set; }
        public int QuantidadeDatas { get; set; }
        public int CongregacaoId { get; set; }
    }
}
