using BeeStore_Repository.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BeeStore_Api.Controllers
{
    public class PictureController : BaseController
    {
        private readonly IPictureService _pictureService;

        public PictureController(IPictureService pictureService)
        {
            _pictureService = pictureService;
        }

        [Route("upload-image")]
        [HttpPost]
        public async Task<IActionResult> Upload([FromForm(Name = "image")] IFormFile file)
        {
            return Ok(await _pictureService.UploadImage(file));
        }

        [Route("upload-profile-image/{id}")]
        [HttpPost]
        public async Task<IActionResult> user(int id, [FromForm(Name = "image")] IFormFile file)
        {
            return Ok(await _pictureService.uploadImageForUser(id, file));
        }

        [Route("upload-order-image/{id}")]
        [HttpPost]
        public async Task<IActionResult> order(int id, [FromForm(Name = "image")] IFormFile file)
        {
            return Ok(await _pictureService.uploadImageForOrder(id, file));
        }

        [Route("upload-warehouse-image/{id}")]
        [HttpPost]
        public async Task<IActionResult> product(int id, [FromForm(Name = "image")] IFormFile file)
        {
            return Ok(await _pictureService.uploadImageForProduct(id, file));
        }
    }
}
