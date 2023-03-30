using System;
using Microsoft.AspNetCore.Mvc;
using ArtworkProvider.Backend.Models;
using ArtworkProvider.Backend.Services;

namespace ArtworkProvider.Backend.Controllers;

[Controller]
[Route("api/1.0/company")]
public class CompanyController : Controller
{
    private readonly CompanyService _CompanyService;
    public CompanyController(CompanyService CompanyService)
    {
        _CompanyService = CompanyService;
    }

    [HttpGet]
    public async Task<List<CompanyModel>> GetCompany()
    {
        return await _CompanyService.GetCompany();
    }

    [HttpPost]
    public async Task<IActionResult> PostCompany([FromBody] CompanyModel CompanyModel)
    {
        await _CompanyService.CreateCompany(CompanyModel);
        return CreatedAtAction(nameof(GetCompany), new { id = CompanyModel.Id }, CompanyModel);
    }


}