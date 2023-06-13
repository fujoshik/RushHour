using Moq;
using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.DTOs;
using RushHour.Services.Services;
using RushHour.Domain.DTOs.ClientDtos;
using RushHour.Domain.Enums;

namespace RushHour.Services.Tests
{
    public class ClientServiceTests
    {
        public IClientService service;
        public Mock<IClientRepository> clientRepoMock;
        public Mock<IAccountRepository> accountRepoMock;
        public Mock<IAccountService> accountServiceMock;
        public ClientServiceTests()
        {
            clientRepoMock = new Mock<IClientRepository>();
            accountRepoMock = new Mock<IAccountRepository>();
            accountServiceMock = new Mock<IAccountService>();

            var page = new PaginatedResult<GetClientDto>(new List<GetClientDto>(), 0);

            clientRepoMock
                .Setup(s => s.CreateAsync(It.IsAny<GetAccountDto>(), It.IsAny<CreateClientDto>()))
                .ReturnsAsync(new GetClientDto());
            clientRepoMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new GetClientDto());
            clientRepoMock
                .Setup(s => s.GetPageAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(page);

            accountRepoMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(new AccountDto());
            accountRepoMock
                .Setup(s => s.CreateAsync(It.IsAny<CreateAccountDto>(), It.IsAny<byte[]>()))
                .ReturnsAsync(new AccountDto());
            accountRepoMock 
                .Setup(s => s.CheckIfAnyMatchesIdAndRole(It.IsAny<Guid>(), It.IsAny<Role>()))
                .ReturnsAsync(false);

            accountServiceMock.Setup(s => s.GenerateSalt());
            accountServiceMock.Setup(s => s.HashPasword(It.IsAny<string>(), It.IsAny<byte[]>()));

            service = new ClientService(clientRepoMock.Object, accountRepoMock.Object,
                accountServiceMock.Object);
        }

        [Fact]
        public async Task Create_ValidData_ExpectedNotNull()
        {
            // Arrange
            var expected = new GetClientDto
            {
                Account = new GetAccountDto()
            };

            var dto = new CreateClientDto
            {
                Account = new CreateAccountDto { Role = Role.Client }
            };

            // Act
            var result = await service.CreateClientAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equivalent(expected, result);
        }

        [Fact]
        public async Task Update_InvalidRequesterId_ExpectedException()
        {
            // Act
            async Task a() => await service.UpdateClientAsync(default(Guid), null, default(Guid));

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(a);
        }

        [Fact]
        public async Task GetById_InvalidRequesterId_ExpectedException()
        {
            // Act
            async Task a() => await service.GetClientByIdAsync(default(Guid), default(Guid));

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(a);
        }

        [Fact]
        public async Task GetById_ValidData_ExpectedNotNull()
        {
            // Arrange
            var requesterId = Guid.NewGuid();
            var id = Guid.NewGuid();

            var clientToRead = new GetClientDto
            {
                Id = id,
                AccountId = requesterId
            };

            clientRepoMock
                .Setup(s => s.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(clientToRead);

            service = new ClientService(clientRepoMock.Object, accountRepoMock.Object,
                accountServiceMock.Object);

            // Act
            var result = await service.GetClientByIdAsync(requesterId, id);

            // Assert
            Assert.NotNull(result);
            Assert.Equivalent(clientToRead, result);
        }

        [Fact]
        public async Task GetAllAsync_ValidParameters_ExpectedEmptyCollection()
        {
            // Act
            var clients = await service.GetPageAsync(1, 10);

            // Assert
            Assert.NotNull(clients);
            Assert.Equal(0, clients.Result.Count);
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
