using Microsoft.AspNetCore.Http;
using System;

namespace TaskFlow.Domain.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string entity, Guid id)
            : base($"{entity} with id: {id} not found", StatusCodes.Status404NotFound)
        {
        }
    }
}
