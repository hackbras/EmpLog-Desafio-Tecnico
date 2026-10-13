using EmpLog.Domain.Common;
using Microsoft.AspNetCore.Mvc;

namespace EmpLog.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase : ControllerBase
{
    protected IActionResult Problema(Result resultado)
    {
        if (resultado.Conflito)
        {
            var erro = resultado.Erros[0];
            return Conflict(new ProblemDetails
            {
                Status = StatusCodes.Status409Conflict,
                Title = "Conflito",
                Detail = erro.Mensagem,
                Type = "https://httpstatuses.com/409"
            });
        }

        var erros = resultado.Erros
            .GroupBy(erro => erro.Campo)
            .ToDictionary(grupo => grupo.Key, grupo => grupo.Select(erro => erro.Mensagem).ToArray());

        return BadRequest(new ValidationProblemDetails(erros)
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Requisição inválida",
            Type = "https://httpstatuses.com/400"
        });
    }
}
