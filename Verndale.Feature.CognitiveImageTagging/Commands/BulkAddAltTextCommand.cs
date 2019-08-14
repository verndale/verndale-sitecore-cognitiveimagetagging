using System.Linq;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Verndale.Feature.CognitiveImageTagging.Base;
using Verndale.Feature.CognitiveImageTagging.Extensions;

namespace Verndale.Feature.CognitiveImageTagging.Commands
{
    /// <summary>
    /// Uses Verndale.CognitiveImageTagging services to analyze the image and provide alternate text for the image.
    /// Service is run for the current item and its descendants via recursion.
    /// </summary>
    public class BulkAddAltTextCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.IsNotNull(context, "context");

            var contextItem = context.Items?.FirstOrDefault();

            Log.Info($"Verndale.CognitiveImageTagging: Beginning BulkAddAltTextCommand beginning at item {contextItem?.ID}", this);

            AddAltTextToItem(contextItem);

            Log.Info($"Verndale.CognitiveImageTagging: Ending BulkAddAltTextCommand for self and children of {contextItem?.ID}", this);
        }

        private void AddAltTextToItem(Item item)
        {
            if (item == null) return;

            if (item.IsImage())
            {
                MediaItem mediaItem = item;

                ImageTagging.AddAltText(mediaItem, true);
            }

            if (!item.HasChildren) return;

            foreach (Item child in item.Children)
            {
                AddAltTextToItem(child);
            }
        }
    }
}
