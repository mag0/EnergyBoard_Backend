using EnergyBoard.Application.DTOs.request.cards;
using EnergyBoard.Application.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EnergyBoard.API.Controllers;

[Authorize]
[ApiController]
[Route("api/projects/{projectId:int}/columns/{columnId:int}/cards")]
public class CardController(ICardService cardService) : ControllerBase
{
    private readonly ICardService _cardService = cardService;

    private Guid GetUserId()
        => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    [HttpGet]
    public async Task<IActionResult> GetAll(int projectId, int columnId)
    {
        var userId = GetUserId();
        var cards = await _cardService.GetAllAsync(projectId, columnId, userId);
        return Ok(cards);
    }

    [HttpGet("{cardId:int}")]
    public async Task<IActionResult> GetById(int projectId, int columnId, int cardId)
    {
        var userId = GetUserId();
        var card = await _cardService.GetByIdAsync(projectId, columnId, cardId, userId);
        return Ok(card);
    }

    [HttpPost]
    public async Task<IActionResult> Create(int projectId, int columnId, [FromBody] CreateCardRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        var createdId = await _cardService.AddAsync(projectId, columnId, request, userId);

        return CreatedAtAction(nameof(GetById),
            new { projectId, columnId, cardId = createdId },
            null);
    }

    [HttpPut("{cardId:int}")]
    public async Task<IActionResult> Update(int projectId, int columnId, int cardId, [FromBody] UpdateCardRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        await _cardService.UpdateAsync(projectId, columnId, cardId, request, userId);

        return NoContent();
    }

    [HttpDelete("{cardId:int}")]
    public async Task<IActionResult> Delete(int projectId, int columnId, int cardId)
    {
        var userId = GetUserId();

        await _cardService.DeleteAsync(projectId, columnId, cardId, userId);

        return NoContent();
    }

    [HttpPatch("{cardId:int}/position")]
    public async Task<IActionResult> UpdatePosition(
        int projectId,
        int columnId,
        int cardId,
        [FromBody] MoveCardRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var userId = GetUserId();

        await _cardService.UpdatePositionAsync(projectId, columnId, cardId, request, userId);

        return NoContent();
    }
}