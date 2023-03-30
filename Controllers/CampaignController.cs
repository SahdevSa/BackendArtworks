using System;
using Microsoft.AspNetCore.Mvc;
using ArtworkProvider.Backend.Models;
using ArtworkProvider.Backend.Services;

namespace ArtworkProvider.Backend.Controllers;

[Controller]
[Route("api/1.0/campaigns")]
public class CampaignController : Controller
{
    private readonly CampaignService _CampaignService;
    public CampaignController(CampaignService CampaignService)
    {
        _CampaignService = CampaignService;
    }

    [HttpGet]
    public async Task<List<CampaignModel>> GetCampaigns()
    {
        return await _CampaignService.GetCampaigns();
    }

    [HttpPost]
    public async Task<IActionResult> PostCampaign([FromBody] CampaignModel CompaignModel)
    {
        await _CampaignService.CreateCampaign(CompaignModel);
        return CreatedAtAction(nameof(GetCampaigns), new { id = CompaignModel.Id }, CompaignModel);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCampaign(string id, [FromBody] CampaignModel CompaignModel)
    {
        await _CampaignService.UpdateCampaign(id, CompaignModel);
        return NoContent();
    }

    [HttpGet("{id}")]
    public async Task<CampaignModel> GetSingleCampaigns(string id)
    {
        return await _CampaignService.GetSingleCampaigns(id);
    }
}