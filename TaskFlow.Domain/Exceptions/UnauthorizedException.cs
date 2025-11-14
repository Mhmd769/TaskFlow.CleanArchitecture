using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Domain.Exceptions
{
    public class UnauthorizedException : AppException
    {
        public UnauthorizedException(string message = "You must log in first.")
            : base(message, StatusCodes.Status401Unauthorized) { }
    }
}
