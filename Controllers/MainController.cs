using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyGameAPI.Models;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace MyGameAPI.Controllers
{
    [Route("api/")]
    [ApiController]
    public class MainController : ControllerBase
    {
        private readonly APIContext _context;
        private readonly IConfiguration _configuration;

        public MainController(APIContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;

            // ------- TEMPORARY CRUTCH FOR DUMMY DATA ------------
            // filling db with dummy data in case there aren't any
            if (_context.Matches.ToList().Count == 0)
            {
                for (uint i = 0; i < 10; i++)
                {
                    Match addedMatch = new Match { 
                        Id = i + 1, 
                        Name = "Match " + (i + 2).ToString(), 
                        NowPlaying = i + 1, 
                        MaxPlayers = i + 1 };
                    _context.Matches.Add(addedMatch);
                    _context.SaveChanges();
                }
            }
            if (_context.Players.ToList().Count == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    Player addedPlayer = new Player { 
                        Id = (i + 1).ToString(), 
                        Nickname = "Player" + (i + 1).ToString(), 
                        Email = "Email" + (i + 1).ToString(), 
                        PasswordHash = "Password" + (i + 1).ToString()
                    };
                    _context.Players.Add(addedPlayer);
                    _context.SaveChanges();
                }
            }
            // -----------------------------------------------------
        }

        // POST: api/login
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Route("login")]
        public async Task<ActionResult<Match>> PostMatch(LoginModel loginData)
        {
            Player playerByNickname = _context.Players.SingleOrDefault(user => user.Nickname == loginData.Nickname);

            if (playerByNickname != null)
            {
                if (loginData.PasswordHash != playerByNickname.PasswordHash)
                {
                    return Unauthorized();
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["JWT:ValidIssuer"],
                    audience: _configuration["JWT:ValidAudience"],
                    expires: DateTime.Now.AddHours(3),
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                    );

                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo
                });
            }

            return Unauthorized();
        }


        // GET: api/Matches
        //[Authorize]
        [HttpGet]
        [Route("matches")]
        public async Task<ActionResult<IEnumerable<Match>>> GetMatches()
        {
            return await _context.Matches.ToListAsync();
        }

        // GET: api/players
        //[Authorize]
        [HttpGet]
        [Route("leaderboards/{game type}")]
        public async Task<ActionResult<IEnumerable<Leader>>> GetLeaderboard()
        {
            return await _context.Players.ToListAsync();
        }

        // GET: api/Matches/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Match>> GetMatch(long id)
        {
            var match = await _context.Matches.FindAsync(id);

            if (match == null)
            {
                return NotFound();
            }

            return match;
        }

        // PUT: api/Matches/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMatch(long id, Match match)
        {
            if (id != match.Id)
            {
                return BadRequest();
            }

            _context.Entry(match).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MatchExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/Matches/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMatch(long id)
        {
            var match = await _context.Matches.FindAsync(id);
            if (match == null)
            {
                return NotFound();
            }

            _context.Matches.Remove(match);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool MatchExists(long id)
        {
            return _context.Matches.Any(e => e.Id == id);
        }
    }
}
