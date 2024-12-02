using Microsoft.AspNetCore.Mvc;
using System;
using ItemServiceAPI.Repositories;
using ItemServiceAPI.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;


namespace ItemServiceAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemController : ControllerBase
{
    

    private readonly ILogger<ItemController> _logger; // Logger interface
    private readonly IItemDbRepository _iItemDbRepository; // Repository interface for item database operations


    public ItemController(ILogger<ItemController> logger, IItemDbRepository iItemDbRepository) // Constructor
    {
        _logger = logger;
        _iItemDbRepository = iItemDbRepository;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetItem(string id)
    {
        var item = await _iItemDbRepository.GetItemById(id); // Henter item fra repository
        
        if (item == null) // Hvis item ikke findes
        {
            return NotFound(); // Returnerer not found
        }
        
        return Ok(item); // Returnerer ok med item
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllItems()
    {
        return null; // ikke implementeret kode endnu
    }

    [HttpPost]
    public async Task<IActionResult> CreateItem(Item item)
    {
        return null; // ikke implementeret kode endnu
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(string id, Item item)
    {
        return null; // ikke implementeret kode endnu
    }
   
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(string id)
    {
        return null; // ikke implementeret kode endnu
    }

    [HttpGet("auctionable")]
    public async Task<IActionResult> GetAuctionableItems(DateTime auctionStart, DateTime auctionEnd)
    {
        return null; // ikke implementeret kode endnu
    }

    [HttpGet("owner/{ownerId}")]
    public async Task<IActionResult> GetItemsByOwnerId(string ownerId)
    {
        return null; // ikke implementeret kode endnu
    }
    
}
