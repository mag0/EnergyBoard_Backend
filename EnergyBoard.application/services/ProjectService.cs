using AutoMapper;
using EnergyBoard.Application.DTOs.request.projects;
using EnergyBoard.Application.DTOs.response.projects;
using EnergyBoard.Application.interfaces;
using EnergyBoard.Domain.entities;
using EnergyBoard.Domain.interfaces;
using System.Data.Common;

namespace EnergyBoard.Application.services;
public class ProjectService(IProjectRepository projectRepository, IMapper mapper) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<int> AddAsync(CreateProjectRequest request, Guid userId)
    {
        var nextPosition = await _projectRepository.GetNextPositionAsync(userId);

        var project = _mapper.Map<Project>(request);

        project.UserId = userId;
        project.Position = nextPosition;
        project.CreatedAt = DateTime.UtcNow;

        await _projectRepository.AddAsync(project);

        return project.Id;
    }

    public async Task<IEnumerable<ProjectResponse>> GetAllAsync(Guid userId)
    {
        var projects = await _projectRepository.GetAllAsync(userId);
        return _mapper.Map<IEnumerable<ProjectResponse>>(projects);
    }

    public async Task<ProjectResponse> GetByIdAsync(int projectId, Guid userId)
    {
        var project = await _projectRepository.GetByIdAsync(projectId, userId)
            ?? throw new KeyNotFoundException("Project not found");
        return _mapper.Map<ProjectResponse>(project);
    }

    public async Task<CompleteProjectResponse> GetCompleteAsync(int projectId, Guid userId)
    {
        var project = await _projectRepository.GetCompleteProjectAsync(projectId, userId)
            ?? throw new KeyNotFoundException("Project not found");

        project.Columns = project.Columns
            .OrderBy(c => c.Position)
            .ToList();

        foreach (var column in project.Columns)
            column.Cards = column.Cards
                .OrderBy(card => card.Position)
                .ToList();

        return _mapper.Map<CompleteProjectResponse>(project);
    }

    public async Task UpdateAsync(int projectId, UpdateProjectRequest request, Guid userId)
    {
        var project = await GetEntityAsync(projectId, userId);

        project.Title = request.Title ?? project.Title;
        project.Description = request.Description ?? project.Description;
        project.UpdatedAt = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(project);
    }

    public async Task DeleteAsync(int projectId, Guid userId)
    {
        var project = await GetEntityAsync(projectId, userId);

        project.IsDeleted = true;
        project.UpdatedAt = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(project);
    }

    public async Task UpdatePositionAsync(int projectId, int newPosition, Guid userId)
    {
        var projects = await _projectRepository.GetAllAsync(userId);

        var projectToMove = projects.FirstOrDefault(p => p.Id == projectId)
            ?? throw new KeyNotFoundException("Project not found");

        projects.Remove(projectToMove);

        var newIndex = Math.Clamp(newPosition - 1, 0, projects.Count);
        projects.Insert(newIndex, projectToMove);

        for (int i = 0; i < projects.Count; i++)
            projects[i].Position = i + 1;

        await _projectRepository.UpdateRangeAsync(projects);
    }

    private async Task<Project> GetEntityAsync(int projectId, Guid userId)
    {
        return await _projectRepository.GetByIdAsync(projectId, userId)
            ?? throw new KeyNotFoundException("Project not found");
    }
}