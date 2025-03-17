using AutoMapper;
using Contracts;
using MassTransit;
using MongoDB.Entities;
using SearchService.Models;
using static MassTransit.ValidationResultExtensions;

namespace SearchService.Consumers
{
    public class AuctionCreatedConsumer(IMapper mapper) : IConsumer<AuctionCreated>
    {
        public async Task Consume(ConsumeContext<AuctionCreated> context)
        {
            var item = mapper.Map<Item>(context.Message);

            await item.SaveAsync();
        }
    }
}
