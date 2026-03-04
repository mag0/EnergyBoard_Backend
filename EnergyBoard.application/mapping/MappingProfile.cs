using AutoMapper;
using EnergyBoard.Domain.entities;
using EnergyBoard.Application.DTOs.response;
using EnergyBoard.Application.DTOs.request;

namespace EnergyBoard.Application.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Project, ProjectResponse>();
            CreateMap<Column, ColumnResponse>();
            CreateMap<Card, CardResponse>();
            CreateMap<CreateProjectRequest, Project>();
            CreateMap<CreateColumnRequest, Column>();
            CreateMap<CreateCardRequest, Card>();
            CreateMap<UpdateProjectRequest, Project>();
            CreateMap<UpdateColumnRequest, Column>();
            CreateMap<UpdateCardRequest, CardResponse>();
            CreateMap<Project, CompleteProjectResponse>();
        }
    }
}