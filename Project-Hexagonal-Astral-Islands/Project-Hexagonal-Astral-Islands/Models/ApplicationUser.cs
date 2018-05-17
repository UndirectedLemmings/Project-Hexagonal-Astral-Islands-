using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Project_Hexagonal_Astral_Islands;

namespace Project_Hexagonal_Astral_Islands.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public long my_map_id { get; set; }
        public int mana { get; set; }
    }
}
