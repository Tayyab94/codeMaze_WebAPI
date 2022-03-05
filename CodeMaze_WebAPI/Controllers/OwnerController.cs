using AutoMapper;
using Contracts;
using Entities;
using Entities.DataSendObjects;
using Entities.DataTransferObjects;
using Entities.RequestEntitiesDTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Dynamic;

namespace CodeMaze_WebAPI.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class OwnerController : ControllerBase
    {
        private readonly ILogger<OwnerController> logger;
        private readonly IRepositoryWrapper repositoryWrapper;
        private readonly IMapper mapper;

        public OwnerController(ILogger<OwnerController> logger,
                IRepositoryWrapper repositoryWrapper, IMapper mapper)
        {
            this.logger = logger;
            this.repositoryWrapper = repositoryWrapper;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllOwners()
        {
            try
            {
                var owners =await repositoryWrapper.OwnerRepository.GetAllOwners();
                logger.LogInformation("Return All Owners from the Database");

                return Ok(owners);
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside GetAllOwners action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        // Paginated List
        //[HttpGet]
        //public IActionResult GetOwners([FromQuery]OwnerParameters ownerParameters)
        //{
        //    var owners = repositoryWrapper.OwnerRepository.GetOwners(ownerParameters);

        //    logger.LogInformation($"Returned {owners.Count()} owners from database.");
        //    return Ok(owners);
        //}

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SendOwnerDto model)
        {
            var owner = mapper.Map<Owner>(model);
            //var owner = new Owner()
            //{
            //    OwnerId = model.OwnerId,
            //    Address = model.Address,
            //    DateOfBirth = model.DateOfBirth,
            //    Name = model.Name
            //};
           repositoryWrapper.OwnerRepository.Create(owner);
            repositoryWrapper.Save();
            return Ok();
        }

        [HttpPost]
        public IActionResult CreateOwner([FromBody] SendOwnerDto model)
        {
            try
            {
                if(model == null)
                {
                    logger.LogError("Owner object sent from client is null.");
                    return BadRequest("Owner object is null");
                }
                if(ModelState.IsValid==false)
                {
                    logger.LogError("Invalid owner object sent from client.");
                    return BadRequest(ModelState);
                }

                var ownerEntity= mapper.Map<Owner>(model);

                repositoryWrapper.OwnerRepository.CreateOwner(ownerEntity);
                repositoryWrapper.Save();

                var createdOwner = mapper.Map<OwnerDto>(ownerEntity);

                return CreatedAtRoute("OwnerById", new { id = createdOwner.Id }, createdOwner);
            }
            catch (Exception)
            {

                throw;
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOwners([FromQuery] OwnerParameters ownerParameters)
        {
            if(!ownerParameters.ValidYearRange)
            {
                return BadRequest("Max year of birth cannot be less than min year of birth");
            }
            var owners =await repositoryWrapper.OwnerRepository.GetOwners(ownerParameters);

            var matadata = new
            {
                owners.TotalCount,
                owners.PageSize,
                owners.CurrentPage,
                owners.TotalPages,
                owners.HasNext,
                owners.HasPrevious
            };

            Response.Headers.Add("X-Pagination", JsonConvert.SerializeObject(matadata));
            logger.LogInformation($"Returned {owners.Count()} owners from database.");
            return Ok(owners);
        }


        [HttpGet("{id}",Name = "OwnerById ")]
        public IActionResult GetOwnerById(Guid id)
        {
            try
            {
                //var owner = repositoryWrapper.OwnerRepository.FindByCondition(s => s.OwnerId == id);

                var owner = repositoryWrapper.OwnerRepository.GetOwnerById(id);
                if (owner==null)
                {
                    logger.LogError("Owner not found");
                    return NotFound();
                }
                else
                {
                    return Ok(owner);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside GetOwnerById action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}", Name = "ByIdOwnerGet")]
        public IActionResult GetOwnerById_1(Guid id, [FromQuery] string fields)
        {
            var owner = repositoryWrapper.OwnerRepository.GetOwnerById(id, fields);
            if (owner == default(ExpandoObject))
            {
                logger.LogError($"Owner with id: {id}, hasn't been found in db.");
                return NotFound();
            }
            return Ok(owner);
        }


        [HttpGet("{id}/account")]
        public IActionResult GetOwnerWithDetail(Guid id)
        {
            try
            {
                var owner = repositoryWrapper.OwnerRepository.GetOwnerWithDetail(id);
                if (owner == null)
                {
                    logger.LogError($"Owner with id: {id}, hasn't been found in db.");
                    return NotFound();
                }
                else
                {
                    var ownerResult = mapper.Map<OwnerDto>(owner);
                    return Ok(ownerResult);
                }
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside GetOwnerWithDetails action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOwner(Guid id, [FromBody] OwnerForUpdateDto owner)
        {
            try
            {
                if (owner == null)
                {
                    logger.LogError("Owner object sent from client is null.");
                    return BadRequest("Owner object is null");
                }


                if (!ModelState.IsValid)
                {
                    logger.LogError("Invalid owner object sent from client.");
                    return BadRequest("Invalid model object");
                }

                var ownerEntity =await repositoryWrapper.OwnerRepository.GetOwnerById(id);

                if (ownerEntity == null)
                {
                    logger.LogError($"Owner with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                mapper.Map(owner, ownerEntity);

                repositoryWrapper.OwnerRepository.UpdateOwner(ownerEntity);
                repositoryWrapper.Save();

                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside UpdateOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOwner(Guid id)
        {
            try
            {
                var owner =await repositoryWrapper.OwnerRepository.GetOwnerById(id);
                if (owner == null)
                {
                    logger.LogError($"Owner with id: {id}, hasn't been found in db.");
                    return NotFound();
                }

                if(repositoryWrapper.AccountRepository.GetAccountByOwner(id).Any())
                {
                    logger.LogError($"Cannot delete owner with id: {id}. It has related accounts. Delete those accounts first");
                    return BadRequest("Cannot delete owner. It has related accounts. Delete those accounts first");
                }

                repositoryWrapper.OwnerRepository.DeleteOwner(owner);
                repositoryWrapper.Save();
                return NoContent();
            }
            catch (Exception ex)
            {
                logger.LogError($"Something went wrong inside DeleteOwner action: {ex.Message}");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
