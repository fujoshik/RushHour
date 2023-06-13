using Moq;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.DTOs.ActivityDtos;
using RushHour.Domain.DTOs.AppointmentDtos;
using RushHour.Domain.DTOs.EmployeeDtos;
using RushHour.Domain.DTOs.ProviderDtos;
using RushHour.Domain.DTOs.ProviderWorkingDaysDto;
using RushHour.Domain.Enums;
using RushHour.Services.Services;

namespace RushHour.Services.Tests
{
    public class AppointmentServiceTests
    {
        public IAppointmentService service;
        public Mock<IAppointmentRepository> repositoryMock;
        public Mock<IAccountRepository> accountRepoMock;
        public Mock<IActivityRepository> activityRepoMock;
        public Mock<IProviderRepository> providerRepoMock;
        public Mock<IProviderWorkingDaysRepository> providerWorkDaysRepoMock;
        public Mock<IEmployeeRepository> employeeRepoMock;

        public AppointmentServiceTests()
        {
            repositoryMock = new Mock<IAppointmentRepository>();
            accountRepoMock = new Mock<IAccountRepository>();
            activityRepoMock = new Mock<IActivityRepository>();
            providerRepoMock = new Mock<IProviderRepository>();
            providerWorkDaysRepoMock = new Mock<IProviderWorkingDaysRepository>();
            employeeRepoMock = new Mock<IEmployeeRepository>();

            service = new AppointmentService(repositoryMock.Object, accountRepoMock.Object, null, null,
                activityRepoMock.Object, providerRepoMock.Object, providerWorkDaysRepoMock.Object, null);
        }

        [Fact]
        public async Task Create_ValidData_ExpectedNotNull()
        {
            // Arrange
            var expected = new GetAppointmentDto
            {
                TotalPrice = 10
            };

            var provider = CreateMockProvider();

            var activity = CreateMockActivity(provider.Id);

            var appointment = CreateMockAppointment(activity.Id);

            var workingDays = CreateMockWorkingDays(provider.Id);

            var employee = CreateMockEmployee(appointment.EmployeeId, provider.Id);

            CreateTestSetup(provider, activity, appointment, workingDays, employee); 

            // Act
            var result = await service.CreateAppointmentAsync(Guid.NewGuid(), appointment);

            // Assert
            Assert.NotNull(result);
            Assert.Equivalent(expected, result);
        }

        [Fact]
        public async Task Update_InvalidAppointmentId_ExpectedException()
        {
            // Act
            async Task a() => await service.UpdateAppointmentAsync(default(Guid), null, default(Guid));

            // Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(a);
        }

        [Fact]
        public async Task GetById_InvalidRequesterId_ExpectedException()
        {
            // Act
            async Task a() => await service.GetAppointmentByIdAsync(default(Guid), default(Guid));

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(a);
        }

        [Fact]
        public async Task GetAllAsync_ValidParameters_ExpectedEmptyCollection()
        {
            // Arrange
            repositoryMock
                .Setup(s => s.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>(), It.IsAny<Guid>()))
                .ReturnsAsync(new PaginatedResult<GetAppointmentDto>(new List<GetAppointmentDto>(), 0));

            // Act
            var appointments = await service.GetPageAsync(1, 10, Guid.NewGuid());

            // Assert
            Assert.NotNull(appointments);
            Assert.Equal(0, appointments.Result.Count);
        }

        [Fact]
        public async Task Delete_InvalidRequesterId_ExpectedException()
        {
            // Act
            async Task a() => await service.DeleteAsync(default(Guid), default(Guid));

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(a);
        }

        [Fact]
        public async Task Delete_InvalidAppointmentId_ExpectedException()
        {
            // Act
            async Task a() => await service.DeleteAsync(Guid.NewGuid(), default(Guid));

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(a);
        }

        private GetProviderDto CreateMockProvider()
        {
            return new GetProviderDto()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                Website = "www.test.com",
                BusinessDomain = "test",
                Phone = "1234567890",
                StartTime = TimeOnly.Parse("08:00"),
                EndTime = TimeOnly.Parse("18:00"),
                WorkingDays = "1, 2, 3, 4, 5"
            };
        }

        private GetActivityDto CreateMockActivity(Guid providerId)
        {
            return new GetActivityDto()
            {
                Id = Guid.NewGuid(),
                Name = "Test",
                Price = 10,
                Duration = 30,
                ProviderId = providerId,
                EmployeeIds = new List<Guid>() { Guid.NewGuid(), Guid.NewGuid() }
            };        
        }

        private CreateAppointmentDto CreateMockAppointment(Guid activityId)
        {
            return new CreateAppointmentDto()
            {
                ActivityId = activityId,
                StartDate = new DateTime(2023, 06, 05, 11, 20, 0),
                EmployeeId = Guid.NewGuid()
            };
        }

        private GetEmployeeDto CreateMockEmployee(Guid employeeId, Guid providerId)
        {
            return new GetEmployeeDto()
            {
                Id = employeeId,
                Title = "Test",
                Phone = "1234567890",
                HireDate = new DateTime(2020, 04, 12),
                RatePerHour = 10,
                ProviderId = providerId,
                AccountId = Guid.NewGuid()
            };
        }

        private List<ProviderWorkingDaysDto> CreateMockWorkingDays(Guid providerId)
        {
            return new List<ProviderWorkingDaysDto>()
            {
                new ProviderWorkingDaysDto { ProviderId = providerId, DayOfTheWeek = 1 },
                new ProviderWorkingDaysDto { ProviderId = providerId, DayOfTheWeek = 2 },
                new ProviderWorkingDaysDto { ProviderId = providerId, DayOfTheWeek = 3 },
                new ProviderWorkingDaysDto { ProviderId = providerId, DayOfTheWeek = 4 },
                new ProviderWorkingDaysDto { ProviderId = providerId, DayOfTheWeek = 5 }
            };
        }

        private void CreateTestSetup(GetProviderDto provider, GetActivityDto activity, 
            CreateAppointmentDto appointment, List<ProviderWorkingDaysDto> workingDays, GetEmployeeDto employee)
        {
            repositoryMock
                .Setup(s => s.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Role>(), It.IsAny<Guid>()))
                .ReturnsAsync(new PaginatedResult<GetAppointmentDto>(new List<GetAppointmentDto>(), 0));
            repositoryMock
                .Setup(s => s.CreateAsync(It.IsAny<CreateAppointmentDto>(), It.IsAny<DateTime>()))
                .ReturnsAsync(new GetAppointmentDto());

            activityRepoMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(activity);

            accountRepoMock
               .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
               .ReturnsAsync(new AccountDto());

            providerRepoMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(provider);

            providerWorkDaysRepoMock
                .Setup(s => s.GetAllWorkingDaysOfProviderAsync(It.IsAny<Guid>()))
                .ReturnsAsync(workingDays);

            employeeRepoMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(employee);

            service = new AppointmentService(repositoryMock.Object, accountRepoMock.Object, null, null,
                activityRepoMock.Object, providerRepoMock.Object, providerWorkDaysRepoMock.Object, employeeRepoMock.Object);
        }
    }
}