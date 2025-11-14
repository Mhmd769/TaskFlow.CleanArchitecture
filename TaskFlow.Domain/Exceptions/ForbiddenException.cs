using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Domain.Exceptions
{
    public class ForbiddenException : AppException
    {
        public ForbiddenException(string message = "You are not allowed to access this resource.")
            : base(message, StatusCodes.Status403Forbidden) { }
    }
}
