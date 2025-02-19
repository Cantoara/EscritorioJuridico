
using Microsoft.EntityFrameworkCore;
using Escritório_Jurídico___Visual_Stúdio.Models;
using Escritório_Jurídico___Visual_Stúdio.ORM;
using System.Drawing;
using System.Globalization;

namespace Escritório_Jurídico___Visual_Stúdio.Repositorio
{
    public class AgendamentoRepositorio
    {

        private PotereDeMeritisContext _context;

        public AgendamentoRepositorio(PotereDeMeritisContext context)
        {
            _context = context;
        }
        // Método para inserir um novo agendamento
        public bool InserirAgendamento(DateTime dtHoraAgendamento, DateOnly dataAtendimento, TimeOnly horario, int fkUsuarioId, int fkServicoId)
        {
            try
            {
                // Exibindo os valores que estão sendo passados para o método (ajuda na depuração)
                Console.WriteLine($"Data e Hora do Agendamento: {dtHoraAgendamento}");
                Console.WriteLine($"Data do Atendimento: {dataAtendimento}");
                Console.WriteLine($"Horário: {horario}");
                Console.WriteLine($"ID do Usuário: {fkUsuarioId}");
                Console.WriteLine($"ID do Serviço: {fkServicoId}");

                // Criando uma instância do modelo AtendimentoVM
                var atendimento = new TbAgendamento
                {
                    DthoraAgendamento = dtHoraAgendamento,
                    DataAtendimento = dataAtendimento,
                    Horario = horario,
                    FkUsuarioId = fkUsuarioId,
                    FkServicoId = fkServicoId
                };

                // Verificando se a instância de _context é nula
                if (_context == null)
                {
                    Console.WriteLine("Contexto _context é nulo!");
                    return false;
                }

                // Verificando se a tabela de agendamentos existe no contexto
                if (_context.TbAgendamentos == null)
                {
                    Console.WriteLine("Tabela TbAgendamentos não encontrada no contexto!");
                    return false;
                }

                // Adicionando o atendimento ao contexto
                _context.TbAgendamentos.Add(atendimento);

                // Salvando as mudanças no banco de dados
                _context.SaveChanges(); // Persistindo as mudanças no banco de dados

                Console.WriteLine("Agendamento cadastrado com sucesso!");

                return true; // Retorna true indicando sucesso
            }
            catch (Exception ex)
            {
                // Logando a exceção completa
                Console.WriteLine($"Erro ao inserir agendamento: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");

                return false; // Retorna false em caso de erro
            }
        }

        public List<ViewAgendamentoVM> ListarAgendamento()
        {
            List<ViewAgendamentoVM> listAgendamento = new List<ViewAgendamentoVM>();

            // Recuperando todos os Agendamento do DbSet
            var listTb = _context.ViewAgendamentos.ToList();

            foreach (var item in listTb)
            {
                var agendamento = new ViewAgendamentoVM
                {
                    Id = item.Id,
                    DthoraAgendamento = item.DthoraAgendamento,
                    DataAtendimento = item.DataAtendimento,
                    Horario = item.Horario,
                    TipoServico = item.TipoServico,
                    Valor = item.Valor,
                    Nome = item.Nome,
                    Email = item.Email,
                    Telefone = item.Telefone

                };

                listAgendamento.Add(agendamento);
            }

            return listAgendamento;
        }

        public List<ViewAgendamentoVM> ListarAgendamentosCliente()
        {
            // Obtendo o ID do usuário a partir da variável de ambiente
            string nome = Environment.GetEnvironmentVariable("USUARIO_NOME");

            List<ViewAgendamentoVM> listAtendimentos = new List<ViewAgendamentoVM>();

            // Recuperando todos os agendamentos que correspondem ao ID do usuário
            var listTb = _context.ViewAgendamentos.Where(x => x.Nome == nome).ToList();

            // Convertendo cada agendamento para ViewAgendamentoVM
            foreach (var item in listTb)
            {
                var atendimento = new ViewAgendamentoVM
                {
                    Id = item.Id,
                    DthoraAgendamento = item.DthoraAgendamento,
                    DataAtendimento = item.DataAtendimento,
                    Horario = item.Horario,
                    TipoServico = item.TipoServico,
                    Valor = item.Valor,
                    Nome = item.Nome,
                    Email = item.Email,
                    Telefone = item.Telefone,
                };

                listAtendimentos.Add(atendimento);
            }

            return listAtendimentos;
        }
        // Método para atualizar um atendimento
        public bool AlterarAgendamento(int id, string data, int servico, TimeOnly horario)
        {
            try
            {
                TbAgendamento agt = _context.TbAgendamentos.Find(id);
                DateOnly dtHoraAgendamento;
                if (agt != null)
                {
                    agt.Id = id;
                    if (data != null)
                    {
                        if (DateOnly.TryParse(data, out dtHoraAgendamento))
                        {
                            agt.DataAtendimento = dtHoraAgendamento;
                        }
                    }

                    // Corrigido a verificação do tipo TimeOnly
                    if (horario != TimeOnly.MinValue)  // Verificando se o horário não é o valor padrão
                    {
                        agt.Horario = horario;
                    }

                    agt.FkServicoId = servico;
                    _context.SaveChanges();
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        // Método para excluir um atendimento
        public bool ExcluirAgendamento(int id)
        {
            try
            {


                var agt = _context.TbAgendamentos.Where(a => a.Id == id).FirstOrDefault();
                if (agt != null)
                {
                    _context.TbAgendamentos.Remove(agt);

                }
                _context.SaveChanges();
                return true;
            }

            catch (Exception)
            {

                return false;
            }
        }

        public List<AgendamentoVM> ConsultarAgendamento(string datap)
        {
            if (string.IsNullOrEmpty(datap))
            {
                // Se o parâmetro for vazio ou nulo, retornamos uma lista vazia ou podemos tratar conforme necessário
                Console.WriteLine("O parâmetro 'datap' está vazio ou nulo.");
                return new List<AgendamentoVM>(); // Retorna uma lista vazia
            }

            try
            {
                // Tenta converter a string para DateOnly, caso contrário retorna uma lista vazia
                DateOnly data = DateOnly.ParseExact(datap, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                string dataFormatada = data.ToString("yyyy-MM-dd"); // Formato desejado: "yyyy-MM-dd"
                Console.WriteLine(dataFormatada);

                // Consulta ao banco de dados, convertendo para o tipo AgendamentoVM
                var ListaAgendamento = _context.TbAgendamentos
                    .Where(a => a.DataAtendimento == DateOnly.Parse(dataFormatada))
                    .Select(a => new AgendamentoVM
                    {
                        // Mapear as propriedades de TbAgendamento para AgendamentoVM
                        Id = a.Id,
                        DthoraAgendamento = a.DthoraAgendamento,
                        DataAtendimento = DateOnly.Parse(dataFormatada),
                        Horario = a.Horario,
                        FkUsuarioId = a.FkUsuarioId,
                        FkServicoId = a.FkServicoId
                    })
                    .ToList(); // Converte para uma lista

                return ListaAgendamento;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao consultar agendamentos: {ex.Message}");
                return new List<AgendamentoVM>(); // Retorna uma lista vazia em caso de erro
            }
        }

        public List<UsuarioVM> ListarNomesAgendamentos()
        {
            // Lista para armazenar os usuários com apenas Id e Nome
            List<UsuarioVM> listFun = new List<UsuarioVM>();

            // Obter apenas os campos Id e Nome da tabela TbUsuarios
            var listTb = _context.TbUsuarios
                                 .Select(u => new UsuarioVM
                                 {
                                     Id = u.Id,
                                     Nome = u.Nome
                                 })
                                 .ToList();

            // Retorna a lista já com os campos filtrados
            return listTb;
        }

       
       
    }
}
