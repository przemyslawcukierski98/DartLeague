using DartLeague.Application.Matches;
using DartLeague.Contracts;
using DartLeague.Infrastructure.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DartLeague.Controllers
{
    [ApiController]
    [Route("api/legs")]
    public class LegController : ControllerBase
    {
        private readonly DartLeagueDbContext _db;

        public LegController(DartLeagueDbContext db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<IActionResult> Create(Guid matchId, [FromBody] CreateLegRequest request)
        {
            var match = await _db.Matches.FindAsync(matchId);

            if(match == null)
            {
                return NotFound("Match not found");
            }

            var leg = new Leg(match, request.Player1Score, request.Player2Score);

            match.Legs.Add(leg);
            await _db.SaveChangesAsync();

            return Ok(leg);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var leg = await _db.Legs
                .Include(l => l.Match)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (leg == null)
                return NotFound($"Leg with ID {id} not found");

            return Ok(leg);
        }

        [HttpGet]
        public async Task<IActionResult> GetFromMatch([FromQuery] Guid matchId)
        {
            var exists = await _db.Matches.AnyAsync(m => m.Id == matchId);

            if (!exists)
                return NotFound($"Match {matchId} not found");

            var legs = await _db.Legs
                .Where(l => l.MatchId == matchId)
                .ToListAsync();

            return Ok(legs);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, int player1Score, int player2Score)
        {
            var leg = await _db.Legs.FindAsync(id);

            if (leg == null)
                return NotFound($"Leg {id} not found");

            leg.UpdateScores(player1Score, player2Score);
            await _db.SaveChangesAsync();

            return Ok(leg);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var leg = await _db.Legs.FindAsync(id);

            if (leg == null)
                return NotFound($"Leg {id} not found");

            _db.Legs.Remove(leg);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
