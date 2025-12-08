using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Domain.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Project> Projects { get; }
        IRepository<TaskItem> Tasks { get; }
        IRepository<Comment> Comments { get; }

        IProjectRepository ProjectsWithDetails { get; }


        Task<int> SaveAsync();
    }
}
