using System.Linq;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using Verndale.Feature.CognitiveImageTagging.Base;
using Verndale.Feature.CognitiveImageTagging.Extensions;

namespace Verndale.Feature.CognitiveImageTagging.Commands
{
    /// <summary>
    /// Uses Verndale.CognitiveImageTagging services to analyze the image and provide possible captions.
    /// Assigns the first caption to the Alt field in the Image section upon execution of the command.
    /// NOTE: This command replaces the existing text with the new value from Verndale CognitiveImageTagging.
    /// </summary>
    public class ForceAddAltTextCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.IsNotNull(context,"context");

            var contextItem = context.Items.FirstOrDefault();

            if (contextItem == null || !contextItem.IsImage())
                return;

            MediaItem mediaItem = contextItem;

            ImageTagging.AddAltText(mediaItem, false);
        }
    }
}
