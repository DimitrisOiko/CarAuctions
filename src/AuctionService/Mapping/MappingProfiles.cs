using AuctionService.Entities;
using AuctionService.Models;
using AutoMapper;

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
        }
    }
}
