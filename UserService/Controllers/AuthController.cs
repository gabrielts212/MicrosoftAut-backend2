using Microsoft.AspNetCore.Mvc;
using UserService.Data;
using UserService.Models;

namespace UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUserRepository _repo;

    public AuthController(IUserRepository repo)
    {
        _repo = repo;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest req)
    {
        if (string.IsNullOrWhiteSpace(req.Name) || string.IsNullOrWhiteSpace(req.Password))
            return BadRequest(new { message = "Name and password are required." });

        var user = await _repo.GetByNameAsync(req.Name);
        if (user == null)
            return Unauthorized(new { message = "Invalid credentials." });

        // Currently passwords are stored in plain text (PasswordHash field), compare directly
        if (user.PasswordHash != req.Password)
            return Unauthorized(new { message = "Invalid credentials." });

        // Return simple response indicating success. In future we can return JWT.
        return Ok(new { id = user.Id, name = user.Name, email = user.Email });
    }
}
