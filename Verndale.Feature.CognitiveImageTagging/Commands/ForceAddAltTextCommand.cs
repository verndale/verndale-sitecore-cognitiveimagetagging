using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using Sitecore.Shell.Framework.Commands;
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
			var item = context?.Items?[0];

			if (item == null || !item.IsImage(out var mediaItem))
			{
				Sitecore.Context.ClientPage.ClientResponse.Alert("Selected Item is not an Image.");
				return;
			}

			var text = mediaItem.GetAltTextFromAI();

			using (new SecurityDisabler())
			{

				using (new EditContext(mediaItem))
				{
					mediaItem.InnerItem.Fields["Alt"].Value = text;
					Sitecore.Context.ClientPage.ClientResponse.Alert($"Alt text returned from Azure:<br> {text}.");
				}

			}
		}
	}
}
