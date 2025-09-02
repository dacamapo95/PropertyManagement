using Carter;
using MediatR;
using PropertyManagement.API.Extensions;
using PropertyManagement.Application.Features.Owners.GetIdentificationTypes;
using PropertyManagement.Application.Features.Properties.GetStatuses;

namespace PropertyManagement.API.Endpoints;

public sealed class MasterData : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/master").WithTags("Master");

        group.MapGet("/identification-types", async (ISender sender) =>
        {
            var result = await sender.Send(new GetIdentificationTypesQuery());
            return result.IsValid ? Results.Ok(result.Value) : ResultExtension.ResultToResponse(result);
        })
        .Produces<IReadOnlyList<IdentificationTypeResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);

        group.MapGet("/property-statuses", async (ISender sender) =>
        {
            var result = await sender.Send(new GetPropertyStatusesQuery());
            return result.IsValid ? Results.Ok(result.Value) : ResultExtension.ResultToResponse(result);
        })
        .Produces<IReadOnlyList<PropertyStatusResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound);
    }
}
