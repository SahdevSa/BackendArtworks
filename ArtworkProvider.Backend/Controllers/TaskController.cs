using System;
using Microsoft.AspNetCore.Mvc;
using ArtworkProvider.Backend.Models;
using ArtworkProvider.Backend.Services;

namespace ArtworkProvider.Backend.Controllers;

[Controller]
[Route("api/1.0/tasks")]
public class TaskController : Controller
{
    private readonly TaskService _TaskService;
    public TaskController(TaskService TaskService)
    {
        _TaskService = TaskService;
    }

    [HttpGet]
    public async Task<List<TaskModel>> GetTasks()
    {
        return await _TaskService.GetTasks();
    }

    [HttpPost]
    public async Task<IActionResult> PostTask([FromBody] TaskModel TaskModel)
    {
        await _TaskService.CreateTask(TaskModel);
        return CreatedAtAction(nameof(GetTasks), new { id = TaskModel.Id }, TaskModel);
    }


}