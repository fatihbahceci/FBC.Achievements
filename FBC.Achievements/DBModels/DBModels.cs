using System.ComponentModel.DataAnnotations;

namespace FBC.Achievements.DBModels
{
    public enum UserType
    {
        [Display(Name = "Seçiniz...")]
        NotSet,
        [Display(Name = "Öğrenci")]
        User,
        [Display(Name = "Eğitmen")]
        Mentor,
        [Display(Name = "Yönetici")]
        Admin
    }
    public class DBUser
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public UserType UserType { get; set; } = UserType.NotSet;
        public string? Email { get; set; }
        public string FullName { get; set; } = String.Empty;
    }
    public class DBAchievement
    {
        [Key]
        public int Id { get; set; }
        [MinLength(1)]
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public int Goal { get; set; } // Görevin tamamlanması için gereken sayı (örneğin: 100 plastik atık)
        //public List<UserTask> UserTasks { get; set; } = new();
    }
    public class DBUserAchievement
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int AchievementId { get; set; }
        //public int CompletedGoals { get; set; } // Kullanıcının görevi kaç kez tamamladığı
        public int CurrentGoalProgress { get; set; } // Kullanıcının son tamamlanmamış görevi tamamlamak için kaç adım attığı
    }
}
