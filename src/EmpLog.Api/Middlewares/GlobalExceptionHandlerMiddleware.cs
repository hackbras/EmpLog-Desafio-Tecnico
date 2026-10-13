using EmpLog.Domain.Exceptions;

namespace EmpLog.Api.Middlewares;

public sealed class GlobalExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

    public GlobalExceptionHandlerMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
        {
            throw;
        }
        catch (DomainException excecao)
        {
            if (context.Response.HasStarted)
            {
                throw;
            }

            _logger.LogWarning(excecao, "Estado inválido no domínio.");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await Results.Problem(
                title: "Requisição inválida",
                detail: excecao.Message,
                statusCode: StatusCodes.Status400BadRequest)
                .ExecuteAsync(context);
        }
        catch (Exception excecao)
        {
            if (context.Response.HasStarted)
            {
                throw;
            }

            _logger.LogError(excecao, "Erro não previsto ao processar a requisição.");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await Results.Problem(
                title: "Erro interno",
                detail: "Ocorreu um erro inesperado.",
                statusCode: StatusCodes.Status500InternalServerError)
                .ExecuteAsync(context);
        }
    }
}
