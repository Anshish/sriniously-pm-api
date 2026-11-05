using Core.DTOs;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IProjectService
    {
        Task<ProjectResponseDTO> CreateAsync(CreateProjectDTO dto,CancellationToken ct=default);
        Task<PagedResponse<ProjectResponseDTO>> GetAllAsync(ProjectParams p,CancellationToken ct=default);
        Task<ProjectResponseDTO> GetByIdAsync(Guid id,CancellationToken ct=default);
        Task<ProjectResponseDTO> UpdateAsync(Guid id,UpdateProjectDTO dto, CancellationToken ct=default);
        Task DeleteAsync(Guid id, CancellationToken ct=default);
        Task<ProjectResponseDTO> CloneAsync(Guid id,CancellationToken ct=default);
    }
}