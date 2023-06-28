using RushHour.Data.Repositories;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.Middleware;
using RushHour.Services.Services;
using FluentValidation;
using RushHour.Domain.DTOs.ProviderDtos;
using RushHour.Domain.Validators.Provider;
using RushHour.Domain.DTOs.EmployeeDtos;
using RushHour.Domain.Validators.Employee;
using RushHour.Domain.DTOs.ClientDtos;
using RushHour.Domain.Validators.Client;
using RushHour.Domain.DTOs.AppointmentDtos;
using RushHour.Domain.Validators.Appointment;
using RushHour.Domain.DTOs.ActivityDtos;
using RushHour.Domain.Validators.Activity;
using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.Validators.Account;
using FluentValidation.AspNetCore;

namespace RushHour.API.Configuration
{
    public static class ServicesConfiguration
    {
        public static void AddCustomServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<IProviderService, ProviderService>();
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();
            builder.Services.AddScoped<IClientService, ClientService>();
            builder.Services.AddScoped<IAuthService, AuthService>(); 
            builder.Services.AddScoped<IActivityService, ActivityService>();
            builder.Services.AddScoped<IAppointmentService, AppointmentService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddTransient<GlobalExceptionHandlingMiddleware>();
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
        }
        public static void AddCustomRepositories(this WebApplicationBuilder builder)
        {
            builder.Services.AddScoped<IProviderRepository, ProviderRepository>();
            builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            builder.Services.AddScoped<IAccountRepository, AccountRepository>();
            builder.Services.AddScoped<IClientRepository, ClientRepository>();
            builder.Services.AddScoped<IActivityRepository, ActivityRepository>();
            builder.Services.AddScoped<IActivityEmployeeRepository, ActivityEmployeeRepository>();
            builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
            builder.Services.AddScoped<IProviderWorkingDaysRepository, ProviderWorkingDaysRepository>();
        }

        public static void AddFluentValidation(this WebApplicationBuilder builder)
        {
            builder.Services.AddFluentValidation(fv =>
            {
                fv.RegisterValidatorsFromAssemblyContaining<CreateProviderDtoValidator>();
            });
        }
    }
}
