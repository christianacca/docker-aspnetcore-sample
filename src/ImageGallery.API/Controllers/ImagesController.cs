using AutoMapper;
using ImageGallery.API.Services;
using ImageGallery.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ImageGallery.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly IGalleryRepository _galleryRepository;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ImagesController(IGalleryRepository galleryRepository,
            IHostingEnvironment hostingEnvironment)
        {
            _galleryRepository = galleryRepository;
            _hostingEnvironment = hostingEnvironment;
        }

        [HttpGet]
        public async Task<ActionResult<ICollection<Image>>> GetImages()
        {
            var ownerId = User.Claims.First(c => c.Type == "sub").Value;

            // get from repo
            var imagesFromRepo = await _galleryRepository.GetImagesAsync(ownerId);

            // map to model
            return Mapper.Map<IEnumerable<Image>>(imagesFromRepo).ToList();
        }

        [HttpGet("{id}", Name = "GetImage")]
        [Authorize("MustOwnImage")]
        public async Task<ActionResult<Image>> GetImage(Guid id)
        {          
            var imageFromRepo = await _galleryRepository.GetImageAsync(id);

            if (imageFromRepo == null)
            {
                return NotFound();
            }

            return Mapper.Map<Image>(imageFromRepo);
        }

        [HttpPost]
        [Authorize(Roles = "PayingUser")]
        public async Task<ActionResult<Image>> CreateImage([FromBody] ImageForCreation imageForCreation)
        {
            // Automapper maps only the Title in our configuration
            var imageEntity = Mapper.Map<Entities.Image>(imageForCreation);

            // Create an image from the passed-in bytes (Base64), and 
            // set the filename on the image

            // get this environment's web root path (the path
            // from which static content, like an image, is served)
            var webRootPath = _hostingEnvironment.WebRootPath;

            // create the filename
            string fileName = $"{Guid.NewGuid()}.jpg";
            
            // the full file path
            var filePath = Path.Combine($"{webRootPath}/imgs/{fileName}");

            // write bytes and auto-close stream
            System.IO.File.WriteAllBytes(filePath, imageForCreation.Bytes);

            // fill out the filename
            imageEntity.FileName = fileName;

            // ownerId should be set - can't save image in starter solution, will
            // be fixed during the course
            //imageEntity.OwnerId = ...;

            // set the ownerId on the imageEntity
            var ownerId = User.Claims.First(c => c.Type == "sub").Value;
            imageEntity.OwnerId = ownerId;

            // add and save.  
            _galleryRepository.AddImage(imageEntity);

            if (!await _galleryRepository.SaveAsync())
            {
                throw new Exception("Adding an image failed on save.");
            }

            var imageToReturn = Mapper.Map<Image>(imageEntity);

            return CreatedAtRoute("GetImage",
                new { id = imageToReturn.Id },
                imageToReturn);
        }

        [HttpDelete("{id}")]
        [Authorize("MustOwnImage")]
        public async Task<ActionResult> DeleteImage(Guid id)
        {
            var imageFromRepo = await _galleryRepository.GetImageAsync(id);

            if (imageFromRepo == null)
            {
                return NotFound();
            }

            _galleryRepository.DeleteImage(imageFromRepo);

            if (!await _galleryRepository.SaveAsync())
            {
                throw new Exception($"Deleting image with {id} failed on save.");
            }

            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize("MustOwnImage")]
        public async Task<ActionResult> UpdateImage(Guid id, 
            [FromBody] ImageForUpdate imageForUpdate)
        {
            var imageFromRepo = await _galleryRepository.GetImageAsync(id);
            if (imageFromRepo == null)
            {
                return NotFound();
            }

            Mapper.Map(imageForUpdate, imageFromRepo);

            _galleryRepository.UpdateImage(imageFromRepo);

            if (!await _galleryRepository.SaveAsync())
            {
                throw new Exception($"Updating image with {id} failed on save.");
            }

            return NoContent();
        }
    }
}