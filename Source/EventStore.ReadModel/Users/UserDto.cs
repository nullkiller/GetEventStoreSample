using Documently.ReadModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventStore.ReadModel
{
    public class UserDto: Dto
    {
        public string Login { get; set; }

        public string Password { get; set; }
    }
}
