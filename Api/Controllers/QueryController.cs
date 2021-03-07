using Core.Entities;
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
using Runner.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Produces("application/json")]
    public class QueryController : ControllerBase
    {
        private readonly ILogger<QueryController> _logger;
        private readonly DatabaseContext _context;
        private readonly IScraper _scraper;

        public QueryController(ILogger<QueryController> logger, DatabaseContext context, IScraper scraper)
        {
            _logger = logger;
            _context = context;
            _scraper = scraper;
        }

        [HttpGet("list")]
        [ProducesResponseType(typeof(List<Query>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public async Task<IActionResult> GetList()
        {
            var queries = await
                _context
                .Queries
                .Include(x => x.Advertisements)
                .AsNoTracking()
                .Where(x => x.UserId == GetUserID())
                .ToListAsync();

            return queries is not null
                ? Ok(queries)
                : NotFound();
        }

        [HttpGet("{queryId}")]
        [ProducesResponseType(typeof(Query), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(long queryId)
        {
            var queries = await
                _context
                .Queries
                .AsNoTracking()
                .Include(x => x.Advertisements)
                .SingleAsync(x => x.Id == queryId && x.UserId == GetUserID());

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
                    UserId = GetUserID(),
                    Interval = 1
                });
                await _context.SaveChangesAsync();
                return CreatedAtAction(nameof(Get), new { queryId = stateTracker.Entity.Id }, stateTracker.Entity);
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
            var entity = await _context.Queries.SingleAsync(x => x.Id == query.Id && x.UserId == GetUserID());
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
            var query = await _context.Queries.SingleAsync(x => x.Id == queryId && x.UserId == GetUserID());

            if (query == null)
            {
                return BadRequest();
            }

            _context.Remove(query);
            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("scrapeAdvertisements/{queryId}")]
        [ProducesResponseType(typeof(List<Advertisement>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ScrapeAdverstisements(long queryId)
        {
            try
            {
                var queryUrl = await
                _context
                .Queries
                .AsNoTracking()
                .Where(x => x.Id == queryId && x.UserId == GetUserID())
                .Select(x => x.Url).ToListAsync();

                if (queryUrl.Count == 0)
                {
                    return BadRequest();
                }

                var newAddvertisements = await _scraper.RunAsync(queryUrl.First());

                if (newAddvertisements.Count == 0)
                {
                    return Ok(newAddvertisements);
                }
                newAddvertisements.ForEach(x => x.QueryId = queryId);

                await _context.Advertisements.AddRangeAsync(newAddvertisements);

                await _context.SaveChangesAsync();

                return Ok(newAddvertisements);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while scraping", new { queryId });
                return BadRequest();
            }
        }

        private long GetUserID() => long.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
    }
}
