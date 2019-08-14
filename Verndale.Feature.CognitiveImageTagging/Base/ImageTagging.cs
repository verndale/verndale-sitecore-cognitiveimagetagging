using Sitecore.Data.Items;
using System.Linq;
using Sitecore.Diagnostics;
using Verndale.CognitiveImageTagging;

namespace Verndale.Feature.CognitiveImageTagging.Base
{
    public static class ImageTagging
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mediaItem"></param>
        /// <param name="retainExistingText"></param>
        public static async void AddAltText(MediaItem mediaItem, bool retainExistingText = false)
        {
            if (mediaItem == null)
            {
                Log.Error("Verndale.CognitiveImageTagging: Unable to tag a media item that is null.", typeof(ServiceManager));
                return;
            }

            if (!string.IsNullOrWhiteSpace(mediaItem.Alt) && retainExistingText) return;

            var imageStream = mediaItem.GetMediaStream();

            var service = ServiceManager.GetAnalysisService();

            ImageResult result = await service.GetImageDescription(imageStream, "en", true);

            var captionsList = result?.Captions.ToList();

            if (captionsList == null || !captionsList.Any())
            {
                Log.Warn($"Verndale.CognitiveImageTagging: Unable to provide alt text for media item, {mediaItem.ID}", typeof(ServiceManager));
                return;
            }

            mediaItem.Alt = captionsList.First();

            Log.Debug($"Verndale.CognitiveImageTagging: Added alt text of '{captionsList.First()}' to media item, {mediaItem.ID}", typeof(ServiceManager));
        }
    }
}
