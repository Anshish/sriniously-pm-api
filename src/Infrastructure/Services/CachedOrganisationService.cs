using Core.DTOs;
using Core.Interfaces;

namespace Infrastructure.Services
{
    public class CachedOrganisationService(IOrganisationService innerService,ICacheService cache) : IOrganisationService
    {
        private static string CacheKey(Guid id) => $"organisations:{id}";
        public async Task<OrganisationResponseDto> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            var key=CacheKey(id);
            var cachedOrg=await cache.GetAsync<OrganisationResponseDto>(key,ct);
            if(cachedOrg is not null)
            {
                return cachedOrg;
            }
            var org=await innerService.GetByIdAsync(id,ct);
            if(org is not null)
            {
                await cache.SetAsync(key,org,TimeSpan.FromMinutes(5),ct);
            }
            return org!;
        }

        public async Task<OrganisationResponseDto> UpdateAsync(Guid id,UpdateOrganisationDto dto,CancellationToken ct = default)
        {
            var updated=await innerService.UpdateAsync(id,dto,ct);
            await cache.RemoveAsync(CacheKey(id),ct);
            return updated;
        }

        public async Task<bool> DeleteAsync(Guid id,CancellationToken ct = default)
        {
            var deleted=await innerService.DeleteAsync(id,ct);
            await cache.RemoveAsync(CacheKey(id),ct);
            return deleted;
        }

        public async Task<OrganisationResponseDto> ArchiveAsync(Guid id, CancellationToken ct = default)
        {
            var archived = await innerService.ArchiveAsync(id, ct);
            await cache.RemoveAsync(CacheKey(id), ct);
            return archived;
        }

        public async Task<OrganisationResponseDto> CreateAsync(CreateOrganisationDto dto, CancellationToken ct = default)
        {
            return await innerService.CreateAsync(dto, ct);
        }

        public async Task<PagedResponse<OrganisationResponseDto>> GetAllAsync(PaginationParams p, CancellationToken ct = default)
        {
            return await innerService.GetAllAsync(p, ct);
        }
    }
}
