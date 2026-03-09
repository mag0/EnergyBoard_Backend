using AutoMapper;
using EnergyBoard.Application.DTOs.request.projects;
using EnergyBoard.Application.DTOs.response.projects;
using EnergyBoard.Application.interfaces;
using EnergyBoard.Domain.entities;
using EnergyBoard.Domain.interfaces;
using System.Data.Common;

namespace EnergyBoard.Application.services;
public class ProjectService(
    IProjectRepository projectRepository,
    IColumnRepository columnRepository,
    ICardRepository cardRepository,
    IMapper mapper
    ) : IProjectService
{
    private readonly IProjectRepository _projectRepository = projectRepository;
    private readonly IColumnRepository _columnRepository = columnRepository;
    private readonly ICardRepository _cardRepository = cardRepository;
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

        var responses = _mapper.Map<List<ProjectResponse>>(projects);

        foreach (var project in responses)
        {
            project.Progress = await _projectRepository.GetProjectProgressAsync(project.Id);
        }

        return responses;
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
        var project = await GetProjectEntityAsync(projectId, userId);

        project.Title = request.Title ?? project.Title;
        project.Description = request.Description ?? project.Description;
        project.UpdatedAt = DateTime.UtcNow;

        await _projectRepository.UpdateAsync(project);
    }

    public async Task DeleteAsync(int projectId, Guid userId)
    {
        var project = await GetProjectEntityAsync(projectId, userId);
        var now = DateTime.UtcNow;

        var columns = await _columnRepository.GetAllAsync(projectId, userId);
        var cards = await _cardRepository.GetAllByProjectAsync(projectId, userId);

        MarkCardsAsDeleted(cards, now);
        MarkColumnsAsDeleted(columns, now);
        MarkProjectAsDeleted(project, now);

        if (cards.Count != 0)
            await _cardRepository.UpdateRangeAsync(cards);
        if (cards.Count != 0)
            await _columnRepository.UpdateRangeAsync(columns);

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

    private async Task<Project> GetProjectEntityAsync(int projectId, Guid userId)
    {
        return await _projectRepository.GetByIdAsync(projectId, userId)
            ?? throw new KeyNotFoundException("Project not found");
    }

    private static void MarkCardsAsDeleted(List<Card> cards, DateTime now)
    {
        foreach (var card in cards)
        {
            card.IsDeleted = true;
            card.UpdatedAt = now;
        }
    }
    private static void MarkColumnsAsDeleted(List<Column> columns, DateTime now)
    {
        foreach (var column in columns)
        {
            column.IsDeleted = true;
            column.UpdatedAt = now;
        }
    }
    private static void MarkProjectAsDeleted(Project project, DateTime now)
    {
        project.IsDeleted = true;
        project.UpdatedAt = now;
    }
}