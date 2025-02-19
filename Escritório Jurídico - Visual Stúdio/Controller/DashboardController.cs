using Escritório_Jurídico___Visual_Stúdio.Models;
using Escritório_Jurídico___Visual_Stúdio.ORM;
using Escritório_Jurídico___Visual_Stúdio.Repositorio;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;

namespace Escritório_Jurídico___Visual_Stúdio.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DashboardRepositorio _dashboardRepositorio;
        private readonly ILogger<DashboardController> _logger;
        public DashboardController(DashboardRepositorio dashboardRepositorio, ILogger<DashboardController> logger)
        {
            _dashboardRepositorio = dashboardRepositorio;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var dadosGrafico = _dashboardRepositorio.ObterDadosGrafico();
            ViewBag.ChartData = JsonSerializer.Serialize(dadosGrafico.Select(d => d.Y));
            return View();
        }
        public IActionResult ContarAgendamentosPorAno(int ano)
        {
            int totalAgendamentos = _dashboardRepositorio.ContarAgendamentosPorAno(ano);
            return Json(totalAgendamentos);  // Redireciona para a View Index
        }
        public IActionResult ContarUsuariosPorAno(int ano)
        {
            int totalUsuarios = _dashboardRepositorio.ContarUsuariosPorAno(ano);
            return Json(totalUsuarios);  // Redireciona para a View Index
        }
        public IActionResult SomarLucroPorAno(int ano)
        {
            decimal lucroTotal = _dashboardRepositorio.SomarLucroPorAno(ano);
            return Json(lucroTotal);  // Redireciona para a View Index
        }
        public JsonResult ContarAgendamentosPorMes(int ano)
        {
            var dados = _dashboardRepositorio.ContarAgendamentosPorMes(ano);

            var categorias = dados.Select(d => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(d.Mes)).ToList();
            var valores = dados.Select(d => d.TotalAgendamentos).ToList();

            var seriesData = valores.Select(v => new { y = v }).ToList(); // Formatação para Highcharts

            return Json(new
            {
                categorias,
                series = new[]
                {
         new { name = "Agendamentos", data = seriesData }
     }
            });
        }
        public JsonResult ContarUsuariosPorMes(int ano)
        {
            var dados = _dashboardRepositorio.ContarUsuariosPorMes(ano);

            var categorias = dados.Select(d => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(d.Mes)).ToList();
            var valores = dados.Select(d => d.TotalUsuarios).ToList();

            var seriesData = valores.Select(v => new { y = v }).ToList(); // Formatação para Highcharts

            return Json(new
            {
                categorias,
                series = new[]
                {
         new { name = "Usuários Cadastrados", data = seriesData }
     }
            });
        }
        public JsonResult SomarLucroPorMes(int ano)
        {
            var dados = _dashboardRepositorio.SomarLucroPorMes(ano);

            var categorias = dados.Select(d => CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(d.Mes)).ToList();
            var valores = dados.Select(d => d.TotalLucro).ToList();

            var seriesData = valores.Select(v => new { y = v }).ToList(); // Formatação para Highcharts

            return Json(new
            {
                categorias,
                series = new[]
                {
         new { name = "Lucro", data = seriesData }
     }
            });
        }
        public JsonResult ConsultarEvolucaoMensalPorAno()
        {
            var dados = _dashboardRepositorio.ConsultarEvolucaoMensalAtendimentos();

            // Agrupa os dados por ano
            var dadosAgrupadosPorAno = dados
                .GroupBy(d => d.Ano)
                .Select(g => new
                {
                    Ano = g.Key,
                    DadosMensais = g.OrderBy(d => d.Mes).Select(d => d.TotalAtendimentos).ToList()
                })
                .ToList();

            // Definindo as categorias (meses)
            var categorias = Enumerable.Range(1, 12).Select(m => m.ToString("D2")).ToList(); // Meses de 01 a 12

            // Preparar as séries para o gráfico (um para cada ano)
            var series = dadosAgrupadosPorAno.Select(anoData => new
            {
                name = anoData.Ano.ToString(),
                data = anoData.DadosMensais
            }).ToList();

            // Retorna os dados no formato correto para o Highcharts
            return Json(new
            {
                categorias,
                series
            });
        }
        public JsonResult SomarServicosMaisUsadosPorAno(int ano)
        {
            // Chama o serviço para obter os dados
            var dados = _dashboardRepositorio.ConsultarServicosMaisUsadosPorAno(ano);

            // Cria as categorias com os nomes dos serviços
            var categorias = dados.Select(d => d.TipoServico).ToList();  // Utilizando TipoServico para a categoria

            // Cria os valores (quantidade de usos de cada serviço)
            var valores = dados.Select(d => d.TotalUsos).ToList();

            // Formatação para o gráfico, onde cada valor se torna um objeto com a chave 'y'
            var seriesData = valores.Select(v => new { y = v }).ToList();

            // Retorna os dados em formato JSON compatível com o gráfico
            return Json(new
            {
                categorias,  // Os nomes dos serviços
                series = new[]
                {
     new { name = "Serviços Mais Usados", data = seriesData }
 }
            });
        }

    }
}
