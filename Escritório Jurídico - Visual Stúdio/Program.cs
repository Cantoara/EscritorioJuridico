using Escritório_Jurídico___Visual_Stúdio.ORM;
using Escritório_Jurídico___Visual_Stúdio.Repositorio;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;

var builder = WebApplication.CreateBuilder(args);

// Registrar o DbContext
builder.Services.AddDbContext<PotereDeMeritisContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Registrar os repositórios
builder.Services.AddScoped<UsuarioRepositorio>();
builder.Services.AddScoped<ServicoRepositorio>();
builder.Services.AddScoped<AgendamentoRepositorio>();
builder.Services.AddScoped<RelatorioRepositorio>();
builder.Services.AddScoped<DashboardRepositorio>();



// Adicionar suporte a sessões
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Tempo de expiração da sessão
    options.Cookie.HttpOnly = true; // Torna o cookie acessível apenas via HTTP
});

// Adicionar suporte a autenticação com cookies
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Usuario/Login";  // Caminho para a página de login
        options.LogoutPath = "/Usuario/Logout";  // Caminho para a página de logout
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);  // Tempo de expiração do cookie
    });

// Registrar outros serviços, como controllers com views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configurar o pipeline de requisição HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Adicionar o middleware de sessão
app.UseSession();

// Colocar a ordem correta dos middlewares de autenticação e autorização
app.UseRouting();

// Middleware de autenticação: permite identificar quem é o usuário
app.UseAuthentication();  // Coloque antes de UseAuthorization()

// Middleware de autorização: verifica se o usuário tem permissão para acessar a rota
app.UseAuthorization();   // Deve ser chamado depois de UseAuthentication()

// Configurar as rotas dos controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Rodar a aplicação
app.Run();
