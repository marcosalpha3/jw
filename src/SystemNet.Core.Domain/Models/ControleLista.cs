using System;
using System.Collections.Generic;
using System.Text;

namespace SystemNet.Core.Domain.Models
{
    public class ControleLista
    {
        public int CodigoControleLista { get; set; }
        public int TipoListaId { get; set; }
        public int IrmaoId { get; set; }
        public int CongregacaoId { get; set; }
        public bool Participou { get; set; }
    }
}
