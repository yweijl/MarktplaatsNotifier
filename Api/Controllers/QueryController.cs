using Api.Entities;
using Api.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;
using Api.Dtos;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class QueryController : ControllerBase
    {
        private readonly ILogger<QueryController> _logger;
        private readonly DatabaseContext _context;

        public QueryController(ILogger<QueryController> logger, DatabaseContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("list/{userId}")]
        [ProducesResponseType(typeof(List<Query>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetList(long userId)
        {
            var queries = await 
                _context
                .Queries
                .Include(x => x.Advertisements)
                .Where(x => x.UserId == userId)
                .AsNoTracking()
                .ToListAsync();

            return queries is not null
                ? Ok(queries)
                : NotFound();
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(typeof(Query), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(long userId)
        {
            var queries = await
                _context
                .Queries
                .AsNoTracking()
                .Include(x => x.Advertisements)
                .SingleAsync(x => x.UserId == userId);

            return queries is not null
                ? Ok(queries)
                : NotFound();
        }

        [HttpPost]
        [ProducesResponseType(typeof(Query), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] NewQueryDto newQuery)
        {
            try
            {
            var date = DateTime.Now;
            var stateTracker = await _context.Queries.AddAsync(new Query
            {
                CreateDate = date,
                MutationDate = date,
                Name = newQuery.Name,
                Url = newQuery.Url,
                UserId = newQuery.UserId,
                Interval = 1
            });
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(Get), new { userId = stateTracker.Entity.Id }, stateTracker.Entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, newQuery);
                return BadRequest();
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromBody] QueryDto query)
        {
            var entity = await _context.Queries.SingleAsync(x => x.Id == query.Id && x.UserId == query.UserId);
            if (entity == null)
            {
                return BadRequest();
            }

            entity.Name = query.Name;
            entity.Url = query.Url;

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{queryId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Delete(long queryId)
        {
            var query = await _context.Queries.SingleAsync(x => x.Id == queryId);
            
            if (query == null)
            {
                return BadRequest();
            }

            _context.Remove(query);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("scrapeAdvertisements/{id}")]
        [ProducesResponseType(typeof(List<Advertisement>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ScrapeAdverstisements(long userId)
        {
            var queries = await
                _context
                .Queries
                .Include(x => x.Advertisements)
                .Where(x => x.UserId == userId)
                .AsNoTracking()
                .ToListAsync();

            return queries is not null
                ? Ok(queries)
                : NotFound();
        }

    }
}
