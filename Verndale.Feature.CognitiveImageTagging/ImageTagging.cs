using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using System.Linq;
using Verndale.CognitiveImageTagging;

namespace Verndale.Feature.CognitiveImageTagging
{
    public static class ImageTagging
    {
        /// <summary>
        /// Call CognitiveImageTagging Service Manager / Analysis Service;
        /// Call GetImageDescription to call the analysis service to retrieve the possible captions for the given media item.
        /// </summary>
        /// <param name="mediaItem">MediaItem to be analyzed</param>
        /// <param name="retainExistingText">If Alt text is populated, method won't overwrite alt text if true.  Default: false</param>
        public static void AddAltText(MediaItem mediaItem, bool retainExistingText = false)
        {
            if (mediaItem == null)
            {
                Log.Error("Verndale.CognitiveImageTagging: Unable to tag a media item that is null.", typeof(ServiceManager));
                return;
            }

            if (!string.IsNullOrWhiteSpace(mediaItem.Alt) && retainExistingText) return;

            var imageStream = mediaItem.GetMediaStream();

            var service = ServiceManager.GetAnalysisService();

            ImageResult result = service.GetImageDescription(imageStream, "en", true).Result;

            if (result?.Exception != null && !string.IsNullOrWhiteSpace(result.Exception.Message))
            {
                Log.Error($"Verndale.CognitiveImageTagging: Unable to tag media item, {mediaItem.ID}; caption is null.", typeof(ServiceManager));
                return;
            }

            var captionsList = result?.Captions.ToList();

            if (captionsList == null || !captionsList.Any())
            {
                Log.Warn($"Verndale.CognitiveImageTagging: Unable to provide alt text for media item, {mediaItem.ID}", typeof(ServiceManager));
                return;
            }

            using (new EditContext(mediaItem))
            {
                mediaItem.Alt = captionsList.First();
            }

            Log.Debug($"Verndale.CognitiveImageTagging: Added alt text of '{captionsList.First()}' to media item, {mediaItem.ID}", typeof(ServiceManager));
        }
    }
}
