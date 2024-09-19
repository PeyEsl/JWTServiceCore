using Microsoft.EntityFrameworkCore;
using JWTServiceCore.Models.DTOs;
using JWTServiceCore.Models.ViewModels;
using JWTServiceCore.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JWTServiceCore.Controllers
{
    [Authorize]
    public class RoleController : Controller
    {
        #region Ctor

        private readonly IAuthService _authenticationService;
        private readonly IRoleService _roleService;
        private readonly IClaimService _claimService;

        public RoleController(
            IAuthService authenticationService,
            IRoleService roleService,
            IClaimService claimService)
        {
            _authenticationService = authenticationService;
            _roleService = roleService;
            _claimService = claimService;
        }

        #endregion

        // GET: Role
        public async Task<IActionResult> Index()
        {
            var roles = await _roleService.GetAllRolesAsync();

            var roleList = new List<RoleViewModel>();
            if (roles != null)
            {
                foreach (var role in roles)
                {
                    var claims = await _claimService.GetClaimsRoleAsync(role.Name!);

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
                        Id = role.Id,
                        Name = role.Name,
                        Claims = claimList,
                    });
                }
            }

            return View(roleList);
        }

        // GET: Role/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleDto = await _roleService.GetRoleByIdAsync(id);
            if (roleDto == null)
            {
                return NotFound();
            }

            var claims = await _claimService.GetClaimsRoleAsync(roleDto.Name!);

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

            var model = new RoleViewModel
            {
                Id = roleDto.Id,
                Name = roleDto.Name,
                Claims = claimList,
            };

            return View(model);
        }

        // GET: Role/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Role/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                var roleDto = new RoleDto
                {
                    Name = model.Name,
                };

                var result = await _roleService.CreateRoleAsync(roleDto.Name!);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(model);
        }

        // GET: Role/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleDto = await _roleService.GetRoleByIdAsync(id);
            if (roleDto == null)
            {
                return NotFound();
            }

            var claims = await _claimService.GetClaimsRoleAsync(roleDto.Name!);

            var claimList = new List<ClaimViewModel>();
            if (claims != null)
            {
                foreach (var claim in claims)
                {
                    claimList.Add(new ClaimViewModel
                    {
                        Type = claim.Type,
                        Value = claim.Value,
                        RoleName = roleDto.Name,
                    });
                }
            }

            var model = new RoleViewModel
            {
                Id = roleDto.Id,
                Name = roleDto.Name,
                Claims = claimList,
            };

            return View(model);
        }

        // POST: Role/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, RoleViewModel model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var roleDto = new RoleDto
                    {
                        Id = model.Id,
                        Name = model.Name,
                    };

                    var claimsList = new List<ClaimDto>();
                    foreach (var claim in model.Claims!)
                    {
                        claimsList.Add(new ClaimDto
                        {
                            Type = claim.Type,
                            Value = claim.Value,
                        });
                    }

                    var updateResult = await _roleService.UpdateRoleAsync(roleDto);
                    if (updateResult.Succeeded)
                    {
                        await _claimService.UpdateClaimsForRoleAsync(claimsList, model.Name!);
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    var exist = await RoleExists(model.Id);
                    if (!exist)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        // GET: Role/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var roleDto = await _roleService.GetRoleByIdAsync(id);
            if (roleDto == null)
            {
                return NotFound();
            }

            var claims = await _claimService.GetClaimsRoleAsync(roleDto.Name!);

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

            var model = new RoleViewModel
            {
                Id = roleDto.Id,
                Name = roleDto.Name,
                Claims = claimList,
            };

            return View(model);
        }

        // POST: Role/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            if (id != null)
            {
                await _roleService.DeleteRoleAsync(id);
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> RoleExists(string id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return false;
            }

            return true;
        }
    }
}
