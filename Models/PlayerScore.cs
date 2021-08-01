using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyGameAPI.Models
{
    public class PlayerScore
    {
        public long Id { get; set; }
        public Dictionary<string, int> ScorePerGameMode { get; set; }
    }
}
