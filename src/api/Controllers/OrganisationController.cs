using Core.DTOs;
using Core.Interfaces;
using Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;

namespace src.Controllers
{
    [ApiController]
    [Route("organisations")]
    public class OrganisationsController(IOrganisationService service) : ControllerBase
    {
        // POST /organisations -> 201 Created
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrganisationDto dto, CancellationToken ct)
        {
            var created = await service.CreateAsync(dto, ct);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        // GET /organisations -> 200 OK (with pagination, sort, filter)
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] PaginationParams p, CancellationToken ct)
        {
            var result=await service.GetAllAsync(p,ct);
            return Ok(result);
        }

        // GET /organisations/{id} -> 200 OK/ 404 Not Found
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var res=await service.GetByIdAsync(id,ct);
            return Ok(res);
        }

        // PATCH /organisations/{id} -> 200 OK/ 404 Not Found
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateOrganisationDto dto, CancellationToken ct)
        {
            var updated = await service.UpdateAsync(id, dto, ct);
            return Ok(updated);
        }

        // DELETE /organisations/{id} -> 204 No Content/404 Not Found
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id,CancellationToken ct)
        {
            var deleted=await service.DeleteAsync(id,ct);
            return NoContent();
        }

        // POST /organisaorganisationstion/{id}/archive -> 200 OK/ 404 Not Found
        [HttpPost("{id:guid}/archive")]
        public async Task<IActionResult> Archive(Guid id,CancellationToken ct)
        {
            var archived=await service.ArchiveAsync(id,ct);
            return Ok(archived);
        }
    }
}
