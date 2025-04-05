using AutoMapper;
using QRCodeBasedMetroTicketingSystem.Application.DTOs;
using QRCodeBasedMetroTicketingSystem.Web.Areas.Admin.ViewModels;
using QRCodeBasedMetroTicketingSystem.Web.Models;

namespace QRCodeBasedMetroTicketingSystem.Web.Mapping
{
    public class ViewModelMappingProfile : Profile
    {
        public ViewModelMappingProfile()
        {
            CreateMap<StationSummaryDto, StationSummaryViewModel>().ReverseMap();
            CreateMap<StationCreationDto, StationCreationViewModel>().ReverseMap();
            CreateMap<StationEditDto, StationEditViewModel>().ReverseMap();
            CreateMap<AdjacentStationDistanceDto, AdjacentStationDistanceViewModel>().ReverseMap();
            CreateMap<StationDeletionDto, StationDeletionViewModel>().ReverseMap();
            CreateMap<SettingsDto, SettingsViewModel>().ReverseMap();
            CreateMap<FareAndDistancesDto, FareAndDistancesViewModel>().ReverseMap();
            CreateMap<RegisterUserDto, RegisterUserViewModel>().ReverseMap();
        }
    }
}
