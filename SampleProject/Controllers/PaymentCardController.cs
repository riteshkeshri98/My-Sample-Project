using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SampleProject.Data;
using SampleProject.Models;
using StackExchange.Redis;
using System.Text.Json;

namespace SampleProject.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentCardController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IDatabase _redis;

        public PaymentCardController(AppDbContext context, IConnectionMultiplexer redis)
        {
            _context = context;
            _redis = redis.GetDatabase();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PaymentCard>>> GetAll()
        {
            string cacheKey = "cards:all";
            var cached = await _redis.StringGetAsync(cacheKey);

            if (cached.HasValue)
            {
                var cards = JsonSerializer.Deserialize<List<PaymentCard>>(cached);
                return Ok(cards);
            }

            var cardsFromDb = await _context.PaymentCards.ToListAsync();
            if (cardsFromDb == null || !cardsFromDb.Any())
                return NotFound();

            await _redis.StringSetAsync(cacheKey, JsonSerializer.Serialize(cardsFromDb));

            return Ok(cardsFromDb);
        }



        [HttpGet("{id}")]
        public async Task<ActionResult<PaymentCard>> Get(int id)
        {
            string cacheKey = $"card:{id}";
            var cached = await _redis.StringGetAsync(cacheKey);

            if (cached.HasValue)
                return Ok(JsonSerializer.Deserialize<PaymentCard>(cached));

            var card = await _context.PaymentCards.FindAsync(id);
            if (card == null) return NotFound();

            await _redis.StringSetAsync(cacheKey, JsonSerializer.Serialize(card));
            return Ok(card);
        }

        [HttpPost]
        public async Task<ActionResult> Create(PaymentCard card)
        {
            _context.PaymentCards.Add(card);
            await _context.SaveChangesAsync();
            await _redis.KeyDeleteAsync("cards:all");

            return CreatedAtAction(nameof(Get), new { id = card.Id }, card);

        }

        [HttpPut("{id}")]
        public async Task<ActionResult> Update(int id, PaymentCard card)
        {
            if (id != card.Id) return BadRequest();

            _context.Entry(card).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            string cacheKey = $"card:{id}";
            await _redis.StringSetAsync(cacheKey, JsonSerializer.Serialize(card));
            await _redis.KeyDeleteAsync("cards:all");

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var card = await _context.PaymentCards.FindAsync(id);
            if (card == null) return NotFound();

            _context.PaymentCards.Remove(card);
            await _context.SaveChangesAsync();

            await _redis.KeyDeleteAsync($"card:{id}");
            await _redis.KeyDeleteAsync("cards:all");

            return NoContent();
        }
    }
}
