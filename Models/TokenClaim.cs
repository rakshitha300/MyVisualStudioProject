using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace cafe_management_system.Models
{
    public class TokenClaim
    {
        public string email { get; set; }

        public string Role { get; set; }
    }
}