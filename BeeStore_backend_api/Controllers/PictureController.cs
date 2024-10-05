using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace BeeStore_Api.Controllers
{
    [ApiController]
    public class PictureController : BaseController
    {
        private readonly IPictureService _pictureService;

        public PictureController(IPictureService pictureService)
        {
            _pictureService = pictureService;
        }

        [HttpPost()]
        public async Task<IActionResult> Upload([FromForm(Name = "image")] IFormFile file)
        {
            return Ok(await _pictureService.UploadImage(file));
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> user(int id, [FromForm(Name = "image")] IFormFile file)
        {
            return Ok(await _pictureService.uploadImageForUser(id, file));
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> order(int id, [FromForm(Name = "image")] IFormFile file)
        {
            return Ok(await _pictureService.uploadImageForOrder(id, file));
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> product(int id, [FromForm(Name = "image")] IFormFile file)
        {
            return Ok(await _pictureService.uploadImageForProduct(id, file));
        }
    }
}
