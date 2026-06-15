using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Data;
using UserService.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _repo;

    public UserController(IUserRepository repo)
    {
        _repo = repo;
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
    {
        var user = await _repo.GetByIdAsync(id);
        if (user == null) return NotFound();
        return Ok(new { user.Id, user.Name, user.Email });
    }

    // Público: aceita { name, email?, password } do frontend (proxy)
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest req)
    {
        var password = string.IsNullOrWhiteSpace(req.Password) ? req.PasswordHash : req.Password;

        if (string.IsNullOrWhiteSpace(req.Name) || string.IsNullOrWhiteSpace(password))
            return BadRequest(new { message = "Name and password are required." });

        if (!string.IsNullOrWhiteSpace(req.Email))
        {
            var exists = await _repo.GetByEmailAsync(req.Email);
            if (exists != null) return Conflict(new { message = "Email já cadastrado." });
        }

        var user = new User
        {
            Name = req.Name,
            Email = req.Email ?? string.Empty,
            // salva a senha recebida diretamente no campo PasswordHash (não encriptado, conforme pedido)
            PasswordHash = password
        };

        var id = await _repo.CreateAsync(user);
        // não retornar a senha no corpo
        return CreatedAtAction(nameof(Get), new { id }, new { id, user.Name, user.Email });
    }
}
