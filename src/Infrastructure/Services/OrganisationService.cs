using Core.DTOs;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class OrganisationService(AppDbContext db) : IOrganisationService
    {
        public async Task<OrganisationResponseDto> CreateAsync(CreateOrganisationDto dto, CancellationToken ct = default)
        {
            var trimmedName = dto.Name.Trim();
            if (string.IsNullOrWhiteSpace(trimmedName))
            {
                throw new BadRequestException("Organisation name cannot consist only of whitespace.");
            }
            var org=new Organisation
            {
                Name = dto.Name!,
                Status=string.IsNullOrWhiteSpace(dto.Status)?"active":dto.Status.Trim(),
                Description=dto.Description
            };

            db.Organisations.Add(org);
            await db.SaveChangesAsync(ct);
            return ToResponseDto(org);
        }

        public async Task<PagedResponse<OrganisationResponseDto>> GetAllAsync(PaginationParams p,CancellationToken ct=default)
        {
            var page=p.Page<1?1:p.Page;
            var limit=p.Limit is < 1 or > 100? 1: p.Limit;
            var query=db.Organisations.AsNoTracking().AsQueryable();
            if (!string.IsNullOrWhiteSpace(p.Status))
            {
                query=query.Where(o=>o.Status.ToLower()==p.Status.ToLower());
            }
            if (!string.IsNullOrWhiteSpace(p.Name))
            {
                query=query.Where(o=>EF.Functions.ILike(o.Name,$"%{p.Name}%"));
            }
            var total=await query.CountAsync(ct);
            var isAscending=string.Equals(p.SortOrder,"asc",StringComparison.OrdinalIgnoreCase);
            query = (p.SortBy?.ToLower()) switch
            {
                "name"=>isAscending?query.OrderBy(o=>o.Name):query.OrderByDescending(o=>o.Name),
                "status"=>isAscending?query.OrderBy(o=>o.Status):query.OrderByDescending(o=>o.Name),
                _=>isAscending?query.OrderBy(o=>o.CreatedAt):query.OrderByDescending(o=>o.Name)
            };
            var items=await query
                .Skip((page-1)*limit)
                .Take(limit)
                .Select(o=>ToResponseDto(o))
                .ToListAsync(ct);
            var totalPages=(int)Math.Ceiling(total/(double)limit);
            return new PagedResponse<OrganisationResponseDto>(items,total,page,totalPages);
        }

        public async Task<OrganisationResponseDto> GetByIdAsync(Guid id,CancellationToken ct)
        {
            var org=await db.Organisations.AsNoTracking().FirstOrDefaultAsync(o=>o.Id==id,ct);
            if (org is null)
            {
                throw new NotFoundException($"Organisation with ID '{id}' was not found.");
            }
            return ToResponseDto(org);
        }

        public async Task<OrganisationResponseDto> UpdateAsync(Guid id,UpdateOrganisationDto dto,CancellationToken ct = default)
        {
            var org=await db.Organisations.FirstOrDefaultAsync(o=>o.Id==id);
            if (org is null)
            {
                throw new NotFoundException($"Organisation with ID '{id}' was not found.");
            }
            if(dto.Name is not null)
            {
                org.Name=dto.Name;
            }
            if(dto.Status is not null)
            {
                org.Status=dto.Status;
            }
            if(dto.Description is not null)
            {
                org.Description=dto.Description;
            }
            org.UpdatedAt=DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
            return ToResponseDto(org);
        }

        public async Task<bool> DeleteAsync(Guid id,CancellationToken ct=default)
        {
            var org=await db.Organisations.FirstOrDefaultAsync(o=>o.Id==id);
            if (org is null)
            {
                throw new NotFoundException($"Organisation with ID '{id}' was not found.");
            }
            db.Organisations.Remove(org);
            await db.SaveChangesAsync(ct);
            return true;
        }

        public async Task<OrganisationResponseDto> ArchiveAsync(Guid id,CancellationToken ct = default)
        {
            var org=await db.Organisations.FirstOrDefaultAsync(o=>o.Id==id);
            if (org is null)
            {
                throw new NotFoundException($"Organisation with ID '{id}' was not found.");
            }
            org.Status="archive";
            org.UpdatedAt=DateTime.UtcNow;
            await db.SaveChangesAsync(ct);
            return ToResponseDto(org);
        }

        private static OrganisationResponseDto ToResponseDto(Organisation org) =>
        new(org.Id, org.Name, org.Status, org.Description, org.CreatedAt, org.UpdatedAt);
    }
}