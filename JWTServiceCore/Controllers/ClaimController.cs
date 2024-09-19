using JWTServiceCore.Models.DTOs;
using JWTServiceCore.Models.ViewModels;
using JWTServiceCore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTServiceCore.Controllers
{
    [Authorize]
    public class ClaimController : Controller
    {
        #region Ctor

        private readonly IUserService _userService;
        private readonly IClaimService _claimService;

        public ClaimController(IUserService userService,
            IClaimService claimService)
        {
            _userService = userService;
            _claimService = claimService;
        }

        #endregion

        // GET: RoleClaims
        public async Task<IActionResult> RoleClaims(string roleName)
        {
            var claims = await _claimService.GetClaimsRoleAsync(roleName);

            var claimList = new List<ClaimViewModel>();
            if (claims.Count() > 0)
            {
                foreach (var claim in claims)
                {
                    claimList.Add(new ClaimViewModel
                    {
                        Type = claim.Type,
                        Value = claim.Value,
                        RoleName = roleName,
                    });
                }
            }
            else
            {
                claimList.Add(new ClaimViewModel
                {
                    RoleName = roleName,
                });
            }

            return View(claimList);
        }

        // GET: UserClaims
        public async Task<IActionResult> UserClaims(string userId)
        {
            var claims = await _claimService.GetClaimsUserAsync(userId);

            var claimList = new List<ClaimViewModel>();
            if (claims.Count() > 0)
            {
                foreach (var claim in claims)
                {
                    claimList.Add(new ClaimViewModel
                    {
                        Type = claim.Type,
                        Value = claim.Value,
                        UserId = userId,
                        UserName = _userService.GetUserByIdAsync(userId).Result.UserName,
                    });
                }
            }
            else
            {
                claimList.Add(new ClaimViewModel
                {
                    UserId = userId,
                    UserName = _userService.GetUserByIdAsync(userId).Result.UserName,
                });
            }

            return View(claimList);
        }

        // GET: RoleClaim/Create
        public IActionResult CreateRoleClaim(string roleName)
        {
            var model = new ClaimViewModel
            {
                RoleName = roleName,
            };

            return View(model);
        }

        // POST: RoleClaim/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRoleClaim(ClaimViewModel model)
        {
            if (ModelState.IsValid)
            {
                var claimDto = new ClaimDto
                {
                    Type = model.Type,
                    Value = model.Value,
                };

                var result = await _claimService.CreateClaimForRoleAsync(claimDto, model.RoleName!);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Role");
                }
            }

            return RedirectToAction("CreateRoleClaim", "Claim", model.RoleName);
        }

        // GET: UserClaim/Create
        public IActionResult CreateUserClaim(string userId)
        {
            var model = new ClaimViewModel
            {
                UserId = userId,
                UserName = _userService.GetUserByIdAsync(userId).Result.UserName,
            };

            return View(model);
        }

        // POST: UserClaim/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUserClaim(ClaimViewModel model)
        {
            if (ModelState.IsValid)
            {
                var claimDto = new ClaimDto
                {
                    Type = model.Type,
                    Value = model.Value,
                };

                var result = await _claimService.CreateClaimForUserAsync(claimDto, model.UserId!);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "User");
                }
            }

            return RedirectToAction("CreateUserClaim", "Claim", model.UserId);
        }

        // GET: RoleClaim/Edit/5
        public async Task<IActionResult> EditRoleClaim(string roleName, string claimType)
        {
            var claims = await _claimService.GetClaimsRoleAsync(roleName);

            var model = new ClaimViewModel();
            if (claims != null)
            {
                foreach (var claim in claims)
                {
                    if (claim.Type == claimType)
                    {
                        model.Type = claim.Type;
                        model.Value = claim.Value;
                        model.RoleName = roleName;
                        model.ClaimType = claimType;
                        break;
                    }
                }
            }

            return View(model);
        }

        // POST: RoleClaim/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRoleClaim(ClaimViewModel model)
        {
            if (ModelState.IsValid)
            {
                var claimDto = new ClaimDto
                {
                    Type = model.Type,
                    Value = model.Value,
                };

                await _claimService.UpdateClaimForRoleAsync(claimDto, model.RoleName!, model.ClaimType!);

                return RedirectToAction("RoleClaims", "Claim", new { roleName = model.RoleName });
            }

            return View(model);
        }

        // GET: RoleClaim/Edit/5
        public async Task<IActionResult> EditUserClaim(string userId, string claimType)
        {
            var claims = await _claimService.GetClaimsUserAsync(userId);

            var model = new ClaimViewModel();
            if (claims != null)
            {
                foreach (var claim in claims)
                {
                    if (claim.Type == claimType)
                    {
                        model.Type = claim.Type;
                        model.Value = claim.Value;
                        model.UserId = userId;
                        model.UserName = _userService.GetUserByIdAsync(userId).Result.UserName;
                        model.ClaimType = claimType;
                        break;
                    }
                }
            }

            return View(model);
        }

        // POST: RoleClaim/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserClaim(ClaimViewModel model)
        {
            if (ModelState.IsValid)
            {
                var claimDto = new ClaimDto
                {
                    Type = model.Type,
                    Value = model.Value,
                };

                await _claimService.UpdateClaimForUserAsync(claimDto, model.UserId!, model.ClaimType!);

                return RedirectToAction("UserClaims", "Claim", new { userId = model.UserId });
            }

            return View(model);
        }

        // GET: RoleClaim/Delete/5
        public async Task<IActionResult> DeleteRoleClaim(string roleName, string claimType)
        {
            var claims = await _claimService.GetClaimsRoleAsync(roleName);

            var model = new ClaimViewModel();
            if (claims != null)
            {
                foreach (var claim in claims)
                {
                    if (claim.Type == claimType)
                    {
                        model.Type = claim.Type;
                        model.Value = claim.Value;
                        model.RoleName = roleName;
                        model.ClaimType = claimType;
                        break;
                    }
                }
            }

            return View(model);
        }

        // POST: RoleClaim/Delete/5
        [HttpPost, ActionName("DeleteRoleClaim")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string roleName, string claimType, string claimValue)
        {
            if (roleName != null)
            {
                var claims = await _claimService.GetClaimsRoleAsync(roleName);
                if (claims != null)
                {
                    var claimDto = new ClaimDto
                    {
                        Type = claimType,
                        Value = claimValue,
                    };

                    await _claimService.DeleteClaimForRoleAsync(claimDto, roleName);
                }
            }

            return RedirectToAction("RoleClaims", "Claim", new { roleName = roleName! });
        }

        // GET: UserClaim/Delete/5
        public async Task<IActionResult> DeleteUserClaim(string userId, string claimType)
        {
            var claims = await _claimService.GetClaimsUserAsync(userId);

            var model = new ClaimViewModel();
            if (claims != null)
            {
                foreach (var claim in claims)
                {
                    if (claim.Type == claimType)
                    {
                        model.Type = claim.Type;
                        model.Value = claim.Value;
                        model.UserId = userId;
                        model.UserName = _userService.GetUserByIdAsync(userId).Result.UserName;
                        model.ClaimType = claimType;
                        break;
                    }
                }
            }

            return View(model);
        }

        // POST: UserClaim/Delete/5
        [HttpPost, ActionName("DeleteUserClaim")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(string userId, string claimType, string claimValue)
        {
            if (userId != null)
            {
                var claims = await _claimService.GetClaimsUserAsync(userId);
                if (claims != null)
                {
                    var claimDto = new ClaimDto
                    {
                        Type = claimType,
                        Value = claimValue,
                    };

                    await _claimService.DeleteClaimForUserAsync(claimDto, userId);
                }
            }

            return RedirectToAction("UserClaims", "Claim", new { userId = userId! });
        }
    }
}
