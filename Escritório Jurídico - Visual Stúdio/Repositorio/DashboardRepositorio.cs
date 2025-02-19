using Escritório_Jurídico___Visual_Stúdio.Models;
using Escritório_Jurídico___Visual_Stúdio.ORM;
using Highsoft.Web.Mvc.Charts;
using Microsoft.EntityFrameworkCore;


namespace Escritório_Jurídico___Visual_Stúdio.Repositorio
{
    public class DashboardRepositorio
    {

        private PotereDeMeritisContext _context;
        public DashboardRepositorio(PotereDeMeritisContext context)
        {
            _context = context;
        }
        public List<LineSeriesData> ObterDadosGrafico()
        {
            return new List<LineSeriesData>
                 {
                     new LineSeriesData { Y = 10 },
                     new LineSeriesData { Y = 25 },
                     new LineSeriesData { Y = 35 },
                     new LineSeriesData { Y = 50 }
                 };
        }
        public int ContarAgendamentosPorAno(int ano)
        {
            return _context.TbAgendamentos
                .Where(a => a.DthoraAgendamento.Year == ano)
                .Count();
        }
        public int ContarUsuariosPorAno(int ano)
        {
            return _context.TbUsuarios
                           .Where(u => u.DataHoraCadastro.Year == ano)
                           .Count();
        }
        public decimal SomarLucroPorAno(int ano)
        {
            var lucroTotal = _context.ViewAgendamentos
                                     .Where(a => a.DthoraAgendamento.Year == ano)
                                     .Sum(a => (decimal?)a.Valor) ?? 0;

            return lucroTotal;
        }
        public IEnumerable<AgendamentoPorMes> ContarAgendamentosPorMes(int ano)
        {
            return _context.TbAgendamentos
                .Where(a => a.DthoraAgendamento.Year == ano)
                .GroupBy(a => a.DthoraAgendamento.Month)
                .OrderBy(g => g.Key)
                .Select(g => new AgendamentoPorMes
                {
                    Mes = g.Key,
                    TotalAgendamentos = g.Count()
                })
                .ToList();
        }
        public IEnumerable<UsuarioPorMes> ContarUsuariosPorMes(int ano)
        {
            return _context.TbUsuarios
                .Where(u => u.DataHoraCadastro.Year == ano)
                .GroupBy(u => u.DataHoraCadastro.Month)
                .OrderBy(g => g.Key)
                .Select(g => new UsuarioPorMes
                {
                    Mes = g.Key,
                    TotalUsuarios = g.Count()
                })
                .ToList();
        }
        public IEnumerable<LucroPorMes> SomarLucroPorMes(int ano)
        {
            return _context.ViewAgendamentos
                .Where(a => a.DthoraAgendamento.Year == ano)
                .GroupBy(a => a.DthoraAgendamento.Month)
                .Select(g => new LucroPorMes
                {
                    Mes = g.Key,
                    TotalLucro = g.Sum(a => (decimal?)a.Valor) ?? 0
                })
                .ToList();
        }
        public IEnumerable<AgendamentosPorAnoMes> ConsultarEvolucaoMensalAtendimentos()
        {
            var resultados = _context.TbAgendamentos
                .GroupBy(a => new { a.DataAtendimento.Year, a.DataAtendimento.Month })  // Agrupa por ano e mês
                .Select(group => new AgendamentosPorAnoMes
                {
                    Ano = group.Key.Year,
                    Mes = group.Key.Month,
                    TotalAtendimentos = group.Count()
                })
                .OrderBy(result => result.Ano)
                .ThenBy(result => result.Mes)  // Ordena primeiro por ano e depois por mês
                .ToList();

            return resultados;
        }
        public IEnumerable<ServicoMaisUsadoPorAno> ConsultarServicosMaisUsadosPorAno(int ano)
        {
            var resultados = _context.TbAgendamentos
                .Where(a => a.DataAtendimento.Year == ano)  // Filtra por ano
                .GroupBy(a => new { a.DataAtendimento.Year, a.FkServicoId })  // Agrupa por ano e serviço
                .Select(group => new ServicoMaisUsadoPorAno
                {
                    Ano = group.Key.Year,  // Ano
                    ServicoId = group.Key.FkServicoId,  // ID do serviço
                    TotalUsos = group.Count(),  // Contagem de agendamentos por serviço
                    TipoServico = group.FirstOrDefault().FkServico.TipoServico  // Acessando a propriedade TipoServico
                })
                .OrderByDescending(result => result.TotalUsos)  // Ordena pela quantidade de usos
                .ToList();

            return resultados;
        }

    }
}
