using HomeCare.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeCare.Repositorio.Configuracoes;

public class PetConfiguracoes : IEntityTypeConfiguration<Pet>
{
    public void Configure(EntityTypeBuilder<Pet> builder)
    {
        builder.ToTable("Pet");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Id).HasColumnName("PetId");
        builder.Property(p => p.Nome).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Tipo).IsRequired();
        builder.Property(p => p.Raca).HasMaxLength(80);
        builder.Property(p => p.Observacoes).HasMaxLength(500);
        builder.Property(p => p.Peso).HasColumnType("decimal(5,2)");
    }
}
