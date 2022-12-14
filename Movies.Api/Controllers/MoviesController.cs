using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movies.Api.Data;
using Movies.Api.Models;
using System.Net;

namespace Movies.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class MoviesController : ControllerBase
    {
        public readonly MoviesContext _moviesContext;

        public MoviesController(MoviesContext moviesContext)
        {
            _moviesContext = moviesContext;
        }

        [HttpGet("GetMovies")]
        public async Task<ActionResult<IEnumerable<Movie>>> GetMovies()
        {
            return await _moviesContext.Movies.ToListAsync();
        }

        [HttpGet("GetMovie/{id}")]
        public async Task<ActionResult<Movie>> GetMovie(int id)
        {
            var movie = await _moviesContext.Movies.FindAsync(id);

            if (movie == null)
            {
                return NotFound();
            }
            return movie;
        }

        [HttpPut("PutMovie/{id}")]
        public async Task<ActionResult<Movie>> PutMovie(int id, [FromBody] Movie movie)
        {
            if (id != movie.Id)
            {
                return BadRequest();
            }
            _moviesContext.Entry(movie).State = EntityState.Modified;
            var updateMovie = _moviesContext.Movies.Update(movie);
            try
            {
                await _moviesContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MovieExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
        }

        // POST: api/Movies
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Movie>> PostMovie([FromBody] Movie movie)
        {
            _moviesContext.Movies.Add(movie);
            await _moviesContext.SaveChangesAsync();
            return CreatedAtAction("GetMovie", new { id = movie.Id }, movie);
        }

        // DELETE: api/Movies/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Movie>> DeleteMovie(int id)
        {
            var movie = await _moviesContext.Movies.FindAsync(id);
            if (movie == null)
            {
                return NotFound();
            }
            _moviesContext.Movies.Remove(movie);
            await _moviesContext.SaveChangesAsync();
            return movie;
        }

        private bool MovieExists(int id)
        {
            return _moviesContext.Movies.Any(e => e.Id == id);
        }

    }
}