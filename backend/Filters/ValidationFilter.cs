using System.ComponentModel.DataAnnotations;

namespace SimpleTaskBackend.Filters;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var argument = context.Arguments.OfType<T>().FirstOrDefault();

        if (argument is not null)
        {
            var validationContext = new ValidationContext(argument);
            var validationResults = new List<ValidationResult>();

            if (!Validator.TryValidateObject(argument, validationContext, validationResults, true))
            {
                var errors = validationResults
                    .GroupBy(e => e.MemberNames.FirstOrDefault() ?? "Error")
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage ?? "Invalid value").ToArray()
                    );

                return Results.ValidationProblem(errors);
            }
        }

        return await next(context);
    }
}
