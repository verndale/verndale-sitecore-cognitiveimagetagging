using Sitecore.Data.Items;
using Sitecore.SecurityModel;
using Sitecore.Shell.Framework.Commands;
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
			var item = context?.Items?[0];

			if (item == null || !item.IsImage(out var mediaItem))
			{
				Sitecore.Context.ClientPage.ClientResponse.Alert("Selected Item is not an Image.");
				return;
			}

			if (!string.IsNullOrEmpty(mediaItem.Alt))
			{
				Sitecore.Context.ClientPage.ClientResponse.Alert("Selected Item already has a valid Alt value.");
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
