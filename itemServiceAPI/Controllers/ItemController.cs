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
    return null;
}
   
    

    
}
