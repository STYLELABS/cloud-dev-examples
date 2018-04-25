using System.Globalization;

namespace Stylelabs.Integration.Reference.TrainingFunctions.Helpers
{
    public class Constants
    {
        public static readonly CultureInfo DefaultCulture = CultureInfo.GetCultureInfo("en-US");

        public class Definitions
        {
            public const string Asset = "M.Asset";
            public const string FinalLifeCycleStatus = "M.Final.LifeCycle.Status";
            public const string ContentRepository = "M.Content.Repository";
        }

        public class Properties
        {
            public const string IsReleased = "IsReleased";
            public const string ReleaseDate = "ReleaseDate";
            public const string ClassificationName = "ClassificationName";
            public const string StatusValue = "StatusValue";
            public const string Title = "Title";
        }

        public class Relations
        {
            public const string FinalLifeCycleStatusToAsset = "FinalLifeCycleStatusToAsset";
            public const string ContentRepositoryToAsset = "ContentRepositoryToAsset";
        }
    }
}
