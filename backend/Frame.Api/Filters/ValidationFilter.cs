using System.Text.Json;
using FluentValidation;
using Frame.Application.Common.Errors;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Frame.Api.Filters;

/// <summary>
/// Runs before every controller action. For each argument that has a
/// FluentValidation validator (e.g. RegisterRequest), validates it and,
/// on failure, throws one AppException holding EVERY field error,
/// e.g. { "email": ["EMAIL_INVALID"], "password": ["PASSWORD_WEAK"] }.
/// Controllers and services therefore always receive valid requests.
/// </summary>
internal sealed class ValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
                continue;

            var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());
            if (context.HttpContext.RequestServices.GetService(validatorType) is not IValidator validator)
                continue;

            var result = await validator.ValidateAsync(
                new ValidationContext<object>(argument),
                context.HttpContext.RequestAborted);

            if (!result.IsValid)
            {
                // Field names in camelCase, to match the JSON the frontend sent.
                var fieldErrors = result.Errors
                    .GroupBy(e => JsonNamingPolicy.CamelCase.ConvertName(e.PropertyName))
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorCode).Distinct().ToArray());

                throw AppException.Validation(fieldErrors);
            }
        }

        await next();
    }
}
