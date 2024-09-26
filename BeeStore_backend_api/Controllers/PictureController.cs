using BeeStore_Repository.Services;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace BeeStore_Api.Controllers
{
    [ApiController]
    public class PictureController : ControllerBase
    {
        IPictureService _pictureService;

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
