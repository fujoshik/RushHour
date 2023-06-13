using Moq;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs;
using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.DTOs.EmployeeDtos;
using RushHour.Domain.DTOs.ProviderDtos;
using RushHour.Domain.Enums;
using RushHour.Services.Services;

namespace RushHour.Services.Tests
{
    public class EmployeeServiceTests
    {
        public IEmployeeService service;
        public Mock<IEmployeeRepository> employeeRepoMock;
        public Mock<IAccountRepository> accountRepoMock;
        public Mock<IProviderRepository> providerRepoMock;
        public Mock<IAccountService> accountServiceMock;

        public EmployeeServiceTests() 
        {
            employeeRepoMock = new Mock<IEmployeeRepository>();
            accountRepoMock = new Mock<IAccountRepository>();
            providerRepoMock = new Mock<IProviderRepository>();
            accountServiceMock = new Mock<IAccountService>();

            var page = new PaginatedResult<GetEmployeeDto>(new List<GetEmployeeDto>(), 0);

            employeeRepoMock
                .Setup(s => s.CreateAsync(It.IsAny<GetAccountDto>(), It.IsAny<CreateEmployeeDto>()))
                .ReturnsAsync(new GetEmployeeDto());
            employeeRepoMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new GetEmployeeDto());
            employeeRepoMock
                .Setup(s => s.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(page);

            accountRepoMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AccountDto());
            accountRepoMock
                .Setup(s => s.CreateAsync(It.IsAny<CreateAccountDto>(), It.IsAny<byte[]>()))
                .ReturnsAsync(new AccountDto());

            accountServiceMock.Setup(s => s.GenerateSalt());
            accountServiceMock.Setup(s => s.HashPasword(It.IsAny<string>(), It.IsAny<byte[]>()));

            service = new EmployeeService(employeeRepoMock.Object, accountRepoMock.Object,
                providerRepoMock.Object, accountServiceMock.Object);
        }

        [Fact]
        public async Task Create_ValidData_ExpectedNotNull()
        {
            // Arrange
            var dto = new CreateEmployeeDto
            {
                Account = new CreateAccountDto { Email = "test@test.com", Role = Role.Employee }
            };

            var provider = new GetProviderDto
            {
                BusinessDomain = "test"
            };

            var expected = new GetEmployeeDto();

            providerRepoMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(provider);           

            service = new EmployeeService(employeeRepoMock.Object, accountRepoMock.Object,
                providerRepoMock.Object, accountServiceMock.Object);

            // Act
            var result = await service.CreateEmployeeAsync(Guid.NewGuid(), dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equivalent(expected, result);
        }

        [Fact]
        public async Task Create_InvalidRequesterId_ExpectedException()
        {
            // Act
            async Task a() => await service.CreateEmployeeAsync(default(Guid), null);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(a);
        }

        [Fact]
        public async Task Update_InvalidRequesterId_ExpectedException()
        {
            // Act
            async Task a() => await service.UpdateEmployeeAsync(default(Guid), null, default(Guid));

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(a);
        }

        [Fact]
        public async Task GetById_InvalidRequesterId_ExpectedException()
        {
            // Act
            async Task a() => await service.GetEmployeeByIdAsync(default(Guid), default(Guid));

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(a);
        }

        [Fact]
        public async Task GetById_ValidData_ExpectedNotNull()
        {
            // Arrange
            var requesterId = Guid.NewGuid();
            var id = Guid.NewGuid();

            var currentAccount = new AccountDto
            {
                Role = Role.Employee
            };

            var employeeToRead = new GetEmployeeDto
            {
                Id = id,
                AccountId = requesterId
            };

            accountRepoMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(currentAccount);

            employeeRepoMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(employeeToRead);

            service = new EmployeeService(employeeRepoMock.Object, accountRepoMock.Object,
                providerRepoMock.Object, accountServiceMock.Object);

            // Act
            var result = await service.GetEmployeeByIdAsync(requesterId, id);

            // Assert
            Assert.NotNull(result);
            Assert.Equivalent(employeeToRead, result);
        }

        [Fact]
        public async Task GetAllAsync_ValidParameters_ExpectedEmptyCollection()
        {
            // Act
            var employees = await service.GetPageAsync(1, 10, Guid.NewGuid());

            // Assert
            Assert.NotNull(employees);
            Assert.Equal(0, employees.Result.Count);
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
