using FluentValidation;
using ImageAPI.Contracts;
using ImageAPI.DAL;
using Microsoft.AspNetCore.Mvc;

namespace ImageAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImagesController(IImagesRepository _db, IValidator<ImageUploadRequest> validator) : ControllerBase
    {
        [HttpPost]
        public async Task<IResult> UploadImage([FromForm] ImageUploadRequest imageRequest)
        {
            var validateResult = validator.Validate(imageRequest);

            if (validateResult.IsValid)
            {
                var result = await _db.UploadImage(imageRequest);
                return MapToHttpResponse(result);
            }
            else
            {
                return Results.BadRequest(validateResult.ToDictionary());
            }
        }

        private static IResult MapToHttpResponse(UploadStatus resultStatus)
        {
            if (resultStatus == UploadStatus.Ok)
                return Results.Ok();

            if (resultStatus == UploadStatus.ContainerNotFound)
                return Results.NotFound("Container not found. Please check settings.");

            if (resultStatus == UploadStatus.BlobAlreadyExists)
                return Results.Conflict("Blob already exists.");

            return Results.StatusCode(StatusCodes.Status500InternalServerError);
        }
    }    
}
