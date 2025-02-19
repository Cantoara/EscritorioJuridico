namespace Escritório_Jurídico___Visual_Stúdio.Models
{
    public class ServicoMaisUsadoPorAno
    {
        public int Ano { get; set; }            // Ano do atendimento
        public int ServicoId { get; set; }      // ID do serviço
        public int TotalUsos { get; set; }      // Total de vezes que o serviço foi usado
        public string? TipoServico { get; set; } // Nome do serviço (nova propriedade)
    }
}
