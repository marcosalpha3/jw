using System.ComponentModel;

namespace SystemNet.Core.Domain.enums
{
    public enum eTipoLista : int
    {
        [Description("Indicador")]
        IndicadorEntrada = 1,
        [Description("Microfonista")]
        Microfonista = 2,
        [Description("LeitorSentinela")]
        LeitorJW = 3,
        [Description("OracaoFinal")]
        OracaoFinal = 4,
        [Description("LeitorEstudoLivro")]
        LeitorELC = 5,
        [Description("SistemaSonoro")]
        AudioVideo = 6,
        [Description("IndicadorAuditorio")]
        IndicadorAuditorio = 7
    }
}
