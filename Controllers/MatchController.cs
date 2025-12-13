using DartLeague.Application.Matches;
using DartLeague.Infrastructure.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DartLeague.Controllers
{
    [ApiController]
    [Route("api/matches")]
    public class MatchController : ControllerBase
    {
        private readonly DartLeagueDbContext _db;

        public MatchController(DartLeagueDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid player1Id, Guid player2Id)
        {
            var player1 = await _db.Players.FindAsync(player1Id);
            var player2 = await _db.Players.FindAsync(player2Id);

            if (player1 == null || player2 == null)
                return NotFound("Player not found");

            var match = new Match(player1, player2);
            _db.Matches.Add(match);
            await _db.SaveChangesAsync();

            return Ok(match);
        }

        [HttpGet("{matchId:guid}")]
        public async Task<IActionResult> GetMatchById(Guid matchId)
        {
            var match = await _db.Matches
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .Include(m => m.Legs)
                .FirstOrDefaultAsync(m => m.Id == matchId);

            if (match == null)
                return NotFound("Match not found");

            return Ok(match);
        }

        [HttpGet("player/{playerId:guid}")]
        public async Task<IActionResult> GetAllMatchesByPlayer(Guid playerId)
        {
            var matches = await _db.Matches
                .Where(m => m.Player1.Id == playerId || m.Player2.Id == playerId)
                .Include(m => m.Player1)
                .Include(m => m.Player2)
                .Include(m => m.Legs)
                .ToListAsync();

            return Ok(matches);
        }

        [HttpDelete("{matchId:guid}")]
        public async Task<IActionResult> Delete(Guid matchId)
        {
            var match = await _db.Matches.FindAsync(matchId);

            if (match == null)
                return NotFound($"Match {matchId} not found");

            _db.Matches.Remove(match);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
