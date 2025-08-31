using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace PropertyManagement.Application.Core.Behaviours;

public class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger) 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger = logger;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var requestGuid = Guid.NewGuid().ToString();
        
        var requestData = JsonSerializer.Serialize(request, new JsonSerializerOptions
        {
            WriteIndented = true,
            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles
        });
        
        _logger.LogInformation(
            "Beginning request {RequestName} {RequestGuid} with data: {RequestData}",
            requestName, requestGuid, requestData);
        
        var timer = Stopwatch.StartNew();
        
        try
        {
            var response = await next();
            timer.Stop();
            
            _logger.LogInformation(
                "Successfully completed request {RequestName} {RequestGuid} in {ElapsedMilliseconds}ms",
                requestName, requestGuid, timer.ElapsedMilliseconds);
            
            return response;
        }
        catch (Exception ex)
        {
            timer.Stop();
            _logger.LogError(
                ex,
                "Request {RequestName} {RequestGuid} failed after {ElapsedMilliseconds}ms",
                requestName, requestGuid, timer.ElapsedMilliseconds);
            
            throw;
        }
    }
}