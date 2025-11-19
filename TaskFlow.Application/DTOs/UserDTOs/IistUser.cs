using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskFlow.Application.DTOs.UserDTOs
{
    public class IistUser
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
    }
}
