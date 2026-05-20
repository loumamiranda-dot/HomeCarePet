using HomeCare.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeCare.Repositorio.Configuracoes;

public class ClienteConfiguracoes : IEntityTypeConfiguration<Cliente>
{
    public void Configure(EntityTypeBuilder<Cliente> builder)
    {
        builder.ToTable("Cliente");
        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnName("ClienteId");
        builder.Property(c => c.Nome).IsRequired().HasMaxLength(100);
        builder.Property(c => c.Email).IsRequired().HasMaxLength(150);
        builder.Property(c => c.Telefone).HasMaxLength(20);
        builder.Property(c => c.Endereco).HasMaxLength(250);
        builder.Property(c => c.DataCadastro).IsRequired();
        builder.Property(c => c.Ativo).IsRequired();
        builder.HasOne(c => c.Usuario).WithOne().HasForeignKey<Cliente>(c => c.UsuarioId);
        builder.HasMany(c => c.Pets).WithOne(p => p.Cliente).HasForeignKey(p => p.ClienteId);
    }
}
