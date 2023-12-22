using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using AppServer.Api.Endpoint.Controllers;
using Chamberlain.AppServer.Api.Contracts.Commands.Profiles;
using Chamberlain.AppServer.Api.Contracts.Models.RequestModels.Profile;
using Chamberlain.AppServer.Api.Contracts.Models.ResponseModels.Profile;
using Chamberlain.AppServer.Api.Endpoint.Helpers;
using Chamberlain.Common.Akka;
using Chamberlain.Common.Settings;
using Common.StaticMethods.StaticMethods;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Chamberlain.AppServer.Api.Endpoint.Controllers
{
    /// <inheritdoc />
    /// <summary>
    /// Profile controller class.
    /// </summary>
    [Route("profile")]
    public class ProfileController : ChamberlainBaseController
    {
        private const int MaxImageSize = 20971520; // 20 Megabytes
        private const int MaxSamplesSize = 20971520; // 20 Megabytes

        /// <summary>
        /// Gets all profiles from current user.
        /// </summary>
        /// <returns>
        /// List of profiles for current user.
        /// </returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<ProfileModel>), 200)]
        public async Task<IActionResult> GetProfiles()
        {
            var result = await SystemActors.ProfileActor.Execute<GetProfiles, List<ProfileModel>>(new GetProfiles(User.Identity.Name));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Gets profile from current user of selected id.
        /// </summary>
        /// /// <param name="id">
        /// ProfileId of profile to get.
        /// </param>
        /// <returns>
        /// Profile for current user of selected id.
        /// </returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProfileModel), 200)]
        public async Task<IActionResult> GetProfile(long id)
        {
            var result = await SystemActors.ProfileActor.Execute<GetProfile, ProfileModel>(new GetProfile(User.Identity.Name, id));
            return new ObjectResult(result);
        }

        /// <summary>
        /// Adding profile.
        /// </summary>
        /// <param name="profileModel">
        /// Profile model.
        /// </param>
        /// <returns>
        /// Returns 200 if success.
        /// </returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProfileModel), 200)]
        public async Task<IActionResult> AddProfile([FromBody] ProfileModel profileModel)
        {
            var result = await SystemActors.ProfileActor.Execute<AddProfile, ProfileModel>(new AddProfile(User.Identity.Name, profileModel));
            return Ok(result);
        }

        /// <summary>
        /// Updating profile.
        /// </summary>
        /// <param name="profileModel">
        /// Profile model.
        /// </param>
        /// <returns>
        /// Returns 200 if success.
        /// </returns>
        [HttpPut]
        [ProducesResponseType(typeof(ProfileModel), 200)]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileModel profileModel)
        {
            var result = await SystemActors.ProfileActor.Execute<UpdateProfile, ProfileModel>(new UpdateProfile(User.Identity.Name, profileModel));
            return Ok(result);
        }

        /// <summary>
        /// Deletes profile of id for current user.
        /// </summary>
        /// <param name="profileId">
        /// ProfileId of profile to delete.
        /// </param>
        /// <returns>
        /// Returns 204 if succeeded.
        /// </returns>
        [HttpDelete("{profileId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteProfile(long profileId)
        {
            await SystemActors.ProfileActor.Execute(new DeleteProfile(User.Identity.Name, profileId));
            return NoContent();
        }

        /// <summary>
        /// Uploading front face image to cloud.
        /// </summary>
        /// <param name="image">
        /// Image to upload by form-data.
        /// </param>
        /// <param name="profileId">
        /// Id of profile.
        /// </param>
        /// <returns>
        /// Returns 204 if success.
        /// </returns>
        [HttpPost("{profileId:long}/frontal")]
        [ProducesResponseType(typeof(FaceViewReadyModel), 204)]
        public async Task<IActionResult> UploadPhotoFrontal(IFormFile image, long profileId)
        {
            return await UploadImage(image, profileId, FaceViewType.Frontal);
        }

        /// <summary>
        /// Uploading front face from above image to cloud.
        /// </summary>
        /// <param name="image">
        /// Image to upload by form-data.
        /// </param>
        /// <param name="profileId">
        /// Id of profile.
        /// </param>
        /// <returns>
        /// Returns 204 if success.
        /// </returns>
        [HttpPost("{profileId:long}/frontalFromAbove")]
        [ProducesResponseType(typeof(FaceViewReadyModel), 204)]
        public async Task<IActionResult> UploadPhotoFrontalFromAbove(IFormFile image, long profileId)
        {
            return await UploadImage(image, profileId, FaceViewType.FrontalFromAbove);
        }

        /// <summary>
        /// Uploading left side face image to cloud.
        /// </summary>
        /// <param name="image">
        /// Image to upload by form-data.
        /// </param>
        /// <param name="profileId">
        /// Id of profile.
        /// </param>
        /// <returns>
        /// Returns 204 if success.
        /// </returns>
        [HttpPost("{profileId:long}/leftProfile")]
        [ProducesResponseType(typeof(FaceViewReadyModel), 204)]
        public async Task<IActionResult> UploadPhotoLeftProfile(IFormFile image, long profileId)
        {
            return await UploadImage(image, profileId, FaceViewType.LeftProfile);
        }

        /// <summary>
        /// Uploading right side face image to cloud.
        /// </summary>
        /// <param name="image">
        /// Image to upload by form-data.
        /// </param>
        /// <param name="profileId">
        /// Id of profile.
        /// </param>
        /// <returns>
        /// Returns 204 if success.
        /// </returns>
        [HttpPost("{profileId:long}/rightProfile")]
        [ProducesResponseType(typeof(FaceViewReadyModel), 204)]
        public async Task<IActionResult> UploadPhotoRightProfile(IFormFile image, long profileId)
        {
            return await UploadImage(image, profileId, FaceViewType.RightProfile);
        }

        /// <summary>
        /// This method will start learning model of provided face views.
        /// </summary>
        /// <param name="profileId">
        /// ProfileID of profile that should be learnt
        /// </param>
        /// <returns> 
        /// 204 if success.
        /// </returns>
        [HttpPut("{profileId:long}/startTraining")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> StartModelTraining(long profileId)
        {
            await SystemActors.ProfileActor.Execute(new StartModelTraining(User.Identity.Name, profileId));
            return NoContent();
        }

        /// <summary>
        /// Uploading front face image to cloud.
        /// </summary>
        /// <param name="faceRecognitionSamples"></param>
        ///     Id of profile.
        /// <returns>
        /// Returns 204 if success.
        /// </returns>
        [HttpPost("uploadFaceRecognitionSamples")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UploadSamples(IFormFile faceRecognitionSamples)
        {
            return await HandleSamples(faceRecognitionSamples);
        }

        private static bool SamplesValidation(IFormFile faceRecognitionSamples)
        {
            if (true) //TODO
            return faceRecognitionSamples != null &&
                   faceRecognitionSamples.Length > 0 &&
                   faceRecognitionSamples.Length < MaxSamplesSize &&
                   faceRecognitionSamples.ContentType.Equals("application/zip");
        }

        private async Task<IActionResult> HandleSamples(IFormFile faceRecognitionSamples)
        {
            if (!SamplesValidation(faceRecognitionSamples))
                return BadRequest("Wrong recognition samples file size or format");

            var result = await SystemActors.ProfileActor.Execute<UploadSamples, SamplesModel>(new UploadSamples(User.Identity.Name));
            var fileName = FaceRecognitionOverseerDbSettings.DownloadedSampleZipCloudPath(result.CustomerId);

            if (!Directory.Exists(FaceRecognitionOverseerDbSettings.DownloadedSamplesCloudPath(result.CustomerId)))
                Directory.CreateDirectory(FaceRecognitionOverseerDbSettings.DownloadedSamplesCloudPath(result.CustomerId));

            await SaveFile(faceRecognitionSamples, fileName);

            return NoContent();
        }

        private static bool ImageValidation(IFormFile image)
        {
            return image != null && image.Length > 0 && image.Length < MaxImageSize && image.ContentType.Equals("image/jpeg");
        }

        private static async Task SaveFile(IFormFile file, string path)
        {
            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }

        private async Task<IActionResult> UploadImage(IFormFile image, long profileId, FaceViewType modelType)
        {
            if (!ImageValidation(image))
                return BadRequest("Wrong image size or format");

            var profileActor = SystemActors.ProfileActor;
            var userName = User.Identity.Name;
            var photoModel = await profileActor.Execute<AddPhoto, PhotoModel>(new AddPhoto(userName, profileId, modelType));

            var fileName = ModelTrainingPhotosPaths.PhotoFilePath(photoModel.CustomerId, photoModel.FileName);
            await SaveFile(image, fileName);
            
            var faceViewReady = await profileActor.Execute<CheckFaceViewReadiness, FaceViewReadyModel>(new CheckFaceViewReadiness(userName, profileId, modelType));
            return new ObjectResult(faceViewReady);
        }
    }
}
