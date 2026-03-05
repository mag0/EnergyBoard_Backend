using AutoMapper;
using EnergyBoard.Application.DTOs.request.projects;
using EnergyBoard.Application.DTOs.response.projects;
using EnergyBoard.Application.interfaces;
using EnergyBoard.Domain.entities;
using EnergyBoard.Domain.interfaces;

namespace EnergyBoard.Application.services;
public class ProjectService(IProjectRepository projectRepository, IMapper mapper) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IMapper _mapper = mapper;

    public async Task<int> AddProjectAsync(CreateProjectRequest request, Guid userId)
    {
        var projects = await _projectRepository.GetAllAsync(userId);

        var nextPosition = projects.Any()
            ? projects.Max(p => p.Position) + 1
            : 1;

        var project = _mapper.Map<Project>(request);

        project.UserId = userId;
        project.Position = nextPosition;
        project.CreatedAt = DateTime.UtcNow;

        await _projectRepository.AddAsync(project);

        return project.Id;
    }

    public async Task<IEnumerable<ProjectResponse>> GetAllProjectsAsync(Guid userId)
    {
        var projects = await _projectRepository.GetAllAsync(userId);
        return projects.Select(p => _mapper.Map<ProjectResponse>(p));
    }

    public async Task<ProjectResponse?> GetProjectByIdAsync(int id, Guid userId)
    {
        var project = await GetExistingProject(id, userId);
        return _mapper.Map<ProjectResponse>(project);
    }

    public async Task<CompleteProjectResponse?> GetProjectCompleteAsync(int id, Guid userId)
    {
        var project = await _projectRepository.GetCompleteProjectAsync(id, userId)
            ?? throw new KeyNotFoundException("Project not found");

        return _mapper.Map<CompleteProjectResponse>(project);
    }

    public async Task UpdateProjectAsync(int id, UpdateProjectRequest request, Guid userId)
    {
        var project = await GetExistingProject(id, userId);

        if (request.Title != null)
            project.Title = request.Title;

        if (request.Description != null)
            project.Description = request.Description;

        project.UpdatedAt = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(project);
    }

    public async Task DeleteProjectAsync(int id, Guid userId)
    {
        var project = await GetExistingProject(id, userId);

        project.IsDeleted = true;
        project.UpdatedAt = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(project);
    }

    public async Task UpdateProjectPositionAsync(int id, MoveProjectRequest request, Guid userId)
    {
        var projects = (await _projectRepository.GetAllAsync(userId)).ToList();

        var projectToMove = projects.FirstOrDefault(p => p.Id == id)
            ?? throw new KeyNotFoundException("Project not found");

        projects.Remove(projectToMove);

        var newIndex = request.NewPosition;

        if (newIndex < 0)
            newIndex = 0;

        if (newIndex > projects.Count)
            newIndex = projects.Count;

        projects.Insert(newIndex, projectToMove);

        for (int i = 0; i < projects.Count; i++)
            projects[i].Position = i + 1;

        await _projectRepository.UpdateRangeAsync(projects);
    }

    private async Task<Project> GetExistingProject(int id, Guid userId)
    {
        return await _projectRepository.GetByIdAsync(id, userId)
            ?? throw new KeyNotFoundException("Project not found");
    }
}