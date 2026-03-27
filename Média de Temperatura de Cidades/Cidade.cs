using System.Text.Json.Serialization; // Necessário para os atributos

namespace AppCidades
{
    public class Cidade
    {
        [JsonPropertyName("cidade")] // Mapeia o nome diferente no JSON
        public string Nome { get; set; }

        public string Pais { get; set; }
        public string Continente { get; set; }
        public double Verao { get; set; }
        public double Outono { get; set; }
        public double Inverno { get; set; }
        public double Primavera { get; set; }
        public double Media { get; set; }
    }
}
