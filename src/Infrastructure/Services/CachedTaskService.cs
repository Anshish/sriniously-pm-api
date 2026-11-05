using System.Text.Json;
using Core.DTOs;
using Core.Interfaces;

namespace Infrastructure.Services
{
    public class CachedTaskService(ITaskService inner,ICacheService cache) : ITaskService
    {
        private static string GetTaskKey(Guid id)
        {
            return $"task:{id}";
        }

        public async Task<TaskResponseDTO> CreateAsync(CreateTaskDTO dto,CancellationToken ct = default)
        {
            var task=await inner.CreateAsync(dto,ct);
            return task;
        }

        public async Task<PagedResponse<TaskResponseDTO>> GetAllAsync(TaskParams p,CancellationToken ct = default)
        {
            var queryHash=Convert.ToBase64String(JsonSerializer.SerializeToUtf8Bytes(p));
            var chacheKey=$"task:list:{queryHash}";
            var cached=await cache.GetAsync<PagedResponse<TaskResponseDTO>>(chacheKey,ct);
            if(cached is not null)
            {
                return cached;
            }
            var result = await inner.GetAllAsync(p, ct);
            await cache.SetAsync(chacheKey,result,TimeSpan.FromMinutes(1),ct);
            return result;
        }

        public async Task<TaskResponseDTO> GetByIdAsync(Guid id,CancellationToken ct = default)
        {
            var cached=await cache.GetAsync<TaskResponseDTO>(GetTaskKey(id),ct);
            if(cached is not null)
            {
                return cached;
            }
            var task=await inner.GetByIdAsync(id,ct);
            await cache.SetAsync(GetTaskKey(id),task,TimeSpan.FromMinutes(10),ct);
            return task;
        }

        public async Task<TaskResponseDTO> UpdateAsync(Guid id,UpdateTaskDTO dto,CancellationToken ct = default)
        {
            var task=await inner.UpdateAsync(id,dto,ct);
            await cache.RemoveAsync(GetTaskKey(id),ct);
            return task;
        }

        public async Task DeleteAsync(Guid id,CancellationToken ct = default)
        {
            await inner.DeleteAsync(id,ct);
            await cache.RemoveAsync(GetTaskKey(id),ct);
        }

        public async Task<TaskResponseDTO> ChangeStatusAsync(Guid id,string newStatus,CancellationToken ct = default)
        {
            var task=await inner.ChangeStatusAsync(id,newStatus,ct);
            await cache.RemoveAsync(GetTaskKey(id),ct);
            return task;
        }
    }
}