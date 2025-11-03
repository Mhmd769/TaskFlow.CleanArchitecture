using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Domain.Exceptions
{
    public class NotFoundException : AppException
    {
        public NotFoundException(string entity, object key)
            : base($"{entity} with ID '{key}' was not found.") { }
    }
}
