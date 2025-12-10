using AutoMapper;
using MediatR;
using System.Threading;
using System.Threading.Tasks;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.ProjectDTOs;
using TaskFlow.Domain.Entities;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Projects.Command.CreateProject
{
    public class CreateProjectHandler : IRequestHandler<CreateProjectCommand, ProjectDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;

        public CreateProjectHandler(IUnitOfWork unitOfWork, IMapper mapper, ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<ProjectDto> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
        {
            var owner = await _unitOfWork.Users.GetByIdAsync(request.Project.OwnerId);
            if (owner == null) throw new Exception("Owner not found");

            var project = _mapper.Map<Domain.Entities.Project>(request.Project); // ✅ Works now

            await _unitOfWork.Projects.AddAsync(project);
            await _unitOfWork.SaveAsync();

            var dto = _mapper.Map<ProjectDto>(project);

            // Bust list cache and seed single-project cache
            await _cache.RemoveAsync("projects:all");
            await _cache.SetAsync($"project:{dto.Id}", dto, TimeSpan.FromMinutes(5));

            return dto;
        }
    }
}
