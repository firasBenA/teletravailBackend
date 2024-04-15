using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;
using TestApi.Models;
using TestApi.Repositories;

namespace TestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BoatController : ControllerBase
    {
        private readonly IBoatRepository _boatRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public BoatController(IBoatRepository boatRepository, IWebHostEnvironment webHostEnvironment)
        {
            _boatRepository = boatRepository;
            _webHostEnvironment = webHostEnvironment;

        }





        [HttpGet]
        public async Task<ActionResult<List<Boat>>> GetAllBoats()
        {
            var boats = await _boatRepository.GetAllBoats();
            if (boats == null)
            {
                return NotFound(); // Returns 404 if no boats are found
            }
            return boats;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Boat>> GetBoatById(int id)
        {
            var boat = await _boatRepository.GetByIdBoat(id);
            if (boat == null)
            {
                return NotFound(); // Returns 404 if boat with the given id is not found
            }
            return boat;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("File is null or empty.");

                var uploadsFolder = _webHostEnvironment.WebRootPath ?? Path.Combine(AppContext.BaseDirectory, "wwwroot", "images");
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                var imageUrl = "/images/" + fileName;

                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("image/{imageName}")] // Update the route template to include "image"
        public IActionResult GetImage(string imageName)
        {
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", imageName);
            if (!System.IO.File.Exists(imagePath))
            {
                return NotFound(); // Image not found
            }

            var imageData = System.IO.File.ReadAllBytes(imagePath);
            return File(imageData, "image/jpeg"); // Adjust content type as needed
        }

        [HttpPost]
        public async Task<ActionResult<Boat>> AddBoat(Boat boat)
        {
            var result = await _boatRepository.AddBoat(boat);
            if (!result)
            {
                return BadRequest(); // Returns 400 if boat addition fails
            }
            return CreatedAtAction(nameof(GetBoatById), new { id = boat.Id }, boat);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBoat(int id, Boat boat)
        {
            if (id != boat.Id)
            {
                return BadRequest(); // Returns 400 if boat ID in the request body does not match the ID in the URL
            }

            var result = await _boatRepository.UpdateBoat(boat);
            if (!result)
            {
                return NotFound(); // Returns 404 if boat with the given id is not found
            }
            return NoContent(); // Returns 204 if boat update is successful
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBoat(int id)
        {
            var result = await _boatRepository.DeleteBoat(id);
            if (!result)
            {
                return NotFound(); // Returns 404 if boat with the given id is not found
            }
            return NoContent(); // Returns 204 if boat deletion is successful
        }
    }
}
