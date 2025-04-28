using Microsoft.AspNetCore.Mvc;
using OOPClass1.DTO;
using OOPClass1.Models;

namespace OOPClass1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class MockDataController : ControllerBase
    {

        #region Get Users
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            List<User> users = new List<User>();
            using (var reader = new StreamReader("wwwroot/db.csv"))
            {
                

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    if(line == "")
                    {
                        continue;
                    }
                    var values = line.Split(',');

                    users.Add(new User
                    {
                        Id = Guid.Parse(values[0]),
                        Name = values[1],
                        Username = values[2]
                    });
                    
                }
            }
            return Ok(users);
        }
        #endregion

        
        #region New User
        [HttpPost("new-user")]

        public IActionResult AddUser([FromForm]UserDTO user)
        {
            //Users.Add(user);
            using (var writer = new StreamWriter("wwwroot/db.csv",append:true))
            {
                
                writer.WriteLine($"{Guid.NewGuid()},{user.Name},{user.Username}");
            }

            return Ok();
        }

        #endregion
        
        
        #region Delete User
        [HttpDelete("delete-user")]
        public IActionResult DeleteUser([FromForm] string  guid)
        {
            try
            {
                // Validate GUID
                if (!Guid.TryParse(guid, out var deleteGuid))
                {
                    return BadRequest("Invalid GUID format.");
                }

                // Read all users from CSV
                var users = new List<User>();
                var tempFile = Path.GetTempFileName(); // Temporary file to avoid file locking
                bool userFound = false;

                using (var reader = new StreamReader("wwwroot/db.csv"))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }

                        var values = line.Split(',');
                        var userId = Guid.Parse(values[0]);

                        // Skip the user with the matching GUID
                        if (userId == deleteGuid)
                        {
                            userFound = true;
                            continue;
                        }

                        users.Add(new User
                        {
                            Id = userId,
                            Name = values[1],
                            Username = values[2]
                        });
                    }
                }

                if (!userFound)
                {
                    return NotFound("User with specified GUID not found.");
                }

                // Rewrite the CSV with remaining users
                using (var writer = new StreamWriter("wwwroot/db.csv"))
                {
                    foreach (var user in users)
                    {
                        writer.WriteLine($"{user.Id},{user.Name},{user.Username}");
                    }
                }

                return Ok("User deleted successfully.");
            }
            catch (FormatException)
            {
                return BadRequest("Invalid GUID format.");
            }
            
        }
        #endregion
        
        
        #region Update User
        [HttpPut("update-user")]
        public IActionResult UpdateUser([FromForm] User updatedUser)
        {
            try
            {
                // Validate GUID
                

                // Read all users from CSV
                var users = new List<User>();
                var tempFile = Path.GetTempFileName(); // Temporary file to avoid file locking
                bool userFound = false;

                using (var reader = new StreamReader("wwwroot/db.csv"))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line))
                        {
                            continue;
                        }

                        var values = line.Split(',');
                        var userId = Guid.Parse(values[0]);

                        // Skip the user with the matching GUID
                        if (userId == updatedUser.Id)
                        {
                            userFound = true;
                            users.Add(new User
                            {
                                Id = updatedUser.Id,
                                Name = updatedUser.Name,
                                Username = updatedUser.Username
                            });
                            continue;
                        }

                        users.Add(new User
                        {
                            Id = userId,
                            Name = values[1],
                            Username = values[2]
                        });
                    }
                }

                if (!userFound)
                {
                    return NotFound("User with specified GUID not found.");
                }

                // Rewrite the CSV with remaining users
                using (var writer = new StreamWriter("wwwroot/db.csv"))
                {
                    foreach (var user in users)
                    {
                        writer.WriteLine($"{user.Id},{user.Name},{user.Username}");
                    }
                }

                return Ok("User Updated successfully.");
            }
            catch (FormatException)
            {
                return BadRequest("Invalid GUID format.");
            }

        }
        #endregion
    }
}
