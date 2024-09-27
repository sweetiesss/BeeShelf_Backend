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

        [HttpPost("picture/upload")]
        public async Task<IActionResult> Upload([FromForm(Name = "image")] IFormFile file)
        {

            return Ok(await _pictureService.UploadImage(file));
        }
    }
}
