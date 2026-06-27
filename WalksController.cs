using Asp.Versioning;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [ApiVersion("1.0")]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepository walkRepository;

        public WalksController(IMapper mapper, IWalkRepository walkRepository)
        {
            this.mapper = mapper;
            this.walkRepository = walkRepository;
        }

        //Create Walk
        //POST: /api
        //walks
        [ValidateModel]
        [HttpPost]
        [MapToApiVersion("1.0")]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
                //Map DTO to Domain Model
                var walkDomainModel = mapper.Map<Walk>(addWalkRequestDto);

                await walkRepository.CreateAsync(walkDomainModel);

                //Map domain model to dto
                return Ok(mapper.Map<WalkDto>(walkDomainModel));
         }
           
        

        //Get all walks
        //GET: /api/walks?filterOn=Name&filterQuery=Track&sortBy=Name&isAscending=true&pageNumber=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] string? filterOn, [FromQuery] string? filterQuery, [FromQuery] string? sortBy, [FromQuery] bool? isAscending, [FromQuery] int pageNumber=1, [FromQuery] int pageSize=1000)
        {
            var walksDomainModel=await walkRepository.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true, pageNumber,pageSize);

            //Map domain model to DTO
            var walksDto = mapper.Map<List<Walk>>(walksDomainModel);

            return Ok(walksDto);
        }

        //Get a walk by ID
        //GET: /api/walks/{id}
        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetWalkById([FromRoute] Guid id) {
            var walkDomain = await walkRepository.GetWalkByIdAsync(id);
            if (walkDomain == null)
            {
                return NotFound();
            }
            var walkDto = mapper.Map<WalkDto>(walkDomain);
            return Ok(walkDto);
        }
        //Update walk by Id
        //PUT /api/walks/{id}
        [HttpPut]
        [Route("{id:guid}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateWalkById([FromRoute] Guid id, UpdateWalkDTO updateWalkdto)
        {
                var walkDomainModel = mapper.Map<Walk>(updateWalkdto);

                var walk = await walkRepository.UpdateWalkAsync(id, walkDomainModel);

                if (walk == null)
                {
                    return NotFound();
                }
                return Ok(mapper.Map<WalkDto>(walkDomainModel));
            
            
        }

        //Delete walk by Id
        //DELETE /api/walks/{id}
        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteWalkById([FromRoute] Guid id)
        {
            var walk = await walkRepository.DeleteAsync(id);
            if (walk == null)
            {
                return NotFound();
            }
            return Ok(mapper.Map<Walk>(walk));
        }
    }
}
