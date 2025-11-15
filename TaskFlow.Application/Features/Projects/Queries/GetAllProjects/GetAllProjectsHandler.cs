using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Application.Common;
using TaskFlow.Application.DTOs.ProjectDTOs;
using TaskFlow.Application.DTOs.UserDTOs;
using TaskFlow.Domain.Exceptions;
using TaskFlow.Domain.Interfaces;

namespace TaskFlow.Application.Features.Projects.Queries.GetAllProjects
{
    public class GetAllProjectsHandler : IRequestHandler<GetAllProjectsQuery, List<ProjectDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ICacheService _cache;


        public GetAllProjectsHandler(IUnitOfWork unitOfWork, IMapper mapper , ICacheService cache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _cache = cache;
        }

        public async Task<List<ProjectDto>> Handle(GetAllProjectsQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = "projects:all";

            var cachedProjects = await _cache.GetAsync<List<ProjectDto>>(cacheKey);
            if (cachedProjects != null)
                return cachedProjects;


            var projects = _unitOfWork.Projects.GetAll();
            var projectDtos = await _mapper.ProjectTo<ProjectDto>(projects).ToListAsync(cancellationToken);

            if (!projectDtos.Any())
                throw new AppException("No projects found");

            await _cache.SetAsync(cacheKey, projectDtos, TimeSpan.FromMinutes(5));

            return projectDtos;
        }
    }
}
