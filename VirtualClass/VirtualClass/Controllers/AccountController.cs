using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VirtualClass.DAL;
using VirtualClass.DAL.Entities;
using VirtualClass.DLL.JWT;
using VirtualClass.DLL.Validator;
using VirtualClass.Models;
using VirtualClass.Models.Result;

namespace VirtualClass.Controllers
{
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<DbUser> _userManager;
        private readonly SignInManager<DbUser> _signInManager;
        private readonly IJWTTokenService _JwtTokenService;
        private readonly EFContext _context;

        public AccountController(UserManager<DbUser> userManager, SignInManager<DbUser> signInManager, IJWTTokenService jwtTokenService, EFContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _JwtTokenService = jwtTokenService;
            _context = context;
        }

        [HttpPost("register")]
        public async Task<ResultDTO> RegisterCustom([FromBody] RegisterUserModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new ResultErrorDTO()
                    {
                        Status = 401,
                        Message = "ERROR",
                        Errors = CustomValidator.getErrorsByModel(ModelState)
                    };
                }

                var user = new DbUser()
                {
                    UserName = model.Email,
                    Email = model.Email,
                    PhoneNumber = model.Phone,
                    FullName = model.FullName,
                    //BirthDate = model.BirthDate,
                    Image = model.Image,
                    Address = model.Address
                };

                IdentityResult result = await _userManager.CreateAsync(user, model.Password);
                result = await _userManager.AddToRoleAsync(user, _context.Roles.FirstOrDefault(x => x.Name == "Student").Name);

                if (result.Succeeded)
                {
                    result = _userManager.AddToRoleAsync(user, _context.Roles.FirstOrDefault(x => x.Name == "Student").Name).Result;
                    _context.Users.Attach(user);
                    _context.SaveChanges();

                    return new ResultDTO()
                    {
                        Message = "OK",
                        Status = 200
                    };
                }
                else
                {
                    return new ResultErrorDTO()
                    {
                        Message = "ERROR",
                        Status = 403,
                        Errors = CustomValidator.getErrorsByIdentityResult(result)
                    };
                }
            }
            catch (Exception e)
            {
                return new ResultErrorDTO
                {
                    Status = 500,
                    Message = e.Message,
                    Errors = new List<string>()
                    {
                        e.Message
                    }
                };
            }

        }

        //localhost:12312/api/User/login
        [HttpPost("login")]
        public async Task<ResultDTO> Login([FromBody] LoginUserModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return new ResultErrorDTO
                    {
                        Message = "ERROR",
                        Status = 401,
                        Errors = CustomValidator.getErrorsByModel(ModelState)
                    };
                }

                var result = _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false).Result;

                if (!result.Succeeded)
                {
                    return new ResultErrorDTO
                    {
                        Status = 403,
                        Message = "ERROR",
                        Errors = new List<string> { "Incorrect email or password" }
                    };
                }
                else
                {
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    await _signInManager.SignInAsync(user, false);


                    return new ResultLoginDTO
                    {
                        Status = 200,
                        Message = "OK",
                        Token = _JwtTokenService.CreateToken(user)
                    };
                }
            }
            catch (Exception e)
            {
                return new ResultErrorDTO
                {
                    Status = 500,
                    Message = "ERROR",
                    Errors = new List<string> { e.Message }
                };
            }
        }

        [HttpGet("getUserById")]
        public RegisterUserModel GetImageByEmail([FromQuery] string email)
        {
            var user = _context.Users.FirstOrDefault(x => x.Email == email);
            if (user == null)
            {
                return new RegisterUserModel()
                {
                    FullName = "User not found !",
                    Email = "Enter correct email or register this user",
                };
            }
            var newUser = new RegisterUserModel()
            {
                Id = user.Id.ToString(),
                FullName = user.FullName,
                Address = user.Address,
                Email = user.Email,
                Image = user.Image,
                Phone = user.PhoneNumber,
                Password = user.PasswordHash
            };
            return newUser;
        }
    }
}
