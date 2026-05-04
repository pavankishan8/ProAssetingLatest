using ProAssetin.API.Models.DTOs;

namespace ProAssetin.API.Services
{
    public interface IProjectService
    {
        Task<(IEnumerable<ProjectDto> Items, int TotalCount)> GetProjectsAsync(ProjectQueryDto query, string tenantId);
        Task<ProjectDto?> GetProjectByIdAsync(int id, string tenantId);
        Task<ProjectDto> CreateProjectAsync(CreateProjectDto dto, string tenantId, string userId);
        Task<ProjectDto?> UpdateProjectAsync(int id, UpdateProjectDto dto, string tenantId);
        Task<bool> DeleteProjectAsync(int id, string tenantId);
        Task<IEnumerable<string>> GetStatusesAsync();
        Task<IEnumerable<string>> GetPrioritiesAsync();
    }
}
