using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Domain.Exceptions
{
    public class ConflictException : AppException
    {
        public ConflictException(string message)
            : base(message, StatusCodes.Status409Conflict) { }
    }
}
