using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WalksController : ControllerBase
    {
        private readonly IMapper mapper;
        private readonly IWalkRepositary walkRepositary;

        public WalksController(IMapper mapper, IWalkRepositary walkRepositary)
        {
            this.mapper = mapper;
            this.walkRepositary = walkRepositary;
        }
        //Create Walk
        //POST : /api/walks
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> Create([FromBody] AddWalkRequestDto addWalkRequestDto)
        {
                //Map DTO to Domain Model
                var walkDomainModel = mapper.Map<Walk>(addWalkRequestDto);

                await walkRepositary.CreateAsync(walkDomainModel);

                //Domain Model to DTO
                return Ok(mapper.Map<WalkDto>(walkDomainModel));
        }

        //Get walk
        //GET:/api/walks

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] String? filterOn, [FromQuery] string? filterQuery, 
            [FromQuery] string? sortBy, [FromQuery] bool? isAscending,
            [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 1000)
        {
            var walkDomainModel = await walkRepositary.GetAllAsync(filterOn, filterQuery, sortBy, isAscending ?? true, pageNumber, pageSize);

            return Ok(mapper.Map<List<WalkDto>>(walkDomainModel));
        }

        //Get walk by ID
        //GET:/api/walk/{id}

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var walkDomainModel = await walkRepositary.GetByIdAsync(id);

            if (walkDomainModel == null)
                return NotFound();

            return Ok(mapper.Map<WalkDto>(walkDomainModel));
        }

        //Update Walk by ID
        //PUT:/api/update/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        public async Task<IActionResult> Update([FromRoute] Guid id,[FromBody] UpdateWalkRequestDto updateWalkRequestDto)
        {
                var walkDomainModel = mapper.Map<Walk>(updateWalkRequestDto);

                walkDomainModel = await walkRepositary.UpdateAsync(id, walkDomainModel);

                if (walkDomainModel == null)
                {
                    return NotFound();
                }

                return Ok(mapper.Map<WalkDto>(walkDomainModel));
        }

        //Delete Walk by ID
        //DELETE:/api/update/{id}

        [HttpDelete]
        [Route("{id:Guid}")]

        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var walkDomainModel = await walkRepositary.DeleteAsync(id);

            if(walkDomainModel == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<WalkDto>(walkDomainModel));
        }
    }
}
