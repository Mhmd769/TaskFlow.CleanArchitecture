using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Interfaces
{
    public interface IProjectRepository : IRepository<Project>
    {
        Task<Project?> GetProjectWithDetailsByIdAsync(Guid id);
        Task<IQueryable<Project>> GetAllProjectsWithDetailsAsync();
    }
}
