using RushHour.Data.Repositories;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.Middleware;
using RushHour.Services.Services;
using AutoMapper;

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
    }
}
