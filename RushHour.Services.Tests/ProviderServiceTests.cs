using Moq;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs.ProviderDtos;
using RushHour.Domain.DTOs.ProviderWorkingDaysDto;
using RushHour.Domain.DTOs;
using RushHour.Services.Services;

namespace RushHour.Services.Tests
{
    public class ProviderServiceTests
    {
        public IProviderService service;
        public Mock<IProviderRepository> repositoryMock;
        public Mock<IProviderWorkingDaysRepository> providerWorkDaysRepoMock;
        public Mock<IEmployeeRepository> employeeRepoMock;
        public Mock<IEmployeeService> employeeServiceMock;

        public ProviderServiceTests()
        {
            repositoryMock = new Mock<IProviderRepository>();
            providerWorkDaysRepoMock = new Mock<IProviderWorkingDaysRepository>();
            employeeRepoMock = new Mock<IEmployeeRepository>();
            employeeServiceMock = new Mock<IEmployeeService>();

            repositoryMock
                .Setup(s => s.UpdateAsync(It.IsAny<Guid>(), It.IsAny<CreateProviderDto>()));
            repositoryMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new GetProviderDto());
            repositoryMock
                .Setup(s => s.GetPageAsync(It.IsAny<int>(), It.IsAny<int>()))
                .ReturnsAsync(new PaginatedResult<GetProviderDto>(new List<GetProviderDto>(), 0));
            
            providerWorkDaysRepoMock
                .Setup(s => s.GetAllWorkingDaysOfProviderAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new List<ProviderWorkingDaysDto>());

            employeeServiceMock
                .Setup(s => s.GetEmployeeByAccountAsync(It.IsAny<Guid>()))
                .ReturnsAsync(() => null);

            service = new ProviderService(repositoryMock.Object, employeeRepoMock.Object, 
                employeeServiceMock.Object, providerWorkDaysRepoMock.Object);
        }

        [Fact]
        public async Task Create_ValidData_ExpectedNotNull()
        {
            // Arrange
            var provider = new GetProviderDto
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

            var createProvider = new CreateProviderDto
            {
                Name = "Test",
                Website = "www.test.com",
                BusinessDomain = "test",
                Phone = "1234567890",
                StartTime = "08:00",
                EndTime = "18:00",
                WorkingDays = "1, 2, 3, 4, 5"
            };

            repositoryMock
                .Setup(s => s.CreateAsync(It.IsAny<CreateProviderDto>()))
                .ReturnsAsync(provider);

            service = new ProviderService(repositoryMock.Object, employeeRepoMock.Object, null, providerWorkDaysRepoMock.Object);

            // Act
            var result = await service.CreateAsync(createProvider);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(provider, result);
        }

        [Fact]
        public async Task Update_InvalidRequesterId_ExpectedException()
        {
            // Act
            async Task a() => await service.UpdateAsync(default(Guid), default(Guid), null);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(a);
        }

        [Fact]
        public async Task GetById_InvalidRequesterId_ExpectedException()
        {
            // Act
            async Task a() => await service.GetByIdAsync(default(Guid), default(Guid));

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(a);
        }

        [Fact]
        public async Task GetById_ValidData_ExpectedNotNull()
        {
            // Arrange
            var providerId = Guid.NewGuid();

            var expected = new GetProviderDto
            {
                WorkingDays = "Monday, Tuesday, Wednesday, Thursday, Friday"
            };

            List<ProviderWorkingDaysDto> workingDays = new()
            {
                new ProviderWorkingDaysDto { ProviderId = providerId, DayOfTheWeek = 1 },
                new ProviderWorkingDaysDto { ProviderId = providerId, DayOfTheWeek = 2 },
                new ProviderWorkingDaysDto { ProviderId = providerId, DayOfTheWeek = 3 },
                new ProviderWorkingDaysDto { ProviderId = providerId, DayOfTheWeek = 4 },
                new ProviderWorkingDaysDto { ProviderId = providerId, DayOfTheWeek = 5 }
            };

            providerWorkDaysRepoMock
                .Setup(s => s.GetAllWorkingDaysOfProviderAsync(It.IsAny<Guid>()))
                .ReturnsAsync(workingDays);

            service = new ProviderService(repositoryMock.Object, employeeRepoMock.Object, 
                employeeServiceMock.Object, providerWorkDaysRepoMock.Object);

            // Act
            var result = await service.GetByIdAsync(Guid.NewGuid(), providerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equivalent(expected, result);
        }

        [Fact]
        public async Task GetAllAsync_ValidParameters_ExpectedEmptyCollection()
        {
            // Act
            var providers = await service.GetPageAsync(1, 10);

            // Assert
            Assert.NotNull(providers);
            Assert.Equal(0, providers.Result.Count);
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
