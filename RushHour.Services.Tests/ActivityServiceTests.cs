using Moq;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.DTOs;
using RushHour.Domain.Enums;
using RushHour.Services.Services;
using RushHour.Domain.DTOs.ActivityDtos;
using RushHour.Domain.DTOs.EmployeeDtos;
using RushHour.Domain.DTOs.ActivityEmployeeDtos;
using AutoMapper;

namespace RushHour.Services.Tests
{
    public class ActivityServiceTests
    {
        public IActivityService service;
        public Mock<IActivityRepository> activityRepoMock;
        public Mock<IAccountRepository> accountRepoMock;
        public Mock<IEmployeeService> employeeServiceMock;
        public Mock<IEmployeeRepository> employeeRepoMock;
        public Mock<IActivityEmployeeRepository> activityEmployeeRepoMock;
        public Mock<IMapper> mapperMock;

		public ActivityServiceTests() 
        {
            activityRepoMock = new Mock<IActivityRepository>();
            accountRepoMock = new Mock<IAccountRepository>();
            employeeServiceMock = new Mock<IEmployeeService>();
            employeeRepoMock = new Mock<IEmployeeRepository>();
            activityEmployeeRepoMock = new Mock<IActivityEmployeeRepository>();
            mapperMock = new Mock<IMapper>();

            activityRepoMock
                .Setup(s => s.CreateAsync(It.IsAny<CreateActivityDto>()))
                .ReturnsAsync(new GetActivityDto());
            activityRepoMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new GetActivityDto());            

            accountRepoMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AccountDto());
            accountRepoMock
                .Setup(s => s.CreateAsync(It.IsAny<CreateAccountDto>(), It.IsAny<byte[]>()))
                .ReturnsAsync(new AccountDto());
            accountRepoMock
                .Setup(s => s.CheckIfAnyMatchesIdAndRole(It.IsAny<Guid>(), It.IsAny<Role>()))
                .ReturnsAsync(false);

            activityEmployeeRepoMock
                .Setup(s => s.CreateActivityWithManyEmployeesAsync(It.IsAny<Guid>(), It.IsAny<List<Guid>>()));
            activityEmployeeRepoMock
                .Setup(s => s.GetAllEmployeesOfActivityAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<ActivityEmployeeDto>());

            service = new ActivityService(activityRepoMock.Object, accountRepoMock.Object,
                null, activityEmployeeRepoMock.Object, null, null);
        }

        [Fact]
        public async Task Create_ValidData_ExpectedNotNull()
        {
            // Arrange
            var requesterId = Guid.NewGuid();
            var providerId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();

            var expected = new GetActivityDto
            {
                EmployeeIds = new List<Guid> { employeeId }
            };

            var employee = new GetEmployeeDto
            {
                Id = employeeId,
                ProviderId = providerId,
                Account = new GetAccountDto { Role = Role.ProviderAdmin }
            };

            var dto = new CreateActivityDto
            {
                ProviderId = providerId,
                EmployeeIds = new List<Guid> { employeeId }
            };

            employeeRepoMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(employee);

            employeeServiceMock
                .Setup(s => s.GetEmployeeByAccountAsync(It.IsAny<Guid>()))
                .ReturnsAsync(employee);

            mapperMock
                .Setup(s => s.Map<GetActivityDto>(It.IsAny<CreateActivityDto>()))
                .Returns(new GetActivityDto() { ProviderId = dto.ProviderId, EmployeeIds = dto.EmployeeIds});

            service = new ActivityService(activityRepoMock.Object, accountRepoMock.Object,
                employeeServiceMock.Object, activityEmployeeRepoMock.Object, 
                employeeRepoMock.Object, mapperMock.Object);

            //Act
            var result = await service.CreateActivityAsync(requesterId, dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equivalent(expected, result);
        }

        [Fact]
        public async Task Update_InvalidRequesterId_ExpectedException()
        {
            // Act
            async Task a() => await service.UpdateActivityAsync(default(Guid), null, default(Guid));

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(a);
        }

        [Fact]
        public async Task GetById_InvalidRequesterId_ExpectedException()
        {
            // Act
            async Task a() => await service.GetActivityByIdAsync(default(Guid), default(Guid));

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(a);
        }

        [Fact]
        public async Task GetById_ValidData_ExpectedNotNull()
        {
            // Arrange
            var requesterId = Guid.NewGuid();
            var activityId = Guid.NewGuid();
            var providerId = Guid.NewGuid();
            var employeeId = Guid.NewGuid();

            var employee = new GetEmployeeDto
            {
                Id = employeeId,
                ProviderId = providerId,
                Account = new GetAccountDto { Role = Role.ProviderAdmin }
            };

            var activity = new GetActivityDto
            {
                Id = activityId,
                ProviderId = providerId
            };
            
            activityRepoMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(activity);

            employeeServiceMock
                .Setup(s => s.GetEmployeeByAccountAsync(It.IsAny<Guid>()))
                .ReturnsAsync(employee);

            service = new ActivityService(activityRepoMock.Object, accountRepoMock.Object,
                employeeServiceMock.Object, activityEmployeeRepoMock.Object, null, null);

            // Act
            var result = await service.GetActivityByIdAsync(requesterId, activityId);

            // Assert
            Assert.NotNull(result);
            Assert.Equivalent(activity, result);
        }

        [Fact]
        public async Task GetAll_InvalidRequesterId_ExpectedException()
        {
            // Act
            async Task a() => await service.GetPageAsync(1, 10, default(Guid));

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(a);
        }

        [Fact]
        public async Task GetAllAsync_ValidParameters_ExpectedEmptyCollection()
        {
            // Arrange
            var page = new PaginatedResult<GetActivityDto>(new List<GetActivityDto>(), 0);

            activityRepoMock
                .Setup(s => s.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Guid>()))
                .ReturnsAsync(page);

            // Act
            var activities = await service.GetPageAsync(1, 10, Guid.NewGuid());

            // Assert
            Assert.NotNull(activities);
            Assert.Equal(0, activities.Result.Count);
        }

        [Fact]
        public async Task Delete_InvalidProviderId_ExpectedException()
        {
            // Act
            async Task a() => await service.DeleteAsync(default(Guid));

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(a);
        }
    }
}
