using Core.DTOs;
using Core.Exceptions;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("projects")]
    public class ProjectController(IProjectService service) : ControllerBase
    {
        [HttpPost]
        // POST /projects -> 201 OK
        public async Task<IActionResult> Create([FromBody] CreateProjectDTO dto,CancellationToken ct)
        {
            var created = await service.CreateAsync(dto, ct);
            return CreatedAtRoute("GetProjectById", new { id = created.Id }, created);
        }
        
        // GET /projects -> 200 OK
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] ProjectParams p,CancellationToken ct)
        {
            var result = await service.GetAllAsync(p, ct);
            return Ok(result);
        }

        // GET /projects/{id} -> 200 OK / 404
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var project = await service.GetByIdAsync(id, ct);
            return Ok(project);
        }

        // PATCH /projects/{id} -> 200 OK / 404
        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectDTO dto, CancellationToken ct)
        {
            var updated = await service.UpdateAsync(id, dto, ct);
            return Ok(updated);
        }

        // DELETE /projects/{id} -> 204 NoContent / 404
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await service.DeleteAsync(id, ct);
            return NoContent();
        }

        // POST /projects/{id}/clone -> 201 Created (Custom action returning newly created resource)
        [HttpPost("{id:guid}/clone")]
        public async Task<IActionResult> Clone(Guid id, CancellationToken ct)
        {
            var cloned = await service.CloneAsync(id, ct);
            return CreatedAtAction(nameof(GetById), new { id = cloned.Id }, cloned);
        }
    }
}