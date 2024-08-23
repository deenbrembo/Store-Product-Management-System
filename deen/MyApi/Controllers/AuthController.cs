using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    private IActionResult GenerateTokenResponse(int ID, string email, string role, string status, string name, string phoneNumber)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key"));
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(ClaimTypes.NameIdentifier, ID.ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.MobilePhone, phoneNumber),
                new Claim(ClaimTypes.Role, role)
            ]),
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = _configuration["Jwt:Issuer"],
            Audience = _configuration["Jwt:Audience"],
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);

        return Ok(new
        {
            token = tokenString,
            role,
            status,
            ID,
            name,
            email,
            phoneNumber
        });
    }

        private async Task<string> HandleImageUpload(string imageUrl)
    {
        if (imageUrl.StartsWith("data:image"))
        {
            return imageUrl;
        }
        else
        {
            // Handle file upload and return the image URL
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(imageUrl);
            var filePath = Path.Combine("UPLOAD_DIRECTORY_PATH", uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await fileStream.WriteAsync(Convert.FromBase64String(imageUrl.Split(',')[1]));
            }
            return "URL_OF_SAVED_FILE";
        }
    }


    private string GenerateQRCodeURL(string productName)
    {
        // Replace spaces with dashes and encode the name
        string encodedName = Uri.EscapeDataString(productName.Replace(" ", "-"));
        return $"https://localhost:8080/dashboard/borrowing/qr/{encodedName}";
    }

    [HttpPost("token")]
    public IActionResult ReceiveToken()
    {
        // Get the token from the request header
        string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        // You can now use the token to get the current logged-in user data
        token = token.Replace("Bearer ", "");
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key"));
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _configuration["Jwt:Issuer"],
            ValidAudience = _configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };

        SecurityToken validatedToken;
        var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out validatedToken);

        // For example, you can decode the token and extract the user information
        var id = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        var name = principal.FindFirstValue(ClaimTypes.Name);
        var email = principal.FindFirstValue(ClaimTypes.Email);
        var phoneNumber = principal.FindFirstValue(ClaimTypes.MobilePhone);
        var role = principal.FindFirstValue(ClaimTypes.Role);

        // Return any response as needed
        return Ok(new { id, name, email, phoneNumber, role });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        // Check user in HeadAdmin table
        var headAdmin = await _context.HeadAdmin
            .FirstOrDefaultAsync(h => h.Email == request.Email);

        if (headAdmin != null && BCrypt.Net.BCrypt.Verify(request.Password, headAdmin.PasswordHash))
        {
            if (headAdmin.Email != null)
            {
                return GenerateTokenResponse(headAdmin.ID, headAdmin.Email, "HeadAdmin",headAdmin.Status, headAdmin.Name, headAdmin.PhoneNumber??string.Empty);
            }
            else
            {
                return BadRequest(new { message = "Email is null." });
            }
        }

        // Check user in Admin table
        var admin = await _context.Admin
            .FirstOrDefaultAsync(a => a.Email == request.Email);

        if (admin != null && BCrypt.Net.BCrypt.Verify(request.Password, admin.PasswordHash))
        {
            return GenerateTokenResponse(admin.ID,admin.Email ,"Admin" ,admin.Status, admin.Name, admin.PhoneNumber??string.Empty);
        }

        // Check user in Employee table
        var employee = await _context.Employee
            .FirstOrDefaultAsync(e => e.Email == request.Email);

        if (employee != null && BCrypt.Net.BCrypt.Verify(request.Password, employee.PasswordHash))
        {
            return GenerateTokenResponse(employee.ID, employee.Email,"Employee" , employee.Status, employee.Name, employee.PhoneNumber??string.Empty);
        }

        return Unauthorized(new { message = "Invalid email or password." });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegistrationRequest request)
    {
        // Validate request
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(new { message = "Email and password are required." });
        }

        // Check if the email is already registered
        var existingUser = await _context.UserRegistration
            .FirstOrDefaultAsync(u => u.Email == request.Email);

        if (existingUser != null)
        {
            return Conflict(new { message = "User already registered." });
        }

        // Hash the password
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);

        var user = new UserRegistration
        {
            Name = request.Name ?? string.Empty,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber ?? string.Empty,
            PasswordHash = passwordHash,
            RejectedAt = null,
        };

        _context.UserRegistration.Add(user);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Registration successful. Awaiting approval." });
    }

   [Authorize(Roles = "HeadAdmin")]
    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _context.UserRegistration
            .Select(u => new
            {
                u.ID,
                u.Name,
                u.Email,
                u.PhoneNumber,
                u.Role,
                u.Status
            })
            .ToListAsync();

        return Ok(users);
    }
    
[Authorize  (Roles = "HeadAdmin")]
[HttpPost("approve-user")]
public async Task<IActionResult> ApproveUser([FromBody] ApproveUserRequest request)
{
    // Extract the HeadAdminID and HeadAdminName from the token
    var headAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var headAdminName = User.FindFirstValue(ClaimTypes.Name);

    if (string.IsNullOrEmpty(request.UserID.ToString()) || string.IsNullOrEmpty(request.Role))
    {
        return BadRequest(new { message = "UserID and Role are required." });
    }

    // Find the user registration record
    var userRegistration = await _context.UserRegistration
        .FirstOrDefaultAsync(u => u.ID == request.UserID);

    if (userRegistration == null)
    {
        return NotFound(new { message = "User not found." });
    }

    // Update the role and status in the UserRegistration table
    userRegistration.Role = request.Role;
    userRegistration.Status = "Active";  // Assuming "Active" means approved
    _context.UserRegistration.Update(userRegistration);

    if (request.Role == "Admin")
    {
        // Create or update the Admin table
        var admin = await _context.Admin
            .FirstOrDefaultAsync(a => a.UserID == userRegistration.ID);

        if (admin == null)
        {
            admin = new Admin
            {
                UserID = userRegistration.ID,
                Name = userRegistration.Name,
                Email = userRegistration.Email,
                PhoneNumber = userRegistration.PhoneNumber,
                PasswordHash = userRegistration.PasswordHash,
                CreatedAt = DateTime.UtcNow,
                ActivatedAt = DateTime.UtcNow,
                ApprovedByHeadAdminID = int.TryParse(headAdminId, out int id) ? id : 0,
                ApprovedByName = headAdminName
            };
            _context.Admin.Add(admin);
        }
        else
        {
            admin.Name = userRegistration.Name;
            admin.Email = userRegistration.Email;
            admin.PhoneNumber = userRegistration.PhoneNumber;
            admin.PasswordHash = userRegistration.PasswordHash;
            admin.UpdatedAt = DateTime.UtcNow;
            admin.ActivatedAt = DateTime.UtcNow;
            int.TryParse(headAdminId, out int parsedId);
            admin.ApprovedByHeadAdminID = parsedId;
            admin.ApprovedByName = headAdminName;
            _context.Admin.Update(admin);
        }
    }
    else if (request.Role == "Employee")
    {
        // Create or update the Employee table
        var employee = await _context.Employee
            .FirstOrDefaultAsync(e => e.UserID == userRegistration.ID);

        if (employee == null)
        {
            employee = new Employee
            {
                UserID = userRegistration.ID,
                Name = userRegistration.Name,
                Email = userRegistration.Email,
                PhoneNumber = userRegistration.PhoneNumber,
                PasswordHash = userRegistration.PasswordHash,
                CreatedAt = DateTime.UtcNow,
                ActivatedAt = DateTime.UtcNow,
                ApprovedByHeadAdminID = int.TryParse(headAdminId, out int parsedId) ? parsedId : 0,
                ApprovedByName = headAdminName
            };
            _context.Employee.Add(employee);
        }
        else
        {
            employee.Name = userRegistration.Name;
            employee.Email = userRegistration.Email;
            employee.PhoneNumber = userRegistration.PhoneNumber;
            employee.PasswordHash = userRegistration.PasswordHash;
            employee.UpdatedAt = DateTime.UtcNow;
            employee.ActivatedAt = DateTime.UtcNow;
            int.TryParse(headAdminId, out int parsedId);
            employee.ApprovedByHeadAdminID = parsedId;
            employee.ApprovedByName = headAdminName;
            _context.Employee.Update(employee);
        }
    }
    else
    {
        return BadRequest(new { message = "Invalid role specified." });
    }

    await _context.SaveChangesAsync();

    return Ok(new { message = "User approved successfully." });
}

[Authorize(Roles = "HeadAdmin")]
[HttpPost("reject-user")]
public async Task<IActionResult> RejectUser([FromBody] RejectRequest request)
{
    // Extract the HeadAdminID and HeadAdminName from the token
    var headAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var headAdminName = User.FindFirstValue(ClaimTypes.Name);

    if (string.IsNullOrEmpty(request.UserID.ToString()))
    {
        return BadRequest(new { message = "UserID is required." });
    }

    // Find the user registration record
    var userRegistration = await _context.UserRegistration
        .FirstOrDefaultAsync(u => u.ID == request.UserID);

    if (userRegistration == null)
    {
        return NotFound(new { message = "User not found." });
    }

    // Update the status and rejected details in the UserRegistration table
    userRegistration.Status = "Rejected";
    userRegistration.RejectedAt = DateTime.UtcNow;
    userRegistration.RejectedByHeadAdminID = int.TryParse(headAdminId, out int id) ? id : 0;
    userRegistration.RejectedByName = headAdminName;
    _context.UserRegistration.Update(userRegistration);

    await _context.SaveChangesAsync();

    return Ok(new { message = "User rejected successfully." });
}

[Authorize(Roles = "HeadAdmin")]
[HttpGet("employees")]
public async Task<IActionResult> GetEmployees()
{
    var employees = await _context.Employee
        .Select(e => new
        {
            e.ID,
            e.UserID,
            e.Name,
            e.Email,
            e.PhoneNumber,
            e.Role,
            e.Status
        })
        .ToListAsync();

    return Ok(employees);
}

[Authorize (Roles = "HeadAdmin")]
[HttpPost("deactivate-employee")]
public async Task<IActionResult> DeactivateEmployee([FromBody] DeactivateRequest request)
{
    // Extract the HeadAdminID and HeadAdminName from the token
    var headAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var headAdminName = User.FindFirstValue(ClaimTypes.Name);

    if (string.IsNullOrEmpty(request.UserID.ToString()))
    {
        return BadRequest(new { message = "UserID is required." });
    }

    // Find the employee record
    var employee = await _context.Employee
        .FirstOrDefaultAsync(e => e.ID == request.UserID);

    if (employee == null)
    {
        return NotFound(new { message = "Employee not found." });
    }

    // Update the status and deactivation details in the Employee table
    employee.Status = "Deactivated";
    employee.DeactivatedAt = DateTime.UtcNow;
    employee.DeactivatedByHeadAdminID = int.TryParse(headAdminId, out int id) ? id : 0;
    employee.DeactivatedByName = headAdminName;
    _context.Employee.Update(employee);

    await _context.SaveChangesAsync();

    return Ok(new { message = "Employee deactivated successfully." });
}

[Authorize (Roles = "HeadAdmin")]
[HttpPost("activate-employee")]
public async Task<IActionResult> ActivateEmployee([FromBody] ActivateRequest request)
{
    // Extract the HeadAdminID and HeadAdminName from the token
    var headAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var headAdminName = User.FindFirstValue(ClaimTypes.Name);

    if (string.IsNullOrEmpty(request.UserID.ToString()))
    {
        return BadRequest(new { message = "UserID is required." });
    }

    // Find the employee record
    var employee = await _context.Employee
        .FirstOrDefaultAsync(e => e.ID == request.UserID);

    if (employee == null)
    {
        return NotFound(new { message = "Employee not found." });
    }

    // Update the status and activation details in the Employee table
    employee.Status = "Active";
    employee.ActivatedAt = DateTime.UtcNow;
    employee.ActivatedByHeadAdminID = int.TryParse(headAdminId, out int id) ? id : 0;
    employee.ActivatedByName = headAdminName;
    _context.Employee.Update(employee);

    await _context.SaveChangesAsync();

    return Ok(new { message = "Employee activated successfully." });
}

[Authorize(Roles = "HeadAdmin")]
[HttpGet("admins")]
public async Task<IActionResult> GetAdmins()
{
    var admins = await _context.Admin
        .Select(a => new
        {
            a.ID,
            a.UserID,
            a.Name,
            a.Email,
            a.PhoneNumber,
            a.Role,
            a.Status
        })
        .ToListAsync();

    return Ok(admins);
}

[Authorize (Roles = "HeadAdmin")]
[HttpPost("deactivate-admin")]
public async Task<IActionResult> DeactivateAdmin([FromBody] DeactivateRequest request)
{
    // Extract the HeadAdminID and HeadAdminName from the token
    var headAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var headAdminName = User.FindFirstValue(ClaimTypes.Name);

    if (string.IsNullOrEmpty(request.UserID.ToString()))
    {
        return BadRequest(new { message = "UserID is required." });
    }

    // Find the admin record
    var admin = await _context.Admin
        .FirstOrDefaultAsync(a => a.ID == request.UserID);

    if (admin == null)
    {
        return NotFound(new { message = "Admin not found." });
    }

    // Update the status and deactivation details in the Admin table
    admin.Status = "Deactivated";
    admin.DeactivatedAt = DateTime.UtcNow;
    admin.DeactivatedByHeadAdminID = int.TryParse(headAdminId, out int id) ? id : 0;
    admin.DeactivatedByName = headAdminName;
    _context.Admin.Update(admin);

    await _context.SaveChangesAsync();

    return Ok(new { message = "Admin deactivated successfully." });

}

[Authorize (Roles = "HeadAdmin")]
[HttpPost("activate-admin")]
public async Task<IActionResult> ActivateAdmin([FromBody] ActivateRequest request)
{
    // Extract the HeadAdminID and HeadAdminName from the token
    var headAdminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var headAdminName = User.FindFirstValue(ClaimTypes.Name);

    if (string.IsNullOrEmpty(request.UserID.ToString()))
    {
        return BadRequest(new { message = "UserID is required." });
    }

    // Find the admin record
    var admin = await _context.Admin
        .FirstOrDefaultAsync(a => a.ID == request.UserID);

    if (admin == null)
    {
        return NotFound(new { message = "Admin not found." });
    }

    // Update the status and activation details in the Admin table
    admin.Status = "Active";
    admin.ActivatedAt = DateTime.UtcNow;
    admin.ActivatedByHeadAdminID = int.TryParse(headAdminId, out int id) ? id : 0;
    admin.ActivatedByName = headAdminName;
    _context.Admin.Update(admin);

    await _context.SaveChangesAsync();

    return Ok(new { message = "Admin activated successfully." });
}

[Authorize]
[HttpGet("products")]
public async Task<IActionResult> GetAllProducts()
{
    var products = await _context.Product.ToListAsync();

    return Ok(products);
}


[Authorize(Roles = "Admin")]
[HttpPost("add-product")]
public async Task<IActionResult> AddProduct([FromBody] AddProductDto productDto)
{
    // Extract the AdminID and AdminName from the token
    var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var adminName = User.FindFirstValue(ClaimTypes.Name);

    // Validate required fields
    if (string.IsNullOrEmpty(productDto.ProductName) || 
        string.IsNullOrEmpty(productDto.Description) || 
        productDto.QuantityAvalaible <= 0 || 
        string.IsNullOrEmpty(productDto.ImageUrl))
    {
        return BadRequest(new { message = "ProductName, Description, QuantityAvalaible, and ImageUrl are required." });
    }

    // Create a new Product entity
    var product = new Product
    {
        ProductName = productDto.ProductName,
        Description = productDto.Description,
        QuantityAvalaible = productDto.QuantityAvalaible,
        ImageUrl = await HandleImageUpload(productDto.ImageUrl),
        QRcode = GenerateQRCodeURL(productDto.ProductName),
        Status = "Available",
        CreatedAt = DateTime.UtcNow != DateTime.MinValue ? DateTime.UtcNow : new DateTime(1753, 1, 1),
        CreatedByID = int.TryParse(adminId, out int id) ? id : 0,
        CreatedByName = adminName
    };



    // Add the product to the database
    _context.Product.Add(product);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Product added successfully." });
}

[Authorize(Roles = "Admin")]
[HttpPost("update-product")]
public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductDto productDto)
{
    // Extract the AdminID and AdminName from the token
    var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var adminName = User.FindFirstValue(ClaimTypes.Name);

    // Validate required fields
    if (string.IsNullOrEmpty(productDto.ProductName) || 
        string.IsNullOrEmpty(productDto.Description) || 
        productDto.QuantityAvalaible <= 0 || 
        string.IsNullOrEmpty(productDto.ImageUrl))
    {
        return BadRequest(new { message = "ProductName, Description, QuantityAvalaible, and ImageUrl are required." });
    }

    // Find the product record
    var existingProduct = await _context.Product
        .FirstOrDefaultAsync(p => p.ID == productDto.ID);

    if (existingProduct == null)
    {
        return NotFound(new { message = "Product not found." });
    }

    // Update the fields
    existingProduct.ProductName = productDto.ProductName;
    existingProduct.Description = productDto.Description;
    existingProduct.QuantityAvalaible = productDto.QuantityAvalaible;
    existingProduct.ImageUrl = await HandleImageUpload(productDto.ImageUrl);
    existingProduct.UpdatedAt = DateTime.UtcNow != DateTime.MinValue ? DateTime.UtcNow : new DateTime(1753, 1, 1);
    existingProduct.UpdatedByID = int.TryParse(adminId, out int id) ? id : 0;
    existingProduct.UpdatedByName = adminName;

    _context.Product.Update(existingProduct);
    await _context.SaveChangesAsync();

    return Ok(new { message = "Product updated successfully." });
}

[Authorize (Roles = "Admin")]
[HttpPost("remove-product")]
public async Task<IActionResult> RemoveProduct([FromBody] RemoveProductDto request)
{
    var adminId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var adminName = User.FindFirstValue(ClaimTypes.Name);


    // Find the product record
    var product = await _context.Product
        .FirstOrDefaultAsync(p => p.ID == request.ID);

    if (product == null)
    {
        return NotFound(new { message = "Product not found." });
    }

    // Update the status and deactivation details in the Product table
    product.Status = "Removed";
    product.DeactivatedAt = DateTime.UtcNow != DateTime.MinValue ? DateTime.UtcNow : new DateTime(1753, 1, 1);
    product.DeactivatedByID = int.TryParse(adminId, out int id) ? id : 0;
    product.DeactivatedByName = adminName;
    _context.Product.Update(product);

    await _context.SaveChangesAsync();

    return Ok(new { message = "Product removed successfully." });

}


[Authorize]
[HttpPost("manage-profile")]
public async Task<IActionResult> ManageProfile([FromBody] ManageProfileRequest request)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var userRole = User.FindFirstValue(ClaimTypes.Role);

    if (string.IsNullOrEmpty(request.Name) && string.IsNullOrEmpty(request.Email) &&
        string.IsNullOrEmpty(request.PhoneNumber) && string.IsNullOrEmpty(request.Password))
    {
        return BadRequest(new { message = "At least one field (Name, Email, PhoneNumber, Password) must be provided." });
    }

    if (userRole == "HeadAdmin")
    {
        int.TryParse(userId, out int parsedUserId);
        var headAdmin = await _context.HeadAdmin.FirstOrDefaultAsync(ha => ha.ID == parsedUserId);
        if (headAdmin == null)
        {
            return NotFound(new { message = "HeadAdmin not found." });
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            headAdmin.Name = request.Name;
        }

        if (!string.IsNullOrEmpty(request.Email))
        {
            headAdmin.Email = request.Email;
        }

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            headAdmin.PhoneNumber = request.PhoneNumber;
        }

        if (!string.IsNullOrEmpty(request.Password))
        {
            // TODO: Implement password update logic for HeadAdmin
            headAdmin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }

        headAdmin.UpdatedAt = DateTime.UtcNow != DateTime.MinValue ? DateTime.UtcNow : new DateTime(1753, 1, 1);

        _context.HeadAdmin.Update(headAdmin);
        await _context.SaveChangesAsync();

        return Ok(new { message = "HeadAdmin profile updated successfully." });
    }
    else if (userRole == "Admin")
    {
        int.TryParse(userId, out int parsedUserId);
        var admin = await _context.Admin.FirstOrDefaultAsync(a => a.ID == parsedUserId);
        if (admin == null)
        {
            return NotFound(new { message = "Admin not found." });
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            admin.Name = request.Name;
        }

        if (!string.IsNullOrEmpty(request.Email))
        {
            admin.Email = request.Email;
        }

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            admin.PhoneNumber = request.PhoneNumber;
        }

        if (!string.IsNullOrEmpty(request.Password))
        {
            // TODO: Implement password update logic for Admin
            admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        
        }

        admin.UpdatedAt = DateTime.UtcNow != DateTime.MinValue ? DateTime.UtcNow : new DateTime(1753, 1, 1);

        _context.Admin.Update(admin);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Admin profile updated successfully." });
    }
    else if (userRole == "Employee")
    {
        int.TryParse(userId, out int parsedUserId);
        var employee = await _context.Employee.FirstOrDefaultAsync(e => e.ID == parsedUserId);
        if (employee == null)
        {
            return NotFound(new { message = "Employee not found." });
        }

        if (!string.IsNullOrEmpty(request.Name))
        {
            employee.Name = request.Name;
        }

        if (!string.IsNullOrEmpty(request.Email))
        {
            employee.Email = request.Email;
        }

        if (!string.IsNullOrEmpty(request.PhoneNumber))
        {
            employee.PhoneNumber = request.PhoneNumber;
        }

        if (!string.IsNullOrEmpty(request.Password))
        {
            // TODO: Implement password update logic for Employee
            employee.PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        }

        employee.UpdatedAt = DateTime.UtcNow != DateTime.MinValue ? DateTime.UtcNow : new DateTime(1753, 1, 1);

        _context.Employee.Update(employee);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Employee profile updated successfully." });
    }
    else
    {
        return BadRequest(new { message = "Invalid user role." });
    }
}

[Authorize (Roles = "Employee")]
[HttpPost("employee-borrow-product")]
public async Task<IActionResult> BorrowProduct([FromBody] BorrowProductDto request)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var userName = User.FindFirstValue(ClaimTypes.Name);
    var userRole = User.FindFirstValue(ClaimTypes.Role);

    if (request.ID <= 0 || request.Quantity <= 0)
    {
        return BadRequest(new { message = "ProductID, Quantity, or UserID are invalid." });
    }

    var product = await _context.Product
        .FirstOrDefaultAsync(p => p.ID == request.ID);

    if (product == null)
    {
        return NotFound(new { message = "Product not found." });
    }

    if (product.QuantityAvalaible < request.Quantity)
    {
        return BadRequest(new { message = "Insufficient quantity available." });
    }

    product.QuantityAvalaible -= request.Quantity;
    _context.Product.Update(product);

    var borrowing = new Borrowing
    {
        EmployeeID = userRole == "Employee" ? int.TryParse(userId, out int id) ? id : 0 : 0,
        EmployeeName = userRole == "Employee" ? User.FindFirstValue(ClaimTypes.Name) ?? string.Empty : string.Empty,
        ProductID = product.ID,
        ProductName = product.ProductName,
        Quantity = request.Quantity,
        BorrowedAt = DateTime.UtcNow,
        Status = "Borrowed"
    };

        _context.Borrowing.Add(borrowing);
        await _context.SaveChangesAsync();
        return Ok(new { message = "Product borrowed successfully." });

}

[Authorize(Roles = "Admin")]
[HttpPost("admin-borrow-product")]
public async Task<IActionResult> AdminBorrowProduct([FromBody] BorrowProductDto request)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var userName = User.FindFirstValue(ClaimTypes.Name);
    var userRole = User.FindFirstValue(ClaimTypes.Role);

    if (request.ID <= 0 || request.Quantity <= 0)
    {
        return BadRequest(new { message = "ProductID, Quantity, or UserID are invalid." });
    }

    var product = await _context.Product
        .FirstOrDefaultAsync(p => p.ID == request.ID);

    if (product == null)
    {
        return NotFound(new { message = "Product not found." });
    }

    if (product.QuantityAvalaible < request.Quantity)
    {
        return BadRequest(new { message = "Insufficient quantity available." });
    }

    product.QuantityAvalaible -= request.Quantity;
    _context.Product.Update(product);

    var borrowing = new Borrowing
    {
        AdminID = userRole == "Admin" ? int.TryParse(userId, out int id) ? id : 0 : 0,
        AdminName = userRole == "Admin" ? User.FindFirstValue(ClaimTypes.Name) ?? string.Empty : string.Empty,
        ProductID = product.ID,
        ProductName = product.ProductName,
        Quantity = request.Quantity,
        BorrowedAt = DateTime.UtcNow,
        Status = "Borrowed"
    };

    _context.Borrowing.Add(borrowing);
    await _context.SaveChangesAsync();
    return Ok(new { message = "Product borrowed successfully." });

}

[Authorize(Roles = "Employee")]
[HttpPost("employee-return-product")]
public async Task<IActionResult> ReturnProduct([FromBody] ReturnProductDto request)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var userName = User.FindFirstValue(ClaimTypes.Name);
    var userRole = User.FindFirstValue(ClaimTypes.Role);

    if (request.ID <= 0 || request.Quantity <= 0)
    {
        return BadRequest(new { message = "ProductID, Quantity, or UserID are invalid." });
    }

    var borrowing = await _context.Borrowing
        .FirstOrDefaultAsync(b => b.ID == request.ID && b.Status == "Borrowed");

    if (borrowing == null)
    {
        return NotFound(new { message = "Borrowing record not found." });
    }

    var product = await _context.Product
        .FirstOrDefaultAsync(p => p.ID == borrowing.ProductID);

    if (product == null)
    {
        return NotFound(new { message = "Product not found." });
    }

    if (request.Quantity > borrowing.Quantity)
    {
        return BadRequest(new { message = "Invalid quantity. Cannot return more than borrowed." });
    }

    product.QuantityAvalaible += request.Quantity;
    _context.Product.Update(product);

    borrowing.Quantity -= request.Quantity;
    if (borrowing.Quantity == 0)
    {
        borrowing.Status = "Returned";
        borrowing.ReturnedAt = DateTime.UtcNow;
    }

    _context.Borrowing.Update(borrowing);
    await _context.SaveChangesAsync();
    return Ok(new { message = "Product returned successfully." });
}

[Authorize(Roles = "Admin")]
[HttpPost("admin-return-product")]
public async Task<IActionResult> AdminReturnProduct([FromBody] ReturnProductDto request)
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    var userName = User.FindFirstValue(ClaimTypes.Name);
    var userRole = User.FindFirstValue(ClaimTypes.Role);

    if (request.ID <= 0 || request.Quantity <= 0)
    {
        return BadRequest(new { message = "ProductID, Quantity, or UserID are invalid." });
    }

    var borrowing = await _context.Borrowing
        .FirstOrDefaultAsync(b => b.ID == request.ID && b.Status == "Borrowed");

    if (borrowing == null)
    {
        return NotFound(new { message = "Borrowing record not found." });
    }

    var product = await _context.Product
        .FirstOrDefaultAsync(p => p.ID == borrowing.ProductID);

    if (product == null)
    {
        return NotFound(new { message = "Product not found." });
    }

    if (request.Quantity > borrowing.Quantity)
    {
        return BadRequest(new { message = "Invalid quantity. Cannot return more than borrowed." });
    }

    product.QuantityAvalaible += request.Quantity;
    _context.Product.Update(product);

    borrowing.Quantity -= request.Quantity;
    if (borrowing.Quantity == 0)
    {
        borrowing.Status = "Returned";
        borrowing.ReturnedAt = DateTime.UtcNow;
    }

    _context.Borrowing.Update(borrowing);
    await _context.SaveChangesAsync();
    return Ok(new { message = "Product returned successfully." });
}

[Authorize]
[HttpGet("borrowings")]
public async Task<IActionResult> GetBorrowings()
{
    var borrowings = await _context.Borrowing.ToListAsync();

    return Ok(borrowings);

}

[Authorize (Roles = "Employee")]
[HttpGet("my-borrowings")]
public async Task<IActionResult> GetMyBorrowings()
{
    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
    int parsedUserId;
    var borrowings = await _context.Borrowing
        .Where(b => int.TryParse(userId, out parsedUserId) && b.EmployeeID == parsedUserId)
        .ToListAsync();

    return Ok(borrowings);
}
}




