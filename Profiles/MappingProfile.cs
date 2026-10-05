using AutoMapper;
using FinanzasApi.Models;
using FinanzasApi.Models.DTOs;

namespace FinanzasApi.Profiles;

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
