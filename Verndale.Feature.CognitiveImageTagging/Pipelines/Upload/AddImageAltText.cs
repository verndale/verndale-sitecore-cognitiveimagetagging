using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.Upload;
using Sitecore.SecurityModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Verndale.CognitiveImageTagging;
using Verndale.CognitiveImageTagging.Services;
using Verndale.Feature.CognitiveImageTagging.Extensions;

namespace Verndale.Feature.CognitiveImageTagging.Pipelines.Upload
{
    public class AddImageAltText
    {
        /// <summary>
        /// Uses Verndale.CognitiveImageTagging services to analyze the image and provide possible captions.
        /// Assigns the first caption to the Alt field in the Image section upon execution of the command.
        /// Reference should be placed above "Done" and below "Save" in uiUpload Processor
        /// </summary>
        public void Process(UploadArgs args)
        {
            Assert.ArgumentNotNull((object)args, nameof(args));

            var list = args.UploadedItems;

            var service = ServiceManager.GetAnalysisService();

            Log.Info($"Verndale.CognitiveImageTagging: Beginning AddImageAltText in uiUpload processor", this);

            foreach (var item in list)
            {
                if (item == null || !item.IsImage()) continue;

                var mediaItem = (MediaItem)item;

                var imageStream = mediaItem.GetMediaStream();

                using (new SecurityDisabler())
                {
                    mediaItem.Alt = GetAltText(imageStream, service).Result;

                    if (string.IsNullOrEmpty(mediaItem.Alt))
                    {
                        Log.Warn($"Verndale.CognitiveImageTagging: Unable to provide alt text for media item, {mediaItem.ID}", this);
                    }
                    else
                    {
                        Log.Debug($"Verndale.CognitiveImageTagging: Added alt text of '{mediaItem.Alt}' to media item, {mediaItem.ID}", this);
                    }
                }
            }

            Log.Info($"Verndale.CognitiveImageTagging: Ending AddImageAltText in uiUpload processor", this);
        }

        private async Task<string> GetAltText(Stream imageStream, IAnalysisService service)
        {
            ImageResult result = await service.GetImageDescription(imageStream, "en", true);

            var captionsList = result?.Captions.ToList();

			return (captionsList == null || !captionsList.Any()) 
                ? string.Empty 
                : captionsList.First();
        }
    }
}
