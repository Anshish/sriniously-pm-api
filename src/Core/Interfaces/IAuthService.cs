using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DTOs;

namespace Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDTO> RegisterAsync(RegisterRequestDTO dto,CancellationToken ct=default);
        Task<AuthResponseDTO> LoginAsync(LoginRequestDTO dto,CancellationToken ct=default);
    }
}