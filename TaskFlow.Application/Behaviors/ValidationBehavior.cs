using FluentValidation;
using MediatR;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // 🔹 1. Check if there are validators for this request
        if (_validators.Any())
        {
            // 🔹 2. Build a validation context
            var context = new ValidationContext<TRequest>(request);

            // 🔹 3. Run all validators and collect failures
            var failures = _validators
                .Select(v => v.Validate(context))
                .SelectMany(result => result.Errors)
                .Where(f => f != null)
                .ToList();

            // 🔹 4. If there are errors, throw exception
            if (failures.Any())
                throw new ValidationException(failures);
        }

        // 🔹 5. Continue to next behavior (or handler)
        return await next();
    }
}
