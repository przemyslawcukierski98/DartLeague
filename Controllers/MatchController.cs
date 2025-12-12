using DartLeague.Application.Matches;
using DartLeague.Infrastructure.Db;
using Microsoft.AspNetCore.Mvc;

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
                return BadRequest("Player not found");

            var match = new Match(player1, player2);
            _db.Matches.Add(match);
            await _db.SaveChangesAsync();

            return Ok(match);
        }
    }
}
