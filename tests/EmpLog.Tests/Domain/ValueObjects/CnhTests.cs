using EmpLog.Domain.Enums;
using EmpLog.Domain.ValueObjects;
using FluentAssertions;

namespace EmpLog.Tests.Domain.ValueObjects;

public class CnhTests
{
    private static readonly TimeProvider Relogio = new FixedTimeProvider(new DateTimeOffset(2026, 10, 12, 15, 0, 0, TimeSpan.Zero));

    [Fact]
    public void Aceita_numero_categoria_e_validade_futura()
    {
        var criado = Cnh.TryCreate("12345678900", "ab", new DateOnly(2030, 6, 15), Relogio, out var cnh, out var erros);

        criado.Should().BeTrue();
        erros.Should().BeEmpty();
        cnh!.Numero.Should().Be("12345678900");
        cnh.Categoria.Should().Be(CategoriaCnh.AB);
        cnh.Validade.Should().Be(new DateOnly(2030, 6, 15));
    }

    [Theory]
    [InlineData("123")]
    [InlineData("11111111111")]
    [InlineData("12345678901")]
    public void Recusa_numero_invalido(string numero)
    {
        var criado = Cnh.TryCreate(numero, "B", new DateOnly(2030, 6, 15), Relogio, out var cnh, out var erros);

        criado.Should().BeFalse();
        cnh.Should().BeNull();
        erros.Should().Contain(erro => erro.Campo == "cnh.numero");
    }

    [Theory]
    [InlineData("F")]
    [InlineData("ABC")]
    [InlineData("1")]
    public void Recusa_categoria_invalida(string categoria)
    {
        var criado = Cnh.TryCreate("12345678900", categoria, new DateOnly(2030, 6, 15), Relogio, out var cnh, out var erros);

        criado.Should().BeFalse();
        cnh.Should().BeNull();
        erros.Should().Contain(erro => erro.Campo == "cnh.categoria");
    }

    [Fact]
    public void Recusa_validade_vencida_ou_igual_a_hoje()
    {
        var criado = Cnh.TryCreate("12345678900", "B", new DateOnly(2026, 10, 12), Relogio, out var cnh, out var erros);

        criado.Should().BeFalse();
        cnh.Should().BeNull();
        erros.Should().Contain(erro => erro.Campo == "cnh.validade");
    }
}
