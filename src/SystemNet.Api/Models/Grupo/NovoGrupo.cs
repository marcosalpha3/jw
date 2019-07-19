namespace SystemNet.Api.Models.Grupo
{
    public class NovoGrupo
    {
        public string Nome { get; set; }
        public int DirigenteId { get; set; }
        public int CongregacaoId { get; set; }
        public string Local { get; set; }
    }
}
