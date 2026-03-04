
using AutoMapper;
using EnergyBoard.Application.DTOs.request;
using EnergyBoard.Application.DTOs.response;
using EnergyBoard.Domain.entities;
using EnergyBoard.Domain.interfaces;

namespace EnergyBoard.Application.services;

public class ProjectService (IProjectRepository projectRepository, IMapper mapper)
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IMapper _mapper = mapper;

    public async Task AddProjectAsync(CreateProjectRequest createProjectRequest)
    {
        var nextPosition = await _projectRepository.GetNextPositionAsync();

        var project = _mapper.Map<Project>(createProjectRequest);

        project.Position = nextPosition;
        project.CreatedAt = DateTime.UtcNow;

        await _projectRepository.AddAsync(project);
    }

    public async Task<ProjectResponse?> GetProjectByIdAsync(int id)
    {
        var project = await GetExistingProject(id);

        var projectResponse = _mapper.Map<ProjectResponse>(project);

        return projectResponse;
    }
    
    public async Task UpdateProjectAsync(int id, UpdateProjectRequest updateProject)
    {
        var project = await GetExistingProject(id);

        if (updateProject.Title != null)
        {
            project.Title = updateProject.Title;
        }
        if(updateProject.Description != null)
        {
            project.Description = updateProject.Description;
        }
        project.UpdatedAt = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(project);
    }

    public async Task<IEnumerable<ProjectResponse>> GetAllProjectsAsync()
    {
        var projects = await _projectRepository.GetAllAsync();
        var projectResponses = projects.Select(project => _mapper.Map<ProjectResponse>(project));

        return projectResponses;
    }

    public async Task DeleteProjectAsync(int id)
    { 
        var project = await GetExistingProject(id);

        await _projectRepository.DeleteAsync(project!);
    }

    public async Task<CompleteProjectResponse?> GetProjectCompleteAsync(int id)
    {
        var project = await _projectRepository.GetProjectCompleteAsync(id)
            ?? throw new KeyNotFoundException("Project not found");

        var completeProjectResponse = _mapper.Map<CompleteProjectResponse>(project);

        return completeProjectResponse;
    }

    public async Task UpdateProjectPositionAsync(int id, MoveProjectRequest request)
    {
        var projects = (await _projectRepository.GetAllAsync()).ToList();

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
        {
            projects[i].Position = i + 1;
        }

        await _projectRepository.UpdateRangeAsync(projects);
    }

    private async Task<Project> GetExistingProject(int id)
    {
        return await _projectRepository.GetByIdAsync(id) 
            ?? throw new KeyNotFoundException("Project not found");
    }
}
