using EmpLog.Domain.ValueObjects;
using FluentAssertions;

namespace EmpLog.Tests.Domain.ValueObjects;

public class CpfTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("529.982.247-25")]
    [InlineData("39053344705")]
    public void Aceita_cpf_valido(string texto)
    {
        var criado = Cpf.TryCreate(texto, out var cpf, out var erro);

        criado.Should().BeTrue();
        erro.Should().BeEmpty();
        cpf!.Valor.Should().Be(new string(texto.Where(char.IsDigit).ToArray()));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("123")]
    [InlineData("11111111111")]
    [InlineData("52998224726")]
    public void Recusa_cpf_invalido(string? texto)
    {
        var criado = Cpf.TryCreate(texto, out var cpf, out var erro);

        criado.Should().BeFalse();
        cpf.Should().BeNull();
        erro.Should().NotBeNullOrWhiteSpace();
    }
}
