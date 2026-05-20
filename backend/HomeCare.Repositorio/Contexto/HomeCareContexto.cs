using HomeCare.Dominio.Entidades;
using HomeCare.Dominio.Enumeradores;
using HomeCare.Repositorio.Configuracoes;
using Microsoft.EntityFrameworkCore;

namespace HomeCare.Repositorio.Contexto;

public class HomeCareContexto : DbContext
{
    public HomeCareContexto(DbContextOptions<HomeCareContexto> options) : base(options) { }

    public DbSet<Usuario> Usuarios { get; set; } = null!;
    public DbSet<Cliente> Clientes { get; set; } = null!;
    public DbSet<Pet> Pets { get; set; } = null!;
    public DbSet<Servico> Servicos { get; set; } = null!;
    public DbSet<Agendamento> Agendamentos { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UsuarioConfiguracoes());
        modelBuilder.ApplyConfiguration(new ClienteConfiguracoes());
        modelBuilder.ApplyConfiguration(new PetConfiguracoes());
        modelBuilder.ApplyConfiguration(new ServicoConfiguracoes());
        modelBuilder.ApplyConfiguration(new AgendamentoConfiguracoes());

        modelBuilder.Entity<Servico>().HasData(
            new Servico { Id = 1, Nome = "Banho", Descricao = "Banho completo com secagem.", Preco = 60m, DuracaoEmMinutos = 60, Ativo = true },
            new Servico { Id = 2, Nome = "Tosa", Descricao = "Tosa higiênica ou completa.", Preco = 80m, DuracaoEmMinutos = 90, Ativo = true },
            new Servico { Id = 3, Nome = "Passeio", Descricao = "Passeio supervisionado de 30 minutos.", Preco = 40m, DuracaoEmMinutos = 30, Ativo = true },
            new Servico { Id = 4, Nome = "Cuidados em Casa", Descricao = "Visita domiciliar para cuidados e companhia.", Preco = 120m, DuracaoEmMinutos = 120, Ativo = true }
        );

        modelBuilder.Entity<Usuario>().HasData(
            new Usuario
            {
                Id = 1,
                Nome = "Administrador",
                Email = "admin@homecare.com",
                SenhaHash = "$2a$11$8HyPTj41CCgCKLmv0.ZN9OZB955swFQ4Sp1luKJ7S5FQOaIahw3VW",
                TipoUsuario = TiposUsuario.Administrador,
                Ativo = true
            }
        );
    }
}
