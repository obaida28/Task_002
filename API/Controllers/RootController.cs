using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Core.Entites;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class RootController<TEntity, TRepository> : ControllerBase
        where TEntity : BaseEntity 
        where TRepository : IGenericRepository<BaseEntity>
    {
        private readonly TRepository repository;
        public RootController(TRepository repository) 
        {
            this.repository = repository;
        }
        // GET: api/[controller]
        [HttpGet]
        public virtual async Task<IActionResult> Get()
        {
            return Ok(await repository.GetAll());
        }

        // GET: api/[controller]/5
        [HttpGet("{id}")]
        public virtual async Task<IActionResult> Get(Guid id)
        {
            var movie = await repository.GetById(id);
            if (movie == null)
            {
                return NotFound();
            }
            return Ok(movie);
        }

        // PUT: api/[controller]/5
        [HttpPut("{id}")]
        public virtual async Task<IActionResult> Put(Guid id, TEntity entity)
        {
            if (id != entity.Id)
            {
                return BadRequest();
            }
            repository.Update(entity);
            return NoContent();
        }

        // POST: api/[controller]
        [HttpPost]
        public virtual async Task<IActionResult> Post(TEntity entity)
        {
            repository.Add(entity);
            return CreatedAtAction("Get", new { id = entity.Id }, entity);
        }

        // DELETE: api/[controller]/5
        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<TEntity>> Delete(Guid id)
        {
            var entity = await repository.Delete(id);
            if (entity == null)
            {
                return NotFound();
            }
            return NoContent();
        }

    }
}