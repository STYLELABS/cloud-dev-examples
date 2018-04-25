using System.Globalization;

namespace Stylelabs.Integration.Reference.Training
{
    public static class Constants
    {
        public static readonly CultureInfo DefaultCulture = CultureInfo.GetCultureInfo("en-US");

        public static class EntityDefinitions
        {
            public static class Asset
            {
                public const string DefinitionName = "M.Asset";

                public static class MemberGroups
                {
                    public const string Content = "Content";
                }

                public static class Properties
                {
                    public const string ApprovalDate = "ApprovalDate";
                    public const string Title = "Title";
                    public const string Description = "Description";
                    public const string FileName = "FileName";
                }

                public static class Relations
                {
                    public const string AssetTypeToAsset = "AssetTypeToAsset";
                    public const string AssetMediaToAsset = "AssetMediaToAsset";
                    public const string ContentRepositoryToAsset = "ContentRepositoryToAsset";
                    public const string FinalLifeCycleStatusToAsset = "FinalLifeCycleStatusToAsset";
                }
            }

            public static class ContentRepository
            {
                public const string DefinitionName = "M.Content.Repository";
            }

            public static class FinalLifeCycleStatus
            {
                public const string DefinitionName = "M.Final.LifeCycle.Status";

                public static class Properties
                {
                    public const string StatusValue = "StatusValue";
                }
            }

            public static class AssetType
            {
                public const string DefinitionName = "M.AssetType";

                public static class Properties
                {
                    public const string Label = "Label";
                }

                public static class Relations
                {
                    public const string AssetTypeToAsset = "AssetTypeToAsset";
                }
            }

            public static class AssetMedia
            {
                public const string DefinitionName = "M.AssetMedia";

                public static class Properties
                {
                    public const string ClassificationName = "ClassificationName";
                }

                public static class Relations
                {
                    public const string AssetMediaToAsset = "AssetMediaToAsset";
                }
            }

            public static class UserGroup
            {
                public const string DefinitionName = "UserGroup";

                public static class Properties
                {
                    public const string GroupName = "GroupName";
                }
            }

            public static class Training
            {
                public const string DefinitionName = "Training";

                public static class Properties
                {
                    public const string Name = "Name";
                    public const string Description = "Description";
                }
            }

            public static class Chef
            {
                public const string DefinitionName = "Chef";

                public static class MemberGroups
                {
                    public const string Main = "Main";
                }

                public static class Properties
                {
                    public const string Name = DefinitionName + "Name";
                    public const string Bio = DefinitionName + "Bio";
                }

                public static class Relations
                {
                    public const string ChefToRecipe = "ChefToRecipe";
                }
            }

            public static class Recipe
            {
                public const string DefinitionName = "Recipe";
                
                public static class MemberGroups
                {
                    public const string Main = "Main";
                }

                public static class Properties
                {
                    public const string Name = DefinitionName + "Name";
                    public const string Description = DefinitionName + "Description";
                    public const string Preparation = DefinitionName + "Preparation";
                    public const string Servings = DefinitionName + "Servings";
                    public const string PublishDate = DefinitionName + "PublishDate";
                    public const string Vegetarian = DefinitionName + "Vegetarian";
                    public const string Categories = DefinitionName + "Categories";
                }

                public static class Relations
                {
                    public const string RecipeToMasterAsset = "RecipeToMasterAsset";
                }
            }

            public static class Job
            {
                public const string DefinitionName = "M.Job";

                public static class Properties
                {
                    public const string Condition = "Job.Condition";
                    public const string State = "Job.State";
                    public const string Type = "Job.Type";
                }

                public static class Conditions
                {
                    public const string Failed = "Failed";
                    public const string Pending = "Pending";
                    public const string Success = "Success";
                }

                public static class States
                {
                    public const string Failed = "Failed";
                    public const string Pending = "Pending";
                    public const string Completed = "Completed";
                }

                public static class Types
                {
                    public const string Processing = "Processing";
                }
            }
        }

        public static class DataSources
        {
            public const string RecipeCategories = "RecipeCategories";
        }

        public static class Permissions
        {
            public const string TrainingPermission = "TrainingPermission";
            public const string CookingPermission = "CookingPermission";
        }

        public static class Properties
        {
            public const string Identifier = "Identifier";
            public const string CreatedOn = "created_on";
            public const string ModifiedOn = "modified_on";
        }

        public static class Renditions
        {
            public const string Preview = "preview";
            public const string Thumbnail = "thumbnail";
        }

        public static class LifeCycleStatus
        {
            public const string Created = "M.Final.LifeCycle.Status.Created";
            public const string Approved = "M.Final.LifeCycle.Status.Approved";
            public const string Rejected = "M.Final.LifeCycle.Status.Rejected";
            public const string Archived = "M.Final.LifeCycle.Status.Archived";
        }

        public static class ContentRepositories
        {
            public const string Standard = "M.Content.Repository.Standard";
        }

        public static class AppSettings
        {
            public const string MOriginAddress = "MOriginAddress";
            public const string MClientId = "MClientId";
            public const string MClientSecret = "MClientSecret";
            public const string MUsername = "MUsername";
            public const string MPassword = "MPassword";
            public const string TempDirectory = "TempDirectory";
            public const string VisionKey = "VisionKey";
        }
    }
}
