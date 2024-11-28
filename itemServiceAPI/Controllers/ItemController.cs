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
    

    private readonly ILogger<ItemController> _logger;
    private readonly IItemDbRepository _itemDbRepository;


    public ItemController(ILogger<ItemController> logger, IItemDbRepository itemDbRepository)
    {
        _logger = logger;
        _itemDbRepository = itemDbRepository;
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetItem(string id)
    {
        return null; // ikke implementeret kode endnu
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

    
}
