using AutoMapper;
using MediatR;
using TaskFlow.Application.DTOs.ProjectDTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Projects.Command.DeleteProjects
{
    public class DeleteProjectHandler : IRequestHandler<DeleteProjectCommand, ProjectDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public DeleteProjectHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<ProjectDto> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            // 1️⃣ Fetch project from DB
            var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId);
            if (project == null)
                throw new Exception("Project not found");

            // 2️⃣ Delete project
            _unitOfWork.Projects.Delete(project);
            await _unitOfWork.SaveAsync();

            // 3️⃣ Return deleted project as DTO
            return _mapper.Map<ProjectDto>(project);
        }
    }
}
