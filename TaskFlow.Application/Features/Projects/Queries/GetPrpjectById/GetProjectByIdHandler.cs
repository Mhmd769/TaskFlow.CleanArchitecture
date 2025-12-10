using AutoMapper;
using MediatR;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.ProjectDTOs;
using TaskFlow.Application.Features.Projects.Queries.GetPrpjectById;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Projects.Queries.GetProjectById
{
    public class GetProjectByIdHandler : IRequestHandler<GetProjectByIdQuery, ProjectDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public GetProjectByIdHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ProjectDto> Handle(GetProjectByIdQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"project:{request.ProjectId}";
            var cached = await _cache.GetAsync<ProjectDto>(cacheKey);
            if (cached != null)
                return cached;

            var project = await _unitOfWork.ProjectsWithDetails.GetProjectWithDetailsByIdAsync(request.ProjectId);

            if (project == null)
                throw new NotFoundException("Project", request.ProjectId);

            var dto = _mapper.Map<ProjectDto>(project);
            dto.TaskCount = project.Tasks?.Count ?? 0;

            await _cache.SetAsync(cacheKey, dto, TimeSpan.FromMinutes(5));

            return dto;
        }
    }
}
