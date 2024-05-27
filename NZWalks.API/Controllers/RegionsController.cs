using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NZWalks.API.CustomActionFilters;
using NZWalks.API.Data;
using NZWalks.API.Models.Domain;
using NZWalks.API.Models.DTO;
using NZWalks.API.Repositories;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace NZWalks.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegionsController : ControllerBase
    {
        private readonly NZWalksDbContext dbContext;
        private readonly IRegionRepository regionRepository;
        private readonly IMapper mapper;
        private readonly ILogger<RegionsController> logger;

        public RegionsController(NZWalksDbContext dbContext, 
                                 IRegionRepository regionRepository, 
                                 IMapper mapper,
                                 ILogger<RegionsController> logger)
        {
            this.dbContext = dbContext;
            this.regionRepository = regionRepository;
            this.mapper = mapper;
            this.logger = logger;
        }

        //GET ALL DETAILS
        //GET:api/Region
        [HttpGet]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetAll()
        {
            logger.LogInformation("GetAllRegions Action Method was invoked");

            //GET DATA FROM DATABASE - Domain Models
            var regionsDomain = await regionRepository.GetAllAsync();
            
            //MAP Domain Models TO DTO's
            var regionsDto = mapper.Map<List<RegionDto>>(regionsDomain);

            logger.LogInformation($"Finished GetAllRegions request with data: {JsonSerializer.Serialize(regionsDomain)}");

            //RETURN DTO
            return Ok(regionsDto);
        }

        //GET DETAILS BY ID
        //GET:api/Regions/{id}
        [HttpGet]
        [Route("{id:Guid}")]
        //[Authorize(Roles = "Reader")]
        public async Task<IActionResult> GetById(Guid id)
        {
            //GET DATA FROM DATABASE - Domain Models
            var regionsDomain = await regionRepository.GetByIdAsync(id);

            if (regionsDomain == null)
                    return NotFound();

            //MAP Domain Models TO DTO
            /*var regionsDto = new RegionDto
                {
                    Id = regionsDomain.Id,
                    code = regionsDomain.code,
                    name = regionsDomain.name,
                    RegionImageUrl = regionsDomain.RegionImageUrl

                };*/

            //RETURN DTO
            return Ok(mapper.Map<RegionDto>(regionsDomain));
        }

        //POST TO CREATE NEW REGION
        //POST:api/Region
        [HttpPost]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> create([FromBody] AddRegionRequestDto addRegionRequestDto)
        {
                //MAP or Convert DTO TO Region Models
                var regionDomain = mapper.Map<Region>(addRegionRequestDto);


                //Use Domain Model to Create Region
                regionDomain = await regionRepository.createAsync(regionDomain);

                //Map Domain Model Back to DTO
                var regionDto = mapper.Map<RegionDto>(regionDomain);

                return CreatedAtAction(nameof(GetById), new { id = regionDto.Id }, regionDto);
        }

        //PUT Update Details
        //PUT:api/Region/{id}
        [HttpPut]
        [Route("{id:Guid}")]
        [ValidateModel]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> update([FromRoute] Guid id, [FromBody] UpdateRegionRequestDto updateRegionRequestDto)
        {
                //Check If Region Exists
                var regionDomain = mapper.Map<Region>(updateRegionRequestDto);

                regionDomain = await regionRepository.updateAsync(id, regionDomain);

                if (regionDomain == null)
                {
                    return NotFound();
                }

                dbContext.SaveChanges();

                return Ok(mapper.Map<RegionDto>(regionDomain));
        }

        //DELETE Delete Region
        //DELETE:api/Region/{id}
        [HttpDelete]
        [Route("{id:Guid}")]
        //[Authorize(Roles = "Writer")]
        public async Task<IActionResult> delete([FromRoute] Guid id)
        {
                var regionDomain = await regionRepository.deleteAsync(id);

                if (regionDomain == null)
                    return NotFound();

                return Ok();
        }
    }
}
