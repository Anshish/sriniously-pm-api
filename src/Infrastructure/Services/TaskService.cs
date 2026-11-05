using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Core.DTOs;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class TaskService(AppDbContext db):ITaskService
    {
        public async Task<TaskResponseDTO> CreateAsync(CreateTaskDTO dto,CancellationToken ct = default)
        {
            var projectExists=await db.Projects.AnyAsync(p=>p.Id==dto.ProjectId,ct);
            if (!projectExists)
            {
                throw new NotFoundException($"Project with ID '{dto.ProjectId}' does not exist.");            
            }
            var trimmedTitle=dto.Title;
            if (string.IsNullOrWhiteSpace(trimmedTitle))
            {
                throw new BadRequestException("Task title cannot consist only of whitespace.");            
            }
            var task=new TaskItem
            {
                ProjectId=dto.ProjectId,
                Title=dto.Title.Trim(),
                Description=dto.Description?.Trim(),
                Status=string.IsNullOrWhiteSpace(dto.Status?.Trim())?"todo":dto.Status.Trim().ToLower(),
                Priority=string.IsNullOrWhiteSpace(dto.Priority?.Trim())?"todo":dto.Priority.Trim().ToLower(),
                DueDate=dto.DueDate
            };
            db.Tasks.Add(task);
            await db.SaveChangesAsync(ct);
            return ToResponseDto(task);
        }

        public async Task<PagedResponse<TaskResponseDTO>> GetAllAsync(TaskParams p,CancellationToken ct = default)
        {
            var page=p.Page<1?1:p.Page;
            var limit=p.Limit is <1 or >100?10:p.Limit;
            var query=db.Tasks.AsQueryable().AsNoTracking();
            if (p.ProjectId.HasValue)
            {
                var targetProjectId = p.ProjectId.Value;
                query = query.Where(t => t.ProjectId == targetProjectId);
            }

            if (!string.IsNullOrWhiteSpace(p.Status))
            {
                var status = p.Status.Trim().ToLower();
                query = query.Where(t => t.Status.ToLower() == status);
            }

            if (!string.IsNullOrWhiteSpace(p.Priority))
            {
                var priority = p.Priority.Trim().ToLower();
                query = query.Where(t => t.Priority.ToLower() == priority);
            }
            if (!string.IsNullOrWhiteSpace(p.Name))
            {
                var searchTerm = $"%{p.Name.Trim()}%";
                query = query.Where(t => EF.Functions.ILike(t.Title, searchTerm));
            }
            var total=await query.CountAsync(ct);
            var isAscending=string.Equals(p.SortOrder,"asc",StringComparison.OrdinalIgnoreCase);
            
            var sortBy=(p.SortBy ?? "createdat").Trim().ToLower();
            switch (p.SortBy)
            {
                case "title":
                    query=isAscending?query.OrderBy(t=>t.Title):query.OrderByDescending(t=>t.Title);
                break;
                case "status":
                    query=isAscending?query.OrderBy(t=>t.Status):query.OrderByDescending(t=>t.Status);
                break;
                case "priority":
                    query=isAscending?query.OrderBy(t=>t.Priority):query.OrderByDescending(t=>t.Priority);
                break;
                case "duedate":
                    query=isAscending?query.OrderBy(t=>t.DueDate):query.OrderByDescending(t=>t.DueDate);
                break;
                default:
                    query=isAscending?query.OrderBy(t=>t.CreatedAt):query.OrderByDescending(t=>t.CreatedAt);
                break;
            }
            var items=await query.Skip((page-1)*limit)
                .Take(limit)
                .Select(t=>ToResponseDto(t))
                .ToListAsync(ct);
            var totalPages = (int)Math.Ceiling(total / (double)limit);

            return new PagedResponse<TaskResponseDTO>(items, total, page, totalPages);
        }

        public async Task<TaskResponseDTO> UpdateAsync(Guid id,UpdateTaskDTO dto,CancellationToken ct = default)
        {
            var task=await db.Tasks.FirstOrDefaultAsync(t=>t.Id==id,ct);
            if(task is null)
            {
                throw new NotFoundException($"Task with ID '{id}' was not found.");
            }
            
            if(dto.Title is not null)
            {
                string trimmed=dto.Title.Trim();
                if (string.IsNullOrWhiteSpace(trimmed))
                {
                    throw new BadRequestException("Task title cannot consist only of whitespace.");
                }
                task.Title=trimmed;
            }
            if(dto.Status is not null)
            {
                task.Status=dto.Status.ToLower();
            }
            if(dto.Priority is not null)
            {
                task.Priority=dto.Priority.ToLower();
            }
            if(dto.Description is not null)
            {
                task.Description=dto.Description.Trim();
            }
            if(dto.DueDate.HasValue)
            {
                task.DueDate=dto.DueDate.Value.ToUniversalTime();
            }

            task.UpdatedAt=DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
            return ToResponseDto(task);
        }

        public async Task<TaskResponseDTO> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var task = await db.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id, ct);
            if (task is null)
            {
                throw new NotFoundException($"Task with ID '{id}' was not found.");
            }

            return ToResponseDto(task);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == id, ct);
            if (task is null)
            {
                throw new NotFoundException($"Task with ID '{id}' was not found.");
            }

            db.Tasks.Remove(task);
            await db.SaveChangesAsync(ct);
        }

        public async Task<TaskResponseDTO> ChangeStatusAsync(Guid id,string newStatus,CancellationToken ct = default)
        {
            var task=await db.Tasks.FirstOrDefaultAsync(t=>t.Id==id,ct);
            if(task is null)
            {
                throw new NotFoundException($"Task with ID '{id}' was not found.");
            }

            var validStatus=new[] { "todo", "in_progress", "completed", "cancelled" };
            if (!validStatus.Contains(newStatus.ToLower().Trim()))
            {
                throw new BadRequestException($"Invalid status '{newStatus}'. Allowed: 'todo', 'in_progress', 'completed', 'cancelled'.");
            }

            task.Status=newStatus.ToLower().Trim();
            task.UpdatedAt=DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
            return ToResponseDto(task);
        }

        private static TaskResponseDTO ToResponseDto(TaskItem t)=>
            new(t.Id, t.ProjectId, t.Title, t.Description, t.Status, t.Priority, t.DueDate, t.CreatedAt, t.UpdatedAt);    
    }
}