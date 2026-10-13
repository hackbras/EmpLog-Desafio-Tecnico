using EmpLog.Domain.Entities;
using EmpLog.Domain.Enums;
using EmpLog.Domain.Exceptions;
using EmpLog.Domain.ValueObjects;
using FluentAssertions;

namespace EmpLog.Tests.Domain.Entities;

public class MotoristaTests
{
    private static readonly TimeProvider Relogio = new FixedTimeProvider(new DateTimeOffset(2026, 10, 12, 15, 0, 0, TimeSpan.Zero));

    [Fact]
    public void Cadastrar_cria_motorista_ativo()
    {
        var motorista = Motorista.Cadastrar(
            "  Ana   Silva ",
            Cpf.Create("52998224725"),
            Email.Create("Ana.Silva@Email.com"),
            Telefone.Create("(11) 98888-7777"),
            Cnh.Create("12345678900", "B", new DateOnly(2030, 6, 15), Relogio),
            Relogio);

        motorista.Id.Should().NotBeEmpty();
        motorista.NomeCompleto.Should().Be("Ana Silva");
        motorista.Cpf.Valor.Should().Be("52998224725");
        motorista.Email.Valor.Should().Be("ana.silva@email.com");
        motorista.Telefone.Valor.Should().Be("11988887777");
        motorista.Cnh.Numero.Should().Be("12345678900");
        motorista.Status.Should().Be(StatusMotorista.Ativo);
        motorista.CriadoEmUtc.Should().Be(new DateTime(2026, 10, 12, 15, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void Cadastrar_recusa_nome_invalido()
    {
        var acao = () => Motorista.Cadastrar(
            "Ana",
            Cpf.Create("52998224725"),
            Email.Create("ana.silva@email.com"),
            Telefone.Create("11988887777"),
            Cnh.Create("12345678900", "B", new DateOnly(2030, 6, 15), Relogio),
            Relogio);

        acao.Should().Throw<DomainException>();
    }
}
