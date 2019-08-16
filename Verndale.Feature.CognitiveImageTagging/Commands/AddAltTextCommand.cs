using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Commands;
using System.Linq;
using Verndale.Feature.CognitiveImageTagging.Extensions;

namespace Verndale.Feature.CognitiveImageTagging.Commands
{
    /// <summary>
    /// Uses Verndale.CognitiveImageTagging services to analyze the image and provide possible captions.
    /// Assigns the first caption to the Alt field in the Image section upon execution of the command.
    /// This command will NOT replace the value of the Alt field if a value already exists.
    /// </summary>
    public class AddAltTextCommand : Command
    {
        public override void Execute(CommandContext context)
        {
            Assert.IsNotNull(context,"context");

            var contextItem = context.Items.FirstOrDefault();

            if (contextItem == null || !contextItem.IsImage())
                return;

            MediaItem mediaItem = contextItem;

            ImageTagging.AddAltText(mediaItem, true);
            
        }
    }
}
