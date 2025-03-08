using AuctionService.Data;
using AuctionService.Entities;
using AuctionService.Models;
using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionsController(AuctionDbContext _context, IMapper _mapper) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<AuctionModel>>> GetAllAuctions()
        {
            List<Auction> auctions = await _context.Auctions.Include(x => x.Item).OrderBy(x => x.Item.Make).ToListAsync();
            return _mapper.Map<List<AuctionModel>>(auctions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionModel>> GetAuctionById(Guid id)
        {
            var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return NotFound();

            return _mapper.Map<AuctionModel>(auction);
        }
    }
}
