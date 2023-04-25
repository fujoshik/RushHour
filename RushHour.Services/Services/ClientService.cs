﻿using RushHour.Domain.Abstractions.Repositories;
using RushHour.Domain.Abstractions.Services;
using RushHour.Domain.DTOs.AccountDtos;
using RushHour.Domain.DTOs;
using RushHour.Domain.Enums;
using RushHour.Domain.DTOs.ClientDtos;

namespace RushHour.Services.Services
{
    public class ClientService : IClientService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IAccountRepository _accountRepository;

        public ClientService(IClientRepository clientRepository, IAccountRepository accountRepository)
        {
            this._clientRepository = clientRepository;
            this._accountRepository = accountRepository;
        }

        public async Task<GetClientDto> CreateClientAsync(CreateClientDto dto)
        {
            var createdAccount = await CreateAccountAsync(dto.Account);

            var client = await _clientRepository.CreateAsync(createdAccount, dto);

             client.Account = createdAccount;

            return client;
        }

        private async Task<GetAccountDto> CreateAccountAsync(CreateAccountDto dto)
        {
            if (dto.Role == Role.Admin)
            {
                throw new ArgumentException("Can't assign a client to be Admin!");
            }

            if (dto.Role == Role.ProviderAdmin)
            {
                throw new ArgumentException("Can't assign a client to be ProviderAdmin!");
            }

            if (dto.Role == Role.Employee)
            {
                throw new ArgumentException("Can't assign a client to be Employee!");
            }

            dto.Password = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            var account = await _accountRepository.CreateAsync(dto);

            return new GetAccountDto()
            {
                Id = account.Id,
                Email = account.Email,
                FullName = account.FullName,
                Role = account.Role
            };
        }

        public async Task DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var client = await _clientRepository.GetByIdAsync(id);

            await _accountRepository.DeleteAsync(client.AccountId);        
        }

        public async Task<PaginatedResult<GetClientDto>> GetPageAsync(int index, int pageSize)
        {
            return await _clientRepository.GetPageAsync(index, pageSize);
        }

        public async Task<GetClientDto> GetClientByIdAsync(Guid id)
        {
            return await _clientRepository.GetByIdAsync(id);
        }

        private async Task<GetAccountDto> GetAccountByIdAsync(Guid id)
        {
            var account = await _accountRepository.GetByIdAsync(id);

            return new GetAccountDto()
            {
                Id = account.Id,
                Email = account.Email,
                FullName = account.FullName,
                Role = account.Role
            };
        }

        public async Task UpdateClientAsync(Guid id, UpdateClientDto dto, string requesterId)
        {
            var updateAccount = new UpdateAccountWithoutRole()
            {
                Email = dto.Account.Email,
                FullName = dto.Account.FullName
            };

            await UpdateAccountAsync(id, updateAccount, requesterId);

            await _clientRepository.UpdateAsync(id, dto);
        }

        private async Task UpdateAccountAsync(Guid clientId, UpdateAccountWithoutRole account, string requesterId)
        {
            var currentAccount = await _accountRepository.GetByIdAsync(Guid.Parse(requesterId));

            if (currentAccount.Role == Role.Admin)
            {
                await AdminUpdateAccountAsync(clientId, account);
            }
        }

        private async Task AdminUpdateAccountAsync(Guid clientId, UpdateAccountWithoutRole dto)
        {
            var client = await _clientRepository.GetByIdAsync(clientId);

            client.Account = await GetAccountByIdAsync(client.AccountId);

            var createAccount = new CreateAccountDto()
            {
                Email = dto.Email,
                FullName = dto.FullName,
                Role = Role.Client
            };

            await _accountRepository.UpdateAsync(client.AccountId, createAccount);
        }
    }
}