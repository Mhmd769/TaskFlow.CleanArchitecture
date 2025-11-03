using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Domain.Exceptions
{

    public class UnexpectedException : AppException
    {
        public UnexpectedException() : base("An unexpected error occurred.") { }
    }
}
