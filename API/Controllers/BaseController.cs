using API.DTOs;
using API.Helpers;
using AutoMapper;
using Core.Entites;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseController<TEntity, TRepository> : ControllerBase
        where TEntity : BaseEntity 
        where TRepository : IGenericRepository<TEntity>
    {
        protected readonly TRepository _repository;
        protected readonly IMapper _map;
        public BaseController(TRepository repository , IMapper map) 
        {
            _repository = repository;
            _map = map;
        }
        // GET: api/[controller]
        [HttpGet(template : "All")]
        public async Task<PagingModel<TEntity>> GetAllAsync<T>(T input) 
        where T : PagingModel<TEntity>
        {
            var query = _repository.GetQueryable();
            var searchingResult = query.ApplySearching(input.SearchingColumn, input.SearchingValue);
            int countFilterd = searchingResult.Count();
            var sortingResult = searchingResult.ApplySorting(input.OrderByData);
            var pagingResult = sortingResult.ApplyPaging(input.CurrentPage, input.RowsPerPage , false);
            var finalQuery = await pagingResult.GetResult(input.CurrentPage, input.RowsPerPage , countFilterd);
            return finalQuery;
        }

        // GET: api/[controller]/5
        [HttpGet("{id}")]
        public virtual async Task<T> GetByIdAsync<T>(Guid id) 
    {
        var getOne = await _repository.GetByIdAsync(id);
        var result = _map.Map<T>(getOne);
        return result;
    }

        // PUT: api/[controller]/5
        [HttpPut("{id}")]
        public virtual async Task PutAsync<T>(Guid id, T dto , bool compatibleIds)
        {
            if(!compatibleIds)
                throw new BadHttpRequestException("Object id is not compatible with the pass id");
            var res = _map.Map<TEntity>(dto);
            await _repository.UpdateAsync(res);
        }

        // POST: api/[controller]
        [HttpPost]
        public virtual async Task<TDto> PostAsync<TDtoCreate,TDto>(TDtoCreate dto)
        {
            if (!ModelState.IsValid)
            throw new BadHttpRequestException("Validation failed. Please check the input and correct any errors.");
            var entity = _map.Map<TEntity>(dto);
            await _repository.AddAsync(entity);
            var res = _map.Map<TDto>(entity);
            return res;
        }

        // DELETE: api/[controller]/5
        [HttpDelete("{id}")]
        public virtual async Task DeleteAsync(Guid id)
        {
            var entity = await _repository.GetByIdAsync(id) ?? 
                throw new Exception("Not found , This id is invalid");
            await _repository.DeleteAsync(entity);
        }
    }
}