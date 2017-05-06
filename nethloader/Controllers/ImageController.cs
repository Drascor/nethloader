using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using nethloader.Models;
using nethloader.Services.Managers;
using Newtonsoft.Json;

namespace nethloader.Controllers
{
    [Authorize]
    public class ImageController : Controller
    {
        private readonly UserManager<User> _userManager;
        private IImageManager _imageManager;

        public ImageController(UserManager<User> userManager, IImageManager imageManager, IHostingEnvironment env)
        {
            _userManager = userManager;
            _imageManager = imageManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        [ActionName("View")]
        public async Task<IActionResult> ViewImg(string id)
        {
            var img = await _imageManager.GetImageWithOwnerAsync(id);
            img.Url = ImageManager.GetImagePath(img);
            return View(img);
        }
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upload(IFormFile file)
        {

            var currentUser = await _userManager.FindByIdAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var img = await _imageManager.SaveImageAsync(currentUser, file);
            if(img == null)
            {
                return BadRequest();
            }
            return RedirectToAction("View", new { id = img.Id });
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ApiUpload(IFormFile file)
        {
            var user = await _userManager.FindByEmailAsync(HttpContext.Request.Headers["x-auth-email"]);
            if (user == null)
                return StatusCode(403);
            if (HttpContext.Request.Headers["x-auth-key"] != user.ApiKey)
                return StatusCode(403);
            var img = await _imageManager.SaveImageAsync(user, file);
            if (img == null)
            {
                return BadRequest();
            }
            return Ok(ImageManager.GetImagePath(img));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var success = await _imageManager.RemoveImageWithOwnerCheckAsync(User.FindFirst(ClaimTypes.NameIdentifier)?.Value, id);
                if (success)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            } catch(UnauthorizedAccessException) {
                return Unauthorized();
            }
        }
        public IActionResult Filter()
        {
            return View(JsonConvert.SerializeObject(_imageManager.GetAllUserImages(User.FindFirst(ClaimTypes.NameIdentifier).Value).OrderByDescending(x => x.Id)));
        }
    }
}