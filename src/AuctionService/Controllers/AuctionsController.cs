﻿using AuctionService.Data;
using AuctionService.Entities;
using AuctionService.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Contracts;
using MassTransit;
using MassTransit.Transports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionsController(AuctionDbContext _context, IMapper _mapper, IPublishEndpoint publishEndpoint) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetAllAuctions(string date)
        {
            var query = _context.Auctions.OrderBy(x => x.Item.Make).AsQueryable();

            if (!string.IsNullOrEmpty(date))
                query = query.Where(x => x.UpdatedAt.CompareTo(DateTime.Parse(date).ToUniversalTime()) > 0);

            return Ok(await query.ProjectTo<AuctionModel>(_mapper.ConfigurationProvider).ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAuctionById(Guid id)
        {
            var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return NotFound();

            return Ok(_mapper.Map<AuctionModel>(auction));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateAuction(CreateAuctionModel auctionDto)
        {
            var auction = _mapper.Map<Auction>(auctionDto);
            
            auction.Seller = User.Identity.Name;

            _context.Add(auction);

            var newAuction = _mapper.Map<AuctionModel>(auction);

            await publishEndpoint.Publish(_mapper.Map<AuctionCreated>(newAuction));

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Could not save changes to the DB");

            return CreatedAtAction(nameof(GetAuctionById), new { auction.Id }, newAuction);
        }

        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuction(Guid id, UpdateAuctionModel updateAuctionDto)
        {
            var auction = await _context.Auctions.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == id);

            if (auction == null) return NotFound();

            if (auction.Seller != User.Identity?.Name) return Forbid();

            auction.Item.Make = updateAuctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = updateAuctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = updateAuctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = updateAuctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = updateAuctionDto.Year ?? auction.Item.Year;

            await publishEndpoint.Publish(_mapper.Map<AuctionUpdated>(auction));

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok();

            return Ok(_mapper.Map<AuctionModel>(auction));
        }

        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuction(Guid id)
        {
            var auction = await _context.Auctions.FindAsync(id);

            if (auction == null) return NotFound();
            
            if (auction.Seller != User.Identity?.Name) return Forbid();

            _context.Auctions.Remove(auction);

            await publishEndpoint.Publish<AuctionDeleted>(new { Id = auction.Id.ToString() });

            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("Could not update DB");

            return Ok();
        }
    }
}
