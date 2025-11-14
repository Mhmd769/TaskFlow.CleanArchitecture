using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Domain.Exceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException(string message)
            : base(message, StatusCodes.Status400BadRequest) { }
    }
}
