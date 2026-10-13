using EmpLog.Application.Motoristas.DTOs;
using EmpLog.Domain.Entities;

namespace EmpLog.Application.Motoristas.Commands.CadastrarMotorista;

public sealed record CadastrarMotoristaResponse(
    Guid Id,
    string NomeCompleto,
    string Cpf,
    string Email,
    string Telefone,
    CnhDto Cnh,
    string Status,
    DateTime CriadoEmUtc)
{
    public static CadastrarMotoristaResponse From(MotoristaDto motorista) =>
        new(
            motorista.Id,
            motorista.NomeCompleto,
            motorista.Cpf,
            motorista.Email,
            motorista.Telefone,
            motorista.Cnh,
            motorista.Status,
            motorista.CriadoEmUtc);

    public static CadastrarMotoristaResponse From(Motorista motorista) =>
        From(MotoristaDto.From(motorista));
}
