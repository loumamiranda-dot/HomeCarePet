using HomeCare.Aplicacao;
using HomeCare.Aplicacao.Interfaces;
using HomeCare.Api.Servicos;
using HomeCare.Repositorio;
using HomeCare.Repositorio.Consulta;
using HomeCare.Repositorio.Contexto;
using HomeCare.Repositorio.Fabrica;
using HomeCare.Repositorio.Interfaces;
using HomeCare.Repositorio.Interfaces.Consulta;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.local.json", optional: true, reloadOnChange: true);

// configurando o banco
builder.Services.AddDbContext<HomeCareContexto>(opcoes =>
    opcoes.UseSqlServer(builder.Configuration.GetConnectionString("Padrao") ?? string.Empty));

// registrando os repositorios
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
builder.Services.AddScoped<IPetRepositorio, PetRepositorio>();
builder.Services.AddScoped<IServicoRepositorio, ServicoRepositorio>();
builder.Services.AddScoped<IAgendamentoRepositorio, AgendamentoRepositorio>();

// conexao pro dapper (pra usar stored procedures)
builder.Services.AddSingleton<IConexaoFabrica, ConexaoFabrica>();

// repositorios de consulta com dapper
builder.Services.AddScoped<IAgendamentoConsultaRepositorio, AgendamentoConsultaRepositorio>();
builder.Services.AddScoped<IClienteConsultaRepositorio, ClienteConsultaRepositorio>();
builder.Services.AddScoped<IServicoConsultaRepositorio, ServicoConsultaRepositorio>();
builder.Services.AddScoped<IDashboardRepositorio, DashboardRepositorio>();

// client http pra chamar a api da groq
builder.Services.AddHttpClient("Groq", cliente =>
{
    cliente.BaseAddress = new Uri("https://api.groq.com/");
    cliente.Timeout = TimeSpan.FromSeconds(30);
});

// camada de aplicacao
builder.Services.AddScoped<IAutenticacaoAplicacao, AutenticacaoAplicacao>();
builder.Services.AddScoped<IUsuarioAplicacao, UsuarioAplicacao>();
builder.Services.AddScoped<IClienteAplicacao, ClienteAplicacao>();
builder.Services.AddScoped<IPetAplicacao, PetAplicacao>();
builder.Services.AddScoped<IServicoAplicacao, ServicoAplicacao>();
builder.Services.AddScoped<IAgendamentoAplicacao, AgendamentoAplicacao>();
builder.Services.AddScoped<ISugestaoPacoteAplicacao, SugestaoPacoteAplicacao>();
builder.Services.AddHostedService<FinalizarAgendamentosService>();

// configuracao do jwt
var jwtChave = builder.Configuration["Jwt:Chave"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opcoes =>
    {
        opcoes.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Emissor"],
            ValidAudience = builder.Configuration["Jwt:Audiencia"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtChave))
        };
    });

builder.Services.AddAuthorization();

// TODO: talvez mudar pra aceitar qualquer origem em dev
builder.Services.AddCors(opcoes =>
    opcoes.AddPolicy("HomeCarePolicy", politica =>
        politica.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()));

builder.Services.AddControllers();

// swagger - configurei pra aceitar o token jwt
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "HomeCare API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Informe: Bearer {seu_token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// tratamento de erro global - peguei de um tutorial
app.UseExceptionHandler(erro =>
{
    erro.Run(async contexto =>
    {
        contexto.Response.StatusCode = 500;
        contexto.Response.ContentType = "application/json";
        var feature = contexto.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
        var mensagem = feature?.Error switch
        {
            UnauthorizedAccessException => "Acesso negado.",
            _ => feature?.Error.Message ?? "Erro interno do servidor."
        };
        await contexto.Response.WriteAsJsonAsync(new { Mensagem = mensagem });
    });
});

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("HomeCarePolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// roda as migrations automatico quando sobe a api
using (var escopo = app.Services.CreateScope())
{
    var contexto = escopo.ServiceProvider.GetRequiredService<HomeCareContexto>();
    contexto.Database.Migrate();
}

Console.WriteLine("API rodando!");
app.Run();
