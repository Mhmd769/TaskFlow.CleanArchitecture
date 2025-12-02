using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.ProjectDTOs;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Projects.Queries.GetPrpjectById
{
    public class GetProjectByIdHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public GetProjectByIdHandler(IUnitOfWork unitOfWork, IMapper mapper , ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"project:{request.ProjectId}";

            // 1️⃣ Try to read from Redis
            var cachedProject = await _cache.GetAsync<ProjectDto>(cacheKey);

            if (cachedProject != null)
                return cachedProject;
            // 2️⃣ Query DB from Repository
            var project = await _unitOfWork.Projects.GetByIdAsync(request.ProjectId);


            if (project == null)
                throw new NotFoundException("Project", request.ProjectId);

            // ✅ Map and compute custom fields
            var dto = _mapper.Map<ProjectDto>(project);
            dto.Owner.Email = project.Owner.Email;
            dto.TaskCount = project.Tasks?.Count ?? 0;

            // 3️⃣ Store in Redis for 5 minutes
            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));

            return dto;
        }
    }
}
