using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using DatingApp.API.Data;
using DatingApp.API.DTOS;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/users/{userid}/photos")]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper,
                IOptions<CloudinarySettings> cloudinaryConfig)
        {
            this._cloudinaryConfig = cloudinaryConfig;
            this._mapper = mapper;
            this._repo = repo;

            Account acc = new Account(
                _cloudinaryConfig.Value.CloudName,
                _cloudinaryConfig.Value.ApiKey,
                _cloudinaryConfig.Value.ApiSecret
            );

            _cloudinary = new Cloudinary(acc);

        }
        
        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            var photoFromRepo =await _repo.GetPhoto(id);

            var photo = _mapper.Map<PhotoforReturnDTO>(photoFromRepo);

            return Ok(photo);

        }


        [HttpPost]
        public async Task<IActionResult> AddPhotosForUser(int userId,[FromForm]PhotoforCreationDTO photoforcreationDto)
        {
             if (userId !=  int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
                return Unauthorized();

            var userFromRepo = await _repo.GetUser(userId);

            var file = photoforcreationDto.File;

            var uploadResult = new ImageUploadResult();

            if(file.Length > 0)
            {
                using(var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("Fill").Gravity("Face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            photoforcreationDto.Url = uploadResult.Uri.ToString();
            photoforcreationDto.PublieId = uploadResult.PublicId.ToString();
            var photo = _mapper.Map<Photo>(photoforcreationDto);
            if (! userFromRepo.Photos.Any(u => u.isMainp))
            {
                photo.isMainp = true;
            }
            userFromRepo.Photos.Add(photo); 

            if(await _repo.SaveAll())
            {
                var photoToReturn = _mapper.Map<PhotoforReturnDTO>(photo);
                return CreatedAtRoute("GetPhoto",new {userId = userId, id = photo.Id}, photoToReturn);
            }

            return BadRequest("Could not save the photo");
        }
}
}