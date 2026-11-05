using Core.DTOs;

namespace Core.Interfaces
{
    public interface IOrganisationService
    {
        Task<OrganisationResponseDto> CreateAsync(CreateOrganisationDto dto, CancellationToken ct=default);
        Task<PagedResponse<OrganisationResponseDto>> GetAllAsync(PaginationParams p, CancellationToken ct=default);
        Task<OrganisationResponseDto> GetByIdAsync(Guid id,CancellationToken ct=default);
        Task<OrganisationResponseDto> UpdateAsync(Guid id, UpdateOrganisationDto dto, CancellationToken ct=default);
        Task<bool> DeleteAsync(Guid id, CancellationToken ct=default);
        Task<OrganisationResponseDto> ArchiveAsync(Guid id, CancellationToken ct=default);
    }
}