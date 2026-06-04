using Microsoft.AspNetCore.Mvc;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        // In-memory store (replaces a real DB for this demo)
        private static List<User> _users = new List<User>
        {
            new User { Id = 1, Name = "Alice Johnson", Email = "alice@example.com", Age = 28, Role = "Admin" },
            new User { Id = 2, Name = "Bob Smith",     Email = "bob@example.com",   Age = 34, Role = "User" },
            new User { Id = 3, Name = "Carol White",   Email = "carol@example.com", Age = 22, Role = "Manager" }
        };

        private static int _nextId = 4;

        private readonly ILogger<UsersController> _logger;

        public UsersController(ILogger<UsersController> logger)
        {
            _logger = logger;
        }

        // ──────────────────────────────────────────────
        // GET /api/users
        // Returns all users
        // ──────────────────────────────────────────────
        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAllUsers()
        {
            _logger.LogInformation("Fetching all users. Count: {Count}", _users.Count);
            return Ok(_users);
        }

        // ──────────────────────────────────────────────
        // GET /api/users/{id}
        // Returns a single user by ID
        // ──────────────────────────────────────────────
        [HttpGet("{id}")]
        public ActionResult<User> GetUserById(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                _logger.LogWarning("User with ID {Id} not found", id);
                return NotFound(new { error = $"User with ID {id} not found." });
            }

            return Ok(user);
        }

        // ──────────────────────────────────────────────
        // POST /api/users
        // Creates a new user (with validation)
        // ──────────────────────────────────────────────
        [HttpPost]
        public ActionResult<User> CreateUser([FromBody] User newUser)
        {
            // ModelState checks all [Required], [EmailAddress], [Range] annotations
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Create user failed: Invalid data");
                return BadRequest(ModelState);
            }

            // Extra validation: duplicate email check
            bool emailExists = _users.Any(u => u.Email.Equals(newUser.Email, StringComparison.OrdinalIgnoreCase));
            if (emailExists)
            {
                return Conflict(new { error = "A user with this email already exists." });
            }

            newUser.Id = _nextId++;
            _users.Add(newUser);

            _logger.LogInformation("Created user: {Name} (ID: {Id})", newUser.Name, newUser.Id);

            return CreatedAtAction(nameof(GetUserById), new { id = newUser.Id }, newUser);
        }

        // ──────────────────────────────────────────────
        // PUT /api/users/{id}
        // Updates an existing user
        // ──────────────────────────────────────────────
        [HttpPut("{id}")]
        public ActionResult<User> UpdateUser(int id, [FromBody] User updatedUser)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Update user failed: Invalid data for ID {Id}", id);
                return BadRequest(ModelState);
            }

            var existingUser = _users.FirstOrDefault(u => u.Id == id);

            if (existingUser == null)
            {
                return NotFound(new { error = $"User with ID {id} not found." });
            }

            // Check duplicate email (exclude current user)
            bool emailTaken = _users.Any(u =>
                u.Email.Equals(updatedUser.Email, StringComparison.OrdinalIgnoreCase) && u.Id != id);

            if (emailTaken)
            {
                return Conflict(new { error = "Another user already has this email." });
            }

            // Apply updates
            existingUser.Name  = updatedUser.Name;
            existingUser.Email = updatedUser.Email;
            existingUser.Age   = updatedUser.Age;
            existingUser.Role  = updatedUser.Role;

            _logger.LogInformation("Updated user ID {Id}", id);

            return Ok(existingUser);
        }

        // ──────────────────────────────────────────────
        // DELETE /api/users/{id}
        // Deletes a user by ID
        // ──────────────────────────────────────────────
        [HttpDelete("{id}")]
        public ActionResult DeleteUser(int id)
        {
            var user = _users.FirstOrDefault(u => u.Id == id);

            if (user == null)
            {
                return NotFound(new { error = $"User with ID {id} not found." });
            }

            _users.Remove(user);
            _logger.LogInformation("Deleted user ID {Id}", id);

            return NoContent(); // 204 — success, no body
        }
    }
}
