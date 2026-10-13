using EmpLog.Application.Motoristas.Commands.CadastrarMotorista;
using Microsoft.AspNetCore.Mvc;

namespace EmpLog.Api.Controllers.v1;

[Route("api/v1/motoristas")]
public sealed class MotoristasController : ApiControllerBase
{
    private readonly CadastrarMotoristaHandler _handler;

    public MotoristasController(CadastrarMotoristaHandler handler)
    {
        _handler = handler;
    }

    [HttpPost]
    [ProducesResponseType(typeof(CadastrarMotoristaResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Cadastrar(
        [FromBody] CadastrarMotoristaCommand comando,
        CancellationToken cancellationToken)
    {
        var resultado = await _handler.Handle(comando, cancellationToken);
        if (!resultado.Sucesso || resultado.Valor is null)
        {
            return Problema(resultado);
        }

        return Created($"/api/v1/motoristas/{resultado.Valor.Id}", resultado.Valor);
    }
}
