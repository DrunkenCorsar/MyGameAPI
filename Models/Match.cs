using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyGameAPI.Models
{
    public class Match
    { 
        public long Id { get; set; }
        public string Name { get; set; }
        public uint MaxPlayers { get; set; }
        public uint NowPlaying { get; set; }
    }
}
