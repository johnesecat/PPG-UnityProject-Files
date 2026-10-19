using Steamworks.Data;

namespace Steamworks.Ugc
{
	public struct PublishResult
	{
		public Result Result;

		public PublishedFileId FileId;

		public bool NeedsWorkshopAgreement;

		public bool Success => Result == Result.OK;
	}
}
