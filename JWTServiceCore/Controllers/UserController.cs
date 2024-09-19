using Microsoft.EntityFrameworkCore;
using JWTServiceCore.Models.DTOs;
using JWTServiceCore.Models.ViewModels;
using JWTServiceCore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTServiceCore.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        #region Ctor

        private readonly IAuthService _authenticationService;
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IClaimService _claimService;

        public UserController(
            IAuthService authenticationService,
            IUserService userService,
            IRoleService roleService,
            IClaimService claimService)
        {
            _authenticationService = authenticationService;
            _userService = userService;
            _roleService = roleService;
            _claimService = claimService;
        }

        #endregion

        // GET: User
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();

            var userList = new List<UserViewModel>();
            if (users != null)
            {
                foreach (var user in users)
                {
                    var claims = await _claimService.GetClaimsUserAsync(user.Id!);

                    var claimList = new List<ClaimViewModel>();
                    if (claims != null)
                    {
                        foreach (var claim in claims)
                        {
                            claimList.Add(new ClaimViewModel
                            {
                                Type = claim.Type,
                                Value = claim.Value,
                            });
                        }
                    }

                    userList.Add(new UserViewModel
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        UserName = user.UserName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Roles = _userService.GetUserRolesAsync(user.Id!).Result,
                        Claims = claimList,
                    });
                }
            }

            return View(userList);
        }

        // GET: User Roles
        public async Task<IActionResult> Roles(string userId)
        {
            var roles = await _userService.GetUserRolesAsync(userId);

            var roleList = new List<RoleViewModel>();
            if (roles.Count() > 0)
            {
                foreach (var role in roles)
                {
                    var roleDto = await _roleService.GetRoleByNameAsync(role);

                    var claims = await _claimService.GetClaimsRoleAsync(role);

                    var claimList = new List<ClaimViewModel>();
                    if (claims != null)
                    {
                        foreach (var claim in claims)
                        {
                            claimList.Add(new ClaimViewModel
                            {
                                Type = claim.Type,
                                Value = claim.Value,
                            });
                        }
                    }

                    roleList.Add(new RoleViewModel
                    {
                        Id = roleDto.Id,
                        UserName = _userService.GetUserByIdAsync(userId).Result.UserName,
                        Name = role,
                        Claims = claimList,
                    });
                }
            }
            else
            {
                roleList.Add(new RoleViewModel
                {
                    UserName = _userService.GetUserByIdAsync(userId).Result.UserName,
                });
            }

            return View(roleList);
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userDto = await _userService.GetUserByIdAsync(id);
            if (userDto == null)
            {
                return NotFound();
            }

            var claims = await _claimService.GetClaimsUserAsync(userDto.Id!);

            var claimList = new List<ClaimViewModel>();
            if (claims != null)
            {
                foreach (var claim in claims)
                {
                    claimList.Add(new ClaimViewModel
                    {
                        Type = claim.Type,
                        Value = claim.Value,
                    });
                }
            }

            var model = new UserViewModel
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                UserName = userDto.UserName,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                Roles = _userService.GetUserRolesAsync(userDto.Id!).Result,
                Claims = claimList,
            };

            return View(model);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userDto = new UserDto
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    Password = model.Password,
                    ConfirmPassword = model.Password,
                    AcceptPolicy = true,
                };

                var resultRegisterUser = await _userService.RegisterUserAsync(userDto);
                if (resultRegisterUser.Succeeded)
                {
                    var user = await _userService.GetUserByPhoneAsync(model.PhoneNumber!);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "User not found.");

                        return View();
                    }

                    var code = await _authenticationService.GenerateTwoFactorAsync(user, "Phone");
                    if (code != "Code not found.")
                    {
                        var confirmMobileDto = new ConfirmDto
                        {
                            UserId = user.Id,
                            PhoneNumber = model.PhoneNumber,
                            ResetPassword = false,
                            TokenProvider = "Phone",
                            Code = code,
                        };

                        var resultVerify = await _authenticationService.VerifyTwoFactorAsync(confirmMobileDto);
                        if (resultVerify)
                        {
                            await _authenticationService.PhoneNumberConfirmedAsync(user.Id!);

                            return RedirectToAction("Login", "Auth");
                        }
                    }
                }
            }

            return View(model);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userDto = await _userService.GetUserByIdAsync(id);
            if (userDto == null)
            {
                return NotFound();
            }

            var roleDtos = await _roleService.GetAllRolesAsync();

            var roleList = new List<string>();
            if (roleDtos != null)
            {
                foreach (var role in roleDtos)
                {
                    roleList.Add(role.Name!);
                }
            }

            var claims = await _claimService.GetClaimsUserAsync(userDto.Id!);

            var claimList = new List<ClaimViewModel>();
            if (claims != null)
            {
                foreach (var claim in claims)
                {
                    claimList.Add(new ClaimViewModel
                    {
                        Type = claim.Type,
                        Value = claim.Value,
                    });
                }
            }

            var model = new UpdateProfileViewModel
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                UserName = userDto.UserName,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                Roles = _userService.GetUserRolesAsync(userDto.Id!).Result,
                RoleList = roleList,
                Claims = claimList,
            };

            return View(model);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateProfileViewModel model, List<string> selectedRoles, List<ClaimViewModel> claims)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var updateProfileDto = new UpdateProfileDto
                    {
                        Id = model.Id,
                        FirstName = model.FirstName,
                        LastName = model.LastName,
                        UserName = model.UserName,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                    };

                    var updateResult = await _userService.UpdateUserProfileAsync(updateProfileDto);

                    var userRoles = await _userService.GetUserRolesAsync(model.Id!);
                    if (updateResult.Succeeded && userRoles.Count() > 0)
                    {
                        await _userService.RemoveRolesAsync(model.Id, userRoles);
                    }

                    foreach (var role in selectedRoles)
                    {
                        await _userService.AddRoleToUserAsync(model.Id, role);
                    }

                    var userClaims = await _claimService.GetClaimsUserAsync(model.Id);
                    if (updateResult.Succeeded && userClaims.Count() > 0)
                    {
                        await _claimService.DeleteClaimsForUserAsync(model.Id);
                    }

                    var claimsList = new List<ClaimDto>();
                    foreach (var claim in claims)
                    {
                        claimsList.Add(new ClaimDto
                        {
                            Type = claim.Type,
                            Value = claim.Value,
                        });
                    }

                    await _claimService.UpdateClaimsForUserAsync(claimsList, model.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exist = await UserExists(model.Id);
                    if (!exist)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                if (_authenticationService.IsUserSignedIn())
                {
                    var userId = await _authenticationService.GetSignInUserAsync();
                    if (userId != null && userId == id)
                    {
                        await _authenticationService.RefreshLoginAsync(id);
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userDto = await _userService.GetUserByIdAsync(id);
            if (userDto == null)
            {
                return NotFound();
            }

            var model = new UserViewModel
            {
                Id = userDto.Id,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                UserName = userDto.UserName,
                Email = userDto.Email,
                PhoneNumber = userDto.PhoneNumber,
                Password = userDto.Password,
                Roles = _userService.GetUserRolesAsync(userDto.Id!).Result,
            };

            return View(model);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var userDto = await _userService.GetUserByIdAsync(id);
            if (userDto != null)
            {
                await _userService.DeleteUserAsync(userDto.Id!);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> UserExists(string id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                return false;
            }

            return true;
        }
    }
}
