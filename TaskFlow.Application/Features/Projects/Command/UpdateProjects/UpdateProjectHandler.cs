using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.ProjectDTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Projects.Command.UpdateProjects
{
    public class UpdateProjectHandler : IRequestHandler<UpdateProjectCommand, ProjectDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache; 

        public UpdateProjectHandler(IUnitOfWork unitOfWork, IMapper mapper ,  ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ProjectDto> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
        {
            // 1️⃣ Fetch the existing project
            var project = await _unitOfWork.Projects.GetByIdAsync(request.Project.Id);
            if (project == null)
                throw new NotFoundException("Project", request.Project.Id);

            // 2️⃣ Optional: check if the new owner exists
            var owner = await _unitOfWork.Users.GetByIdAsync(request.Project.OwnerId);
            if (owner == null)
                throw new NotFoundException("User", request.Project.OwnerId);

            // 3️⃣ Map updated fields from DTO → Entity
            _mapper.Map(request.Project, project);

            // 4️⃣ Save changes
            _unitOfWork.Projects.Update(project);
            await _unitOfWork.SaveAsync();

            // ❗Invalidate cache
            string cacheById = $"project:{request.Project.Id}";
            string cacheAll = "projects:all";

            await _cache.RemoveAsync(cacheById);
            await _cache.RemoveAsync(cacheAll);

            // 5️⃣ Return updated Project as DTO
            return _mapper.Map<ProjectDto>(project);
        }
    }
}
