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
        try
        {
            var items = await _iItemDbRepository.GetAllItems();// Hent alle items fra repository

            if (items == null || !items.Any())// Hvis der ikke er nogen items, returner en tom liste
            {
                return Ok(new List<Item>());
            }

            return Ok(items);// Returner items med en 200 OK status
        }
        catch (Exception ex)
        {
            // Log eventuelle fejl
            _logger.LogError(ex, "Error occurred while fetching all items.");

            // Returner en generisk fejlmeddelelse
            return StatusCode(500, "An error occurred while processing your request.");
        }
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
