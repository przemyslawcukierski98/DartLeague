using DartLeague.Application.Players;
using DartLeague.Contracts;
using DartLeague.Infrastructure.Db;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DartLeague.Controllers
{
    [ApiController]
    [Route("api/players")]
    public class PlayerController : ControllerBase
    {
        private readonly DartLeagueDbContext _db;
        public PlayerController(DartLeagueDbContext db)
        {
            _db = db;
        }

        // GET api/players -> lista zawodników
        [HttpGet]
        public async Task<IActionResult> GetAllPlayers()
        {
            var players = await _db.Players
                .OrderBy(p => p.LastName)
                .ToListAsync();

            return Ok(players);
        }

        // GET api/players/{id} -> pobierz zawodnika po ID
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetPlayerById(Guid id)
        {
            var player = await _db.Players
                .FirstOrDefaultAsync(p => p.Id == id);

            if (player == null)
                return NotFound($"Player {id} not found");

            return Ok(player);
        }

        // POST api/players -> dodaj zawodnika
        [HttpPost("{id:guid}")]
        public async Task<IActionResult> Create([FromBody] CreatePlayerRequest request)
        {
            var player = new Player(request.FirstName, request.LastName);

            _db.Players.Add(player);
            await _db.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetPlayerById),
                new { id = player.Id },
                player
            );
        }

        // DELETE api/players/{id} -> usuń zawodnika
    }
}
