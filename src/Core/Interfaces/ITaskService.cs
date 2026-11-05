using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DTOs;

namespace Core.Interfaces
{
    public interface ITaskService
    {
        Task<TaskResponseDTO> CreateAsync(CreateTaskDTO dto,CancellationToken ct=default);
        Task<PagedResponse<TaskResponseDTO>> GetAllAsync(TaskParams p,CancellationToken ct=default);
        Task<TaskResponseDTO> UpdateAsync(Guid id, UpdateTaskDTO dto,CancellationToken ct=default);
        Task<TaskResponseDTO> GetByIdAsync(Guid id,CancellationToken ct=default);
        Task DeleteAsync(Guid id,CancellationToken ct=default);
        Task<TaskResponseDTO> ChangeStatusAsync(Guid id,string newStatus,CancellationToken ct=default);
    }
}