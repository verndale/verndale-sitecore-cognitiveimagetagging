using System;
using Sitecore.Data.Items;
using Sitecore.Events;
using Verndale.Feature.CognitiveImageTagging.Extensions;

namespace Verndale.Feature.CognitiveImageTagging.Events
{
    /// <summary>
    /// Uses Verndale.CognitiveImageTagging services to analyze the image and provide possible captions.
    /// Assigns the first caption to the Alt field in the Image section upon Save of an image media item
    /// </summary>
    public class AddAltTextSaveEvent
    {
        public void OnItemSaved(object sender, EventArgs args)
        {
            var item = Event.ExtractParameter(args, 0) as Item;

            if (item == null || !item.IsImage())
                return;

            var mediaItem = (MediaItem) item;

            if (string.IsNullOrWhiteSpace(mediaItem?.Alt))
            {
                ImageTagging.AddAltText(mediaItem, true);
            }
        }
    }
}
