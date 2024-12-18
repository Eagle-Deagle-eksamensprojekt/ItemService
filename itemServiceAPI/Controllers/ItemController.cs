using Microsoft.AspNetCore.Mvc;
using ItemServiceAPI.Services;
using ItemServiceAPI.Models;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;




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

    [AllowAnonymous]
    [HttpGet("version")]
        public async Task<IActionResult> GetVersion()
        {
            var properties = new Dictionary<string, string>();

            var ver = FileVersionInfo.GetVersionInfo(
                typeof(Program).Assembly.Location).ProductVersion ?? "N/A";
            properties.Add("version", ver);

            return Ok(new {properties});
        }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetItem(string id) // ændre it til itemId i hele controlleren
    {
        var item = await _iItemDbRepository.GetItemById(id); // Henter item fra repository
        
        if (item == null) // Hvis item ikke findes
        {
            _logger.LogWarning("Item not found.");
            return NotFound(); // Returnerer not found
        }
        
        _logger.LogInformation("Item found.");
        return Ok(item); // Returnerer ok med item
    }

    [AllowAnonymous]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllItems()
    {
        try
        {
            var items = await _iItemDbRepository.GetAllItems();// Hent alle items fra repository

            if (items == null || !items.Any())// Hvis der ikke er nogen items, returner en tom liste
            {
                _logger.LogWarning("No items found.");
                return Ok(new List<Item>()); // Returner en tom liste med en 200 OK status
            }

            _logger.LogInformation("Items found.");
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

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateItem(Item item)
    {
        if (item == null) // Tjek for null input
        {
            _logger.LogWarning("Item cannot be null.");
            return BadRequest("Item cannot be null.");
        }

        // Conflict check
        var itemConflict = await _iItemDbRepository.GetItemById(item.Id!); // Tjekker om item allerede findes
        if (itemConflict != null)
        {
            _logger.LogWarning("An item with the same ID already exists.");  
            return Conflict("An item with the same ID already exists.");
        }

        // Create item
        var itemSuccess = await _iItemDbRepository.CreateItem(item);
        if (itemSuccess)
        {
            _logger.LogInformation("Item created successfully.");
            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item); // Returnerer item med 201 Created status
        }
        else
        {
            _logger.LogError("An error occurred while creating the item.");
            return StatusCode(500, "An error occurred while creating the item.");
        }
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateItem(string id, Item item)
    {
        if (item == null) // Tjek for null input
        {
            _logger.LogWarning("Item cannot be null.");
            return BadRequest("Item cannot be null.");
        }

        if (id != item.Id) // Tjek for ID mismatch
        {
            _logger.LogWarning("ID mismatch.");
            return BadRequest("ID mismatch.");
        }

        // Conflict check
        var itemConflict = await _iItemDbRepository.GetItemById(item.Id); // Tjekker om item allerede findes
        if (itemConflict == null) // Hvis item ikke findes
        {
            _logger.LogWarning("Item not found.");
            return NotFound("Item not found."); // Returnerer 404 not found
        }
        

        // Update item
        var itemSuccess = await _iItemDbRepository.UpdateItem(item); // Opdaterer item i repository
        if (itemSuccess)
        {
            _logger.LogInformation("Item updated successfully.");
            return Ok(item); // Returnerer item med 200 OK status
            
        }
        else
        {
            _logger.LogError("An error occurred while updating the item.");
            return StatusCode(500, "An error occurred while updating the item."); 
        }
    }
   
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteItem(string id)
    {
        var item = await _iItemDbRepository.GetItemById(id); // Henter item fra repository

        if (item == null) // Hvis item ikke findes
        {
            _logger.LogWarning("Item not found.");
            return NotFound(); // Returnerer 404 not found
        }

        var itemSuccess = await _iItemDbRepository.DeleteItem(id); // Sletter item fra repository
        if (itemSuccess)
        {
            _logger.LogInformation("Item deleted successfully.");
            return NoContent(); // Returnerer 204 No Content status
        }
        else
        {
            _logger.LogError("An error occurred while deleting the item.");
            return StatusCode(500, "An error occurred while deleting the item.");
        }
    }
    
    [Authorize]
    [HttpGet("owner/{ownerId}")]
    public async Task<IActionResult> GetItemsByOwnerId(string ownerId)
    {
        try
        {
            var items = await _iItemDbRepository.GetItemsByOwnerId(ownerId); 

            if (items == null) // Hvis ingen items returneres fra repository
            {
                _logger.LogWarning("Owner ID not found.");
                return NotFound(); // Returner 404 Not Found
            }

            if (!items.Any()) // Hvis listen er tom
            {
                _logger.LogInformation("No items found for the owner.");
                return Ok(new List<Item>()); // Returnerer en tom liste
            }

            _logger.LogInformation("Items found for the owner.");
            return Ok(items); // Returnerer items med 200 OK
        }
        catch (Exception ex) // Håndter eventuelle fejl
        {
            _logger.LogError(ex, "An error occurred while fetching items by owner ID.");
            return StatusCode(500, "An error occurred while processing your request.");
        }
    }
}
