using JWTServiceCore.Models.DTOs;
using JWTServiceCore.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Data;

namespace JWTServiceCore.ApiController
{
    [ApiController]
    [Route("api/authentication")]
    public class AuthenticationController : ControllerBase
    {
        #region Ctor

        private readonly IAuthService _authenticationService;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IClaimService _claimService;

        public AuthenticationController(
            IAuthService authenticationService,
            IUserService userService,
            ITokenService tokenService,
            IClaimService claimService)
        {
            _authenticationService = authenticationService;
            _userService = userService;
            _tokenService = tokenService;
            _claimService = claimService;
        }

        #endregion

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Model is not valid.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            if (userDto.Password != userDto.ConfirmPassword || !userDto.AcceptPolicy)
            {
                return BadRequest(new
                {
                    Message = "Password confirmation does not match or policy not accepted."
                });
            }

            if (await IsAnyUserNameAsync(userDto.UserName!))
            {
                return BadRequest(new 
                {
                    Message = "Username is already registered."
                });
            }

            if (await IsAnyPhoneAsync(userDto.PhoneNumber!))
            {
                return BadRequest(new
                {
                    Message = "Phone number is already registered."
                });
            }

            if (await IsAnyEmailAsync(userDto.Email!))
            {
                return BadRequest(new
                {
                    Message = "Email is already registered."
                });
            }

            var result = await _userService.RegisterUserAsync(userDto);
            if (result.Succeeded)
            {
                var user = await _userService.GetUserByPhoneAsync(userDto.PhoneNumber!);
                if (user == null)
                {
                    return NotFound(new
                    {
                        Message = "User not found."
                    });
                }

                var code = await _authenticationService.GenerateTwoFactorAsync(user, "Phone");
                if (code == "Code not found.")
                {
                    return BadRequest(new
                    { 
                        Message = "Code generation failed."
                    });
                }

                return Ok(new 
                {
                    Message = "User registered successfully.",
                    Phone = userDto.PhoneNumber,
                    Token = code,
                    ResetPassword = false
                });
            }

            return BadRequest(new { Message = "Registration failed.", Errors = result.Errors.Select(e => e.Description) });
        }

        [HttpPost("register-with-phone")]
        public async Task<IActionResult> RegisterWithPhone([FromForm] RegisterWithPhoneDto registerWithPhoneDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Model is not valid.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            if (!registerWithPhoneDto.AcceptPolicy)
            {
                return BadRequest("You must accept the policy.");
            }

            var result = await _userService.RegisterWithPhoneAsync(registerWithPhoneDto);
            if (result.Succeeded)
            {
                var user = await _userService.GetUserByPhoneAsync(registerWithPhoneDto.PhoneNumber!);
                if (user != null)
                {
                    var code = await _authenticationService.GenerateTwoFactorAsync(user, "Phone");
                    if (code != "Code not found.")
                    {
                        return Ok(new
                        {
                            Message = "User registered successfully.",
                            Phone = registerWithPhoneDto.PhoneNumber,
                            Token = code
                        });
                    }
                }
                return BadRequest("Failed to generate two-factor authentication code.");
            }

            var errors = result.Errors.Select(e => e.Description).ToList();

            return BadRequest(new { Errors = errors });
        }

        [HttpPost("confirm-mobile")]
        public async Task<IActionResult> ConfirmMobile([FromForm] ConfirmDto confirmDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Model is not valid.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var user = await _userService.GetUserByPhoneAsync(confirmDto.PhoneNumber!);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            confirmDto.UserId = user.Id;
            confirmDto.TokenProvider = "Phone";

            var result = await _authenticationService.VerifyTwoFactorAsync(confirmDto);
            if (result)
            {
                if (confirmDto.ResetPassword)
                {
                    var resetPasswordDto = new ResetPasswordDto
                    {
                        UserId = user.Id,
                        PhoneNumber = confirmDto.PhoneNumber
                    };

                    var code = await _authenticationService.GeneratePasswordResetAsync(resetPasswordDto);

                    return Ok(new
                    {
                        Message = "Password reset initiated.",
                        ResetToken = code
                    });
                }
                else
                {
                    await _authenticationService.PhoneNumberConfirmedAsync(user.Id!);

                    return Ok(new
                    {
                        Message = "Phone number confirmed. Please login."
                    });
                }
            }
            else
            {
                return BadRequest("The entered code is not valid.");
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Model is not valid.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var user = await GetUserAsync(loginDto.UserName!);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var result = await _authenticationService.CheckPasswordAsync(new ResetPasswordDto
            {
                UserId = user.Id,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                OldPassword = loginDto.Password,
            });

            if (!result)
            {
                return BadRequest("Invalid login attempt.");
            }

            var roles = await _userService.GetUserRolesAsync(user.Id!);

            var claimsList = await GetClaimsForUserAsync(user.Id!, roles);

            var token = _tokenService.GenerateToken(user.Id!, claimsList, roles);

            return Ok(new
            {
                Message = "Login successful.",
                Token = token,
            });
        }

        [HttpPost("login-by-mobile")]
        public async Task<IActionResult> LoginByMobile([FromForm] LoginWithPhoneDto loginWithPhoneDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Model is not valid.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var user = await _userService.GetUserByPhoneAsync(loginWithPhoneDto.Phone!);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            var code = await _authenticationService.GenerateTwoFactorAsync(user, "Phone");
            if (code == "Code not found.")
            {
                return BadRequest("Code generation failed.");
            }

            return Ok(new
            {
                Message = "Two-factor authentication code sent.",
                Phone = user.PhoneNumber,
                Token = code
            });
        }

        [HttpPost("login-confirm-mobile")]
        public async Task<IActionResult> LoginConfirmMobile([FromForm] ConfirmDto confirmDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Model is not valid.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var user = await _userService.GetUserByPhoneAsync(confirmDto.PhoneNumber!);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            confirmDto.UserId = user.Id;
            confirmDto.TokenProvider = "Phone";

            var resultVerify = await _authenticationService.VerifyTwoFactorAsync(confirmDto);
            if (!resultVerify)
            {
                return BadRequest("The entered code is not valid.");
            }

            var roles = await _userService.GetUserRolesAsync(user.Id!);

            var claimsList = await GetClaimsForUserAsync(user.Id!, roles);

            var token = _tokenService.GenerateToken(user.Id!, claimsList!, roles);

            return Ok(new
            {
                Message = "Login successful.",
                Phone = user.PhoneNumber,
                Token = token,
            });
        }


        [HttpPost("logout")]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromForm] ForgotPasswordDto forgotPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Model is not valid.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            if (!string.IsNullOrEmpty(forgotPasswordDto.PhoneNumber))
            {
                var user = await _userService.GetUserByPhoneAsync(forgotPasswordDto.PhoneNumber!);
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                var code = await _authenticationService.GenerateTwoFactorAsync(user, "Phone");
                if (code == "Code not found.")
                {
                    return StatusCode(500, new { message = "Attempt to send code failed." });
                }

                var confirmModel = new ConfirmDto
                {
                    UserId = user.Id,
                    PhoneNumber = user!.PhoneNumber,
                    Email = user.Email,
                    ResetPassword = true,
                    Token = code,
                };

                return Ok(confirmModel);
            }
            else if (!string.IsNullOrEmpty(forgotPasswordDto.Email))
            {
                var user = await _userService.GetUserByEmailAsync(forgotPasswordDto.Email!);
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                var code = await _authenticationService.GenerateTwoFactorAsync(user, "Phone");
                if (code == "Code not found.")
                {
                    return StatusCode(500, new { message = "Attempt to send code failed." });
                }

                var confirmModel = new ConfirmDto
                {
                    UserId = user.Id,
                    PhoneNumber = user!.PhoneNumber,
                    Email = user.Email,
                    ResetPassword = true,
                    Token = code,
                };

                return Ok(confirmModel);
            }

            return BadRequest(new { message = "PhoneNumber or Email is required." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromForm] ResetPasswordDto resetPasswordDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    Message = "Model is not valid.",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });
            }

            var resultPassword = await _authenticationService.CheckPasswordAsync(resetPasswordDto);
            if (resultPassword && resetPasswordDto.NewPassword == resetPasswordDto.ConfirmPassword)
            {
                var result = await _authenticationService.ResetPasswordAsync(resetPasswordDto);
                if (result.Succeeded)
                {
                    return Ok(new { message = "Password reset successful. Please log in." });
                }

                return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
            }

            return BadRequest(new { message = "Password reset failed. Check the provided information." });
        }

        private async Task<UserDto?> GetUserAsync(string identifier)
        {
            var userByName = await _userService.GetUserByUserNameAsync(identifier);
            if (userByName.Id != null) return userByName;

            var userByPhone = await _userService.GetUserByPhoneAsync(identifier);
            if (userByPhone.Id != null) return userByPhone;

            var userByEmail = await _userService.GetUserByEmailAsync(identifier);
            if (userByEmail.Id != null) return userByEmail;

            return null;
        }

        private async Task<List<Claim>> GetClaimsForUserAsync(string userId, IEnumerable<string> roles)
        {
            var claimsList = new List<Claim>();

            foreach (var role in roles)
            {
                var roleClaims = await _claimService.GetClaimsRoleAsync(role);

                claimsList.AddRange(roleClaims.Select(claim => new Claim(claim.Type!, claim.Value!)));
            }

            var userClaims = await _claimService.GetClaimsUserAsync(userId);

            claimsList.AddRange(userClaims.Select(claim => new Claim(claim.Type!, claim.Value!)));

            return claimsList;
        }

        private async Task<bool> IsAnyUserNameAsync(string userName)
        {
            return await _userService.IsAnyUserName(userName);
        }

        private async Task<bool> IsAnyPhoneAsync(string phone)
        {
            return await _userService.IsAnyPhone(phone);
        }

        private async Task<bool> IsAnyEmailAsync(string email)
        {
            return await _userService.IsAnyEmail(email);
        }
    }
}