using System;
using Microsoft.AspNetCore.Mvc;
using ArtworkProvider.Backend.Models;
using ArtworkProvider.Backend.Services;

namespace ArtworkProvider.Backend.Controllers;

[Controller]
[Route("api/1.0/sprints")]
public class SprintController : Controller
{
    private readonly SprintService _SprintService;
    public SprintController(SprintService SprintService)
    {
        _SprintService = SprintService;
    }

    [HttpGet]
    public async Task<List<SprintModel>> GetSprints()
    {
        return await _SprintService.GetSprints();
    }

    [HttpPost]
    public async Task<IActionResult> PostSprint([FromBody] SprintModel SprintModel)
    {
        await _SprintService.CreateSprint(SprintModel);
        return CreatedAtAction(nameof(GetSprints), new { id = SprintModel.Id }, SprintModel);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSprint(string id,[FromBody] SprintModel SprintModel) {
        await _SprintService.UpdateSprint(id, SprintModel);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<SprintModel> GetSingleSprint(string id) {
        return await _SprintService.GetSingleSprint(id);
    }
}