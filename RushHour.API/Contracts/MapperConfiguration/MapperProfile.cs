using AutoMapper;
using RushHour.Data.Entities;
using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.DTOs.ActivityDtos;
using RushHour.Domain.DTOs.ActivityEmployeeDtos;
using RushHour.Domain.DTOs.AppointmentDtos;
using RushHour.Domain.DTOs.ClientDtos;
using RushHour.Domain.DTOs.EmployeeDtos;
using RushHour.Domain.DTOs.ProviderDtos;
using RushHour.Domain.DTOs.ProviderWorkingDaysDto;

namespace RushHour.API.Contracts.MapperConfiguration
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<Provider, CreateProviderDto>();
            CreateMap<CreateProviderDto, Provider>()
                .ForMember(
                    dest => dest.StartTime,
                    opt => opt.MapFrom(src => DateTime.Parse(src.StartTime)))
                .ForMember(
                    dest => dest.EndTime,
                    opt => opt.MapFrom(src => DateTime.Parse(src.EndTime)))
                .ForMember(dest => dest.WorkingDays, opt => opt.Ignore());

            CreateMap<Provider, GetProviderDto>()
                .ForMember(
                    dest => dest.StartTime,
                    opt => opt.MapFrom(src => TimeOnly.FromDateTime(src.StartTime)))
                .ForMember(
                    dest => dest.EndTime,
                    opt => opt.MapFrom(src => TimeOnly.FromDateTime(src.EndTime)));
            CreateMap<GetProviderDto, Provider>();

			CreateMap<CreateProviderDto, GetProviderDto>()
				.ForMember(
					dest => dest.StartTime,
					opt => opt.MapFrom(src => TimeOnly.Parse(src.StartTime)))
				.ForMember(
					dest => dest.EndTime,
					opt => opt.MapFrom(src => TimeOnly.Parse(src.EndTime)));

			CreateMap<ProviderWorkingDays, ProviderWorkingDaysDto>()
                .ForMember(
                    dest => dest.DayOfTheWeek,
                    opt => opt.MapFrom(src => (int)src.DayOfTheWeek));

            CreateMap<Employee, CreateEmployeeDto>().ReverseMap();
            CreateMap<Employee, CreateEmployeeWithoutProviderDto>().ReverseMap();
            CreateMap<Employee, GetEmployeeDto>().ReverseMap();
            CreateMap<CreateEmployeeDto, GetEmployeeDto>();

            CreateMap<Client, CreateClientDto>().ReverseMap();
            CreateMap<Client, GetClientDto>().ReverseMap();
            CreateMap<Client, UpdateClientDto>().ReverseMap();

            CreateMap<Account, AccountDto>().ReverseMap();
            CreateMap<Account, CreateAccountDto>().ReverseMap();
            CreateMap<Account, GetAccountDto>().ReverseMap();
            CreateMap<Account, UpdateAccountWithoutRole>().ReverseMap();
            CreateMap<CreateAccountDto, GetAccountDto>().ReverseMap();
            CreateMap<AccountDto, GetAccountDto>().ReverseMap();
            CreateMap<UpdateAccountWithoutRole, CreateAccountDto>();
            
            CreateMap<Activity, CreateActivityDto>().ReverseMap();
            CreateMap<Activity, GetActivityDto>().ReverseMap();
            CreateMap<CreateActivityDto, GetActivityDto>().ReverseMap();

            CreateMap<ActivityEmployee, ActivityEmployeeDto>().ReverseMap();

            CreateMap<Appointment, CreateAppointmentDto>().ReverseMap();
            CreateMap<Appointment, GetAppointmentDto>().ReverseMap();
            CreateMap<CreateAppointmentDto,  GetAppointmentDto>().ReverseMap();
        }
    }
}
