using AutoMapper;
using EnergyBoard.Domain.entities;
using EnergyBoard.Application.DTOs.response.columns;
using EnergyBoard.Application.DTOs.response.projects;
using EnergyBoard.Application.DTOs.response.cards;
using EnergyBoard.Application.DTOs.request.projects;
using EnergyBoard.Application.DTOs.request.columns;
using EnergyBoard.Application.DTOs.request.cards;

namespace EnergyBoard.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Entidad → Response
            CreateMap<Project, ProjectResponse>();
            CreateMap<Column, ColumnResponse>();
            CreateMap<Column, ColumnWithCardsResponse>();
            CreateMap<Card, CardResponse>();
            CreateMap<Project, CompleteProjectResponse>();

            // Request → Entidad
            CreateMap<CreateProjectRequest, Project>();
            CreateMap<CreateColumnRequest, Column>();
            CreateMap<CreateCardRequest, Card>();
        }
    }
}