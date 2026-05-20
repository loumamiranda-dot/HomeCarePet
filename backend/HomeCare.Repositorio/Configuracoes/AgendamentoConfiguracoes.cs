using HomeCare.Dominio.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeCare.Repositorio.Configuracoes;

public class AgendamentoConfiguracoes : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.ToTable("Agendamento");
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Id).HasColumnName("AgendamentoId");
        builder.Property(a => a.DataHora).IsRequired();
        builder.Property(a => a.Status).IsRequired();
        builder.Property(a => a.Observacoes).HasMaxLength(500);
        builder.HasOne(a => a.Cliente).WithMany().HasForeignKey(a => a.ClienteId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Pet).WithMany().HasForeignKey(a => a.PetId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(a => a.Servico).WithMany().HasForeignKey(a => a.ServicoId).OnDelete(DeleteBehavior.Restrict);
    }
}
