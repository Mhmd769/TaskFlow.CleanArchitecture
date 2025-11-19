using System;
using FluentValidation;
using TaskFlow.Application.Features.Tasks.Command.CreateTask;

public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.Task.Title)
            .NotEmpty().WithMessage("Task title is required.")
            .MaximumLength(200).WithMessage("Task title must not exceed 200 characters.");

        RuleFor(x => x.Task.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters.")
            .When(x => !string.IsNullOrEmpty(x.Task.Description));

        RuleFor(x => x.Task.ProjectId)
            .NotEmpty().WithMessage("ProjectId is required.");

        RuleFor(x => x.Task.DueDate)
            .Must(date => date == null || date.Value.ToUniversalTime().Date >= DateTime.UtcNow.Date)
            .WithMessage("Due date cannot be in the past.");
    }
}
