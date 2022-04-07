using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Music_store_backend.Model
{
    public class CurrentUser
    {
        public UserProfile userProfile { get; set; }
        public String accessToken { get; set; }
        public UserMenu userMenu { get; set; }
    }
}
