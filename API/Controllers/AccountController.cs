using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;
using API.RequestReponse.RequestViewModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _dbContext;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dbContext = dbContext;
        }

        [HttpPost("registration")]
        public async  Task<ActionResult> Regifffstration([FromForm]RegistrationRequestViewModel viewModel)
        {
            var user = await _userManager.FindByNameAsync(viewModel.Email);
            if (user != null)
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }

            user = new ApplicationUser { UserName = viewModel.Email, Email = viewModel.Email };
            var result = await _userManager.CreateAsync(user, viewModel.Password);
            if (result.Succeeded)
            {
                var nowInsertedUser = await _userManager.FindByEmailAsync(viewModel.Email);
                if (!await _roleManager.RoleExistsAsync("customer"))
                {
                   await _roleManager.CreateAsync(new IdentityRole()
                    {
                        Name = "customer"
                    });
                }
                var result1 = await _userManager.AddToRoleAsync(nowInsertedUser, "customer");
                return Ok();
            }
            return StatusCode(StatusCodes.Status409Conflict);
        }
    }
}