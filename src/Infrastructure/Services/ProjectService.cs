using System.Runtime.CompilerServices;
using Core.DTOs;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class ProjectService(AppDbContext db) : IProjectService
    {
        public async Task<ProjectResponseDTO> CreateAsync(CreateProjectDTO dto,CancellationToken ct=default)
        {
            var org=await db.Organisations.AnyAsync(o=>o.Id==dto.OrganisationId);
            if (!org)
            {
                throw new NotFoundException($"Organisation with Id: {dto.OrganisationId} does not exist");
            }
            var trimmedName = dto.Name.Trim();
            if (string.IsNullOrWhiteSpace(trimmedName))
            {
                throw new BadRequestException("Project name cannot consist only of whitespace.");
            }
            var project=new Project
            {
                OrganisationId=dto.OrganisationId,
                Name=dto.Name,
                Status=string.IsNullOrWhiteSpace(dto.Status)?"planned":dto.Status.Trim(),
                Description=dto.Description
            };
            db.Projects.Add(project);
            await db.SaveChangesAsync(ct);
            return ToResponseDto(project);
        }

        public async Task<PagedResponse<ProjectResponseDTO>> GetAllAsync(ProjectParams p,CancellationToken ct = default)
        {
            var page = p.Page < 1 ? 1 : p.Page;
            var limit = p.Limit is < 1 or > 100 ? 10 : p.Limit;

            var query = db.Projects.AsNoTracking().AsQueryable();

            // Filters
            if (p.OrganisationId.HasValue)
            {
                query = query.Where(pr => pr.OrganisationId == p.OrganisationId.Value);
            }

            if (!string.IsNullOrWhiteSpace(p.Status))
            {
                query = query.Where(pr => pr.Status.ToLower() == p.Status.ToLower());
            }

            if (!string.IsNullOrWhiteSpace(p.Name))
            {
                query = query.Where(pr => EF.Functions.ILike(pr.Name, $"%{p.Name}%"));
            }

            var total = await query.CountAsync(ct);

            var isAscending = string.Equals(p.OrderBy, "asc", StringComparison.OrdinalIgnoreCase);
            query = (p.EffectiveSortBy.ToLower()) switch
            {
                "name" => isAscending ? query.OrderBy(pr => pr.Name) : query.OrderByDescending(pr => pr.Name),
                "status" => isAscending ? query.OrderBy(pr => pr.Status) : query.OrderByDescending(pr => pr.Status),
                _ => isAscending ? query.OrderBy(pr => pr.CreatedAt) : query.OrderByDescending(pr => pr.CreatedAt)
            };

            var items = await query
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(pr => ToResponseDto(pr))
                .ToListAsync(ct);

            var totalPages = (int)Math.Ceiling(total / (double)limit);

            return new PagedResponse<ProjectResponseDTO>(items, total, page, totalPages);
        }

        public async Task<ProjectResponseDTO> GetByIdAsync(Guid id, CancellationToken ct= default)
        {
            var project=await db.Projects.AsNoTracking().FirstOrDefaultAsync(pr=>pr.Id==id,ct);
            if (project is null)
            {
                throw new NotFoundException($"Project with Id: {id} does not exist");
            }
            return ToResponseDto(project);
        }

        public async Task<ProjectResponseDTO> UpdateAsync(Guid id,UpdateProjectDTO dto, CancellationToken ct = default)
        {
            var project=await db.Projects.FirstOrDefaultAsync(pr=>pr.Id==id,ct);
            if(project is null)
            {
                throw new NotFoundException($"Project with ID '{id}' was not found.");
            }
            if(dto.Name is not null)
            {
                project.Name=dto.Name;
            }
            if(dto.Status is not null)
            {
                project.Status=dto.Status;
            }
            if(dto.Description is not null)
            {
                project.Status=dto.Description;
            }
            project.UpdatedAt=DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
            return ToResponseDto(project);
        }

        public async Task DeleteAsync(Guid id, CancellationToken ct = default)
        {
            var project = await db.Projects.FirstOrDefaultAsync(pr => pr.Id == id, ct);
            if (project is null)
            {
                throw new NotFoundException($"Project with ID '{id}' was not found.");
            }

            db.Projects.Remove(project);
            await db.SaveChangesAsync(ct);
        }

        // Custom Action from Video 11: POST /projects/{id}/clone
        public async Task<ProjectResponseDTO> CloneAsync(Guid id, CancellationToken ct = default)
        {
            var original = await db.Projects.AsNoTracking().FirstOrDefaultAsync(pr => pr.Id == id, ct);
            if (original is null)
            {
                throw new NotFoundException($"Project with ID '{id}' was not found.");
            }

            var cloned = new Project
            {
                OrganisationId = original.OrganisationId,
                Name = $"{original.Name} (Clone)",
                Status = "planned", // Cloned projects reset to planned
                Description = original.Description
            };

            db.Projects.Add(cloned);
            await db.SaveChangesAsync(ct);

            return ToResponseDto(cloned);
        }

        private static ProjectResponseDTO ToResponseDto(Project pr) =>
        new(pr.Id, pr.OrganisationId, pr.Name, pr.Status, pr.Description, pr.CreatedAt, pr.UpdatedAt);
    }
}
