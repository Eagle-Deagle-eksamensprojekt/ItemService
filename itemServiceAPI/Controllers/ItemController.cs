using Microsoft.AspNetCore.Mvc;
using System;
using ItemServiceAPI.Repositories;


namespace itemServiceAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ItemController : ControllerBase
{
    

    private readonly ILogger<ItemController> _logger;
    private readonly IItemRepository _repository;


    public ItemController(ILogger<ItemController> logger, IItemRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }
    

    
}
