using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyGameAPI.Models
{
    public class Player : IdentityUser
    {
        // public long Id { get; set; }
        public string Nickname { get; set; }
        // public string Email { get; set; }
        // public string PasswordHash { get; set; }
        public List<PlayerScore> Score { get; set; }
    }
}
