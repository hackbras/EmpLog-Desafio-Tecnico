using EmpLog.Domain.Entities;
using EmpLog.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmpLog.Infrastructure.Persistence.Configurations;

public sealed class MotoristaConfiguration : IEntityTypeConfiguration<Motorista>
{
    public void Configure(EntityTypeBuilder<Motorista> builder)
    {
        builder.ToTable("Motoristas");
        builder.HasKey(motorista => motorista.Id);
        builder.Property(motorista => motorista.Id).ValueGeneratedNever();
        builder.Property(motorista => motorista.NomeCompleto)
            .HasMaxLength(Motorista.TamanhoMaximoNome)
            .IsRequired();
        builder.Property(motorista => motorista.CriadoEmUtc).IsRequired();
        builder.Property(motorista => motorista.Status)
            .HasConversion<string>()
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(motorista => motorista.Cpf)
            .HasConversion(cpf => cpf.Valor, valor => Cpf.Create(valor), ComparadorCpf())
            .HasMaxLength(11)
            .IsRequired();
        builder.HasIndex(motorista => motorista.Cpf).IsUnique();

        builder.Property(motorista => motorista.Email)
            .HasConversion(email => email.Valor, valor => Email.Create(valor), ComparadorEmail())
            .HasMaxLength(Email.TamanhoMaximo)
            .IsRequired();

        builder.Property(motorista => motorista.Telefone)
            .HasConversion(telefone => telefone.Valor, valor => Telefone.Create(valor), ComparadorTelefone())
            .HasMaxLength(11)
            .IsRequired();

        builder.OwnsOne(motorista => motorista.Cnh, cnh =>
        {
            cnh.Property(valor => valor.Numero).HasColumnName("CnhNumero").HasMaxLength(11).IsRequired();
            cnh.Property(valor => valor.Categoria)
                .HasColumnName("CnhCategoria")
                .HasConversion<string>()
                .HasMaxLength(2)
                .IsRequired();
            cnh.Property(valor => valor.Validade).HasColumnName("CnhValidade").IsRequired();
            cnh.HasIndex(valor => valor.Numero).IsUnique();
        });

        builder.Navigation(motorista => motorista.Cnh).IsRequired();
    }

    private static ValueComparer<Cpf> ComparadorCpf() =>
        new(
            (esquerda, direita) => esquerda!.Valor == direita!.Valor,
            cpf => cpf.Valor.GetHashCode(),
            cpf => Cpf.Create(cpf.Valor));

    private static ValueComparer<Email> ComparadorEmail() =>
        new(
            (esquerda, direita) => esquerda!.Valor == direita!.Valor,
            email => email.Valor.GetHashCode(),
            email => Email.Create(email.Valor));

    private static ValueComparer<Telefone> ComparadorTelefone() =>
        new(
            (esquerda, direita) => esquerda!.Valor == direita!.Valor,
            telefone => telefone.Valor.GetHashCode(),
            telefone => Telefone.Create(telefone.Valor));
}
