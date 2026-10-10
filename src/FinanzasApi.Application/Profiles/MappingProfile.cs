using AutoMapper;
using FinanzasApi.Domain.Entities;
using FinanzasApi.Domain.Enums;
using FinanzasApi.Application.Dtos;

namespace FinanzasApi.Application.Profiles;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Workspace
        CreateMap<Workspace, WorkspaceDto>();

        // Account
        CreateMap<Account, AccountDto>();

        // Category
        CreateMap<Category, CategoryDto>();

        // Contact
        CreateMap<Contact, ContactDto>();

        // Transaction
        CreateMap<Transaction, TransactionDto>();

        // Budget
        CreateMap<Budget, BudgetDto>();

        // Currency
        CreateMap<Currency, CurrencyDto>();
    }
}
