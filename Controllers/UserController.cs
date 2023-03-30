using System;
using Microsoft.AspNetCore.Mvc;
using ArtworkProvider.Backend.Models;
using ArtworkProvider.Backend.Services;

namespace ArtworkProvider.Backend.Controllers;

[Controller]
[Route("api/1.0/users")]
public class UserController : Controller
{
    private readonly UserService _UserService;
    public UserController(UserService UserService)
    {
        _UserService = UserService;
    }

    [HttpGet]
    public async Task<List<UsersModel>> GetUsers()
    {
        return await _UserService.GetUsers();
    }

    [HttpPost]
    public async Task<IActionResult> PostUser([FromBody] UsersModel UsersModel)
    {
        await _UserService.CreateUser(UsersModel);
        return CreatedAtAction(nameof(GetUsers), new { id = UsersModel.Id }, UsersModel);
    }


}