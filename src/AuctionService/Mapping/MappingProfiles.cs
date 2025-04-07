using AuctionService.Entities;
using AuctionService.Models;
using AutoMapper;
using Contracts;

namespace AuctionService.Mapping
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Auction, AuctionModel>().IncludeMembers(x => x.Item);
            CreateMap<Item, AuctionModel>();
            CreateMap<CreateAuctionModel, Auction>().ForMember(d => d.Item, o => o.MapFrom(s => s));
            CreateMap<CreateAuctionModel, Item>();
            CreateMap<AuctionModel, AuctionCreated>();
            CreateMap<Auction, AuctionUpdated>().IncludeMembers(a => a.Item);
            CreateMap<Item, AuctionUpdated>();
        }
    }
}
