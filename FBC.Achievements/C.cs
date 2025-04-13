using FBC.Achievements.DBModels;
using Radzen.Blazor;

namespace FBC.Achievements
{
    public static class C
    {
        public static class User
        {
            //Login olabilme yetisine sahip kullanıcı türleri
            public readonly static UserType[] UsersThatCanLogin =
            [
                UserType.Admin,
                UserType.Mentor
            ];
        }
        public static class Static
        {
            public const string PicturesPath = "Pictures/";
            public const string AchievementPicturesPath = PicturesPath + "AchievementPictures/";

            public static void CreateAchievementPicturesDirectoryIfNotExists(IWebHostEnvironment env)
            {

                var path = GetAchievementPicturesDirectoryPath(env);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
            public static string GetAchievementPicturesDirectoryPath(IWebHostEnvironment env)
            {
                return Path.Combine(env.WebRootPath, AchievementPicturesPath);
            }
            public static string GetAchievementPictureFilePath(IWebHostEnvironment env, int achievementId)
            {
                return Path.Combine(env.WebRootPath, AchievementPicturesPath, $"{achievementId}.jpg");
            }
            public static string GetAchievementPictureWebPath(IWebHostEnvironment env, int achievementId)
            {
                return $"/{AchievementPicturesPath}{achievementId}.jpg";
            }
            public static string GetAchievementPictureWebPathOrDefault(IWebHostEnvironment env, int achievementId)
            {
                if (File.Exists(GetAchievementPictureFilePath(env, achievementId)))
                {
                    return GetAchievementPictureWebPath(env, achievementId);
                }
                else
                {
                    return $"/achievement.jpg";
                }
            }

        }
        public static class NAV
        {
            public const string HomePage = "/";
            public const string Login = "/Login";
            public static class Define
            {
                private const string Base = "Define/";
                public const string Achievements = Base + "Achievements/";
                public const string Users = Base + "Users/";
                public const string AchievementActions = Base + "AchievementActions/";
                public const string Tools = Base + "Tools/";
                public const string UserActions = Base + "UserActions/";
            }
        }

        public static class Tools
        {
            public static string ToMD5(string str)
            {
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    var inputBytes = System.Text.Encoding.ASCII.GetBytes(str);
                    var hashBytes = md5.ComputeHash(inputBytes);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }
            public static string ToMD5(byte[] bytes)
            {
                using (var md5 = System.Security.Cryptography.MD5.Create())
                {
                    var hashBytes = md5.ComputeHash(bytes);
                    return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
                }
            }
        }
        public static class Localization
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "BL0005:Component parameter should not be set outside of its component.", Justification = "<Pending>")]
            public static void LocalizeGrid<T>(RadzenDataGrid<T> grid)
            {
                if (grid != null)
                {
                    grid.AllColumnsText = "Tümü";
                    grid.GroupPanelText = "Gruplamak istediğiniz başlığı buraya sürükleyin";
                    grid.AndOperatorText = "Ve";
                    grid.ApplyFilterText = "Uygula";
                    grid.ClearFilterText = "Temizle";
                    grid.ColumnsShowingText = "sütun gösterimde";
                    grid.ColumnsText = "Sütunlar";
                    grid.ColumnWidth = "Sütun Genişliği";
                    grid.ContainsText = "İçeren";
                    grid.DoesNotContainText = "İçermeyen";
                    grid.EmptyText = "Gösterilecek kayıt yok";
                    grid.EndsWithText = "Biten";
                    grid.EnumFilterSelectText = "Seç...";
                    grid.EqualsText = "Eşit";
                    grid.FilterText = "Filtrele";
                    grid.GreaterThanOrEqualsText = "Büyük veya eşit";
                    grid.GreaterThanText = "Büyük";
                    grid.IsEmptyText = "Boş";
                    grid.IsNotEmptyText = "Boş değil";
                    grid.IsNotNullText = "Tanımsız değil";
                    grid.IsNullText = "Tanımsız";
                    grid.LessThanOrEqualsText = "Küçük ya da eşit";
                    grid.LessThanText = "Küçük";
                    grid.NotEqualsText = "Eşit değil";
                    grid.OrOperatorText = "Veya";
                    grid.PageSizeText = "Sayfa başı gösterim";
                    grid.PagingSummaryFormat = "Sayfa {0} / {1} (Toplam {2})";
                    grid.StartsWithText = "İle başlayan";


                }
            }
        }
        public static void LocalizeGrid<T>(this RadzenDataGrid<T> grid)
        {
            C.Localization.LocalizeGrid(grid);
        }

        public static string GetEnumDisplayName(this Enum value)
        {
            try
            {
                var field = value.GetType().GetField(value.ToString());
                if (field != null)
                {
                    var attribute = field.GetCustomAttributes(typeof(System.ComponentModel.DataAnnotations.DisplayAttribute), false).FirstOrDefault() as System.ComponentModel.DataAnnotations.DisplayAttribute;
                    if (attribute != null)
                    {
                        return attribute!.Name!;
                    }
                }
            }
            catch (Exception)
            {

            }
            return value.ToString();
        }


    }
}
