using AutoMapper;
using MediatR;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.ProjectDTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Projects.Command.DeleteProjects
{
    public class DeleteProjectHandler : IRequestHandler<DeleteProjectCommand, ProjectDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;


        public DeleteProjectHandler(IUnitOfWork unitOfWork, IMapper mapper , ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ProjectDto> Handle(DeleteProjectCommand request, CancellationToken cancellationToken)
        {
            // 1️⃣ Fetch project from DB
            var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId);
            if (project == null)
                throw new NotFoundException("Project", request.ProjectId);

            // 2️⃣ Delete project
            _unitOfWork.Projects.Delete(project);
            await _unitOfWork.SaveAsync();

            // ❗Invalidate cache
            string cacheById = $"project:{request.ProjectId}";
            string cacheAll = "projects:all";

            await _cache.RemoveAsync(cacheById);
            await _cache.RemoveAsync(cacheAll);

            // 3️⃣ Return deleted project as DTO
            return _mapper.Map<ProjectDto>(project);
        }
    }
}
