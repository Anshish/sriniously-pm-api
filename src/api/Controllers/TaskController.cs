using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.DTOs;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace api.Controllers
{
    [ApiController]
    [Route("tasks")]
    public class TaskController(ITaskService service) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateTaskDTO dto,CancellationToken ct = default)
        {
            var created=await service.CreateAsync(dto,ct);
            return CreatedAtRoute("GetTasksById",new{id=created.Id},created);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TaskParams p,CancellationToken ct = default)
        {
            var result=await service.GetAllAsync(p,ct);
            return Ok(result);
        }

        [HttpGet("{id:guid}",Name="GetTasksById")]
        public async Task<IActionResult> GetTasksById(Guid id,CancellationToken ct = default)
        {
            var result=await service.GetByIdAsync(id,ct);
            return Ok(result);
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDTO dto, CancellationToken ct)
        {
            var updated = await service.UpdateAsync(id, dto, ct);
            return Ok(updated);
        }

        // DELETE /tasks/{id} -> 204 NoContent (or 404 via middleware)
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            await service.DeleteAsync(id, ct);
            return NoContent();
        }

        // POST /tasks/{id}/status -> 200 OK (or 400 if invalid transition)
        [HttpPost("{id:guid}/status")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] string newStatus, CancellationToken ct)
        {
            var updated = await service.ChangeStatusAsync(id, newStatus, ct);
            return Ok(updated);
        }

    }
}