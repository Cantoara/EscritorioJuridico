using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Escritório_Jurídico___Visual_Stúdio.ORM;

public partial class PotereDeMeritisContext : DbContext
{
    public PotereDeMeritisContext()
    {
    }

    public PotereDeMeritisContext(DbContextOptions<PotereDeMeritisContext> options)
        : base(options)
    {
    }

    public virtual DbSet<TbAgendamento> TbAgendamentos { get; set; }

    public virtual DbSet<TbServico> TbServicos { get; set; }

    public virtual DbSet<TbUsuario> TbUsuarios { get; set; }

    public virtual DbSet<ViewAgendamento> ViewAgendamentos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=LAB205_11\\SQLEXPRESS;Database=PotereDeMeritis;User Id=Cantoara;Password=061826;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TbAgendamento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_TB_Atendimento");

            entity.ToTable("TB_Agendamento");

            entity.Property(e => e.DthoraAgendamento)
                .HasColumnType("datetime")
                .HasColumnName("DTHoraAgendamento");
            entity.Property(e => e.FkServicoId).HasColumnName("fk_Servico_ID");
            entity.Property(e => e.FkUsuarioId).HasColumnName("fk_Usuario_ID");
            entity.Property(e => e.Horario).HasPrecision(0);

            entity.HasOne(d => d.FkServico).WithMany(p => p.TbAgendamentos)
                .HasForeignKey(d => d.FkServicoId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TB_Atendimento_TB_Servico");

            entity.HasOne(d => d.FkUsuario).WithMany(p => p.TbAgendamentos)
                .HasForeignKey(d => d.FkUsuarioId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_TB_Atendimento_TB_Usuario");
        });

        modelBuilder.Entity<TbServico>(entity =>
        {
            entity.ToTable("TB_Servico");

            entity.Property(e => e.TipoServico)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Valor).HasColumnType("decimal(18, 0)");
        });

        modelBuilder.Entity<TbUsuario>(entity =>
        {
            entity.ToTable("TB_Usuario");

            entity.Property(e => e.DataHoraCadastro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Senha)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Telefone)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ViewAgendamento>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("View_Agendamentos");

            entity.Property(e => e.DthoraAgendamento)
                .HasColumnType("datetime")
                .HasColumnName("DTHoraAgendamento");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Nome)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Telefone)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.TipoServico)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Valor).HasColumnType("decimal(18, 0)");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
