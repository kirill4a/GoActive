namespace GoActive.WebApi.Infrastructure.Filters;

internal static class Extensions
{
    internal static RouteHandlerBuilder WithRequestValidation<TRequest>(this RouteHandlerBuilder builder)
        =>
        builder.AddEndpointFilter<ValidationFilter<TRequest>>()
               .ProducesValidationProblem();
}
