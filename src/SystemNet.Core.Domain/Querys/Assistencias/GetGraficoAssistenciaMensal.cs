using System.Collections.Generic;

namespace SystemNet.Core.Domain.Querys.Assistencias
{
    public class GetGraficoAssistenciaMensal
    {
        public List<string> Labels { get; set; }
        public List<Dataset> Datasets { get; set; }
    }

    public class Dataset
    {
        public string Label { get; set; }
        public List<int> Data { get; set; }
        public string BackgroundColor { get; set; }
        public string BorderColor { get; set; }
        public int BorderWidth { get; set; }
    }
}
