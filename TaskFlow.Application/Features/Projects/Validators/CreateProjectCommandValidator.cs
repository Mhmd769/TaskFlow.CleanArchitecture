using FluentValidation;
using TaskFlow.Application.Features.Projects.Command.CreateProject;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.Project.Name)
            .NotEmpty().WithMessage("Project name is required.")
            .MaximumLength(150).WithMessage("Project name must not exceed 150 characters.");

        RuleFor(x => x.Project.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Project.Description));

        RuleFor(x => x.Project.OwnerId)
            .NotEmpty().WithMessage("OwnerId is required.");
    }
}
