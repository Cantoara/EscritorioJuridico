namespace Escritório_Jurídico___Visual_Stúdio.Models
{
    public class AgendamentoVM
    {
        public int Id { get; set; }

        public DateTime DthoraAgendamento { get; set; }

        public DateOnly DataAtendimento { get; set; }

        public TimeOnly Horario { get; set; }

        public int FkUsuarioId { get; set; }

        public int FkServicoId { get; set; }

    }
}
