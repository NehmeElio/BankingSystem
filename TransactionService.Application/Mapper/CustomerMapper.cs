using AutoMapper;
using Microsoft.AspNetCore.SignalR;
using TransactionService.Application.ViewModel;
using TransactionService.Domain.Models;

namespace TransactionService.Application.Mapper;

public class CustomerMapper:Profile
{
    public CustomerMapper()
    {
        CreateMap<Account, AccountViewModel>();
        CreateMap<Transaction, TransactionViewModel>()
            .ForMember(dest => dest.Username, opt
                => opt.MapFrom(src => src.Account != null ? src.Account.Username : null))
            .ForMember(dest => dest.TransactionType, opt
                => opt.MapFrom(src => src.TransactionType != null ? src.TransactionType.TransactionTypeName : ""));


    }
}