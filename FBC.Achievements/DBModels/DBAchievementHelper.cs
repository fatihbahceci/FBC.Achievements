using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Linq.Dynamic.Core;

namespace FBC.Achievements.DBModels
{
    public static class DBAchievementHelper
    {
        public static List<string> ValidateAchievement(this DBAchievement achievement)
        {
            var messages = new List<string>();
            if (achievement == null)
            {
                messages.Add("Kazanım geçersiz bir değere sahip");
                return messages;
            }
            if (string.IsNullOrWhiteSpace(achievement.Title))
            {
                messages.Add("Kazanım adı boş olamaz");
            }
            if (achievement.Goal <= 0)
            {
                messages.Add("Kazanım hedefi 0'dan büyük olmalıdır! 1: Tek seferlik kazanım, 1+: tekrar eden kazanım");
            }
            return messages;
        }

        public static DataOperationResult<DBAchievement> AddAchievement(this DB db, DBAchievement achievement)
        {
            var r = new DataOperationResult<DBAchievement>()
            {
                Success = false,
            };
            try
            {
                var v = ValidateAchievement(achievement);
                if (v.Count > 0)
                {
                    r.Messages.AddRange(v);
                    r.ErrorData.Add(achievement);
                    return r;
                }
                db.Achievements.Add(achievement);
                db.SaveChanges();
                r.Success = true;
                r.SuccessData.Add(achievement);
            }
            catch (Exception ex)
            {
                r.Messages.Add(ex.Message);
                r.ErrorData.Add(achievement);
            }
            return r;
        }

        public static DataOperationResult<DBAchievement> UpdateAchievement(this DB db, DBAchievement achievement)
        {
            var r = new DataOperationResult<DBAchievement>()
            {
                Success = false,
            };
            try
            {
                var v = ValidateAchievement(achievement);
                if (v.Count > 0)
                {
                    r.Messages.AddRange(v);
                    r.ErrorData.Add(achievement);
                    return r;
                }
                var existingAchievement = db.Achievements.Find(achievement.Id);
                if (existingAchievement == null)
                {
                    r.Messages.Add("Kazanım bulunamadı");
                    r.ErrorData.Add(achievement);
                    return r;
                }
                existingAchievement.Title = achievement.Title;
                existingAchievement.Description = achievement.Description;
                existingAchievement.Goal = achievement.Goal;
                db.SaveChanges();
                r.Success = true;
                r.SuccessData.Add(achievement);
            }
            catch (Exception ex)
            {
                r.Messages.Add(ex.Message);
                r.ErrorData.Add(achievement);
            }
            return r;
        }

        public static DataOperationResult DeleteAchievement(this DB db, int id)
        {
            var r = new DataOperationResult()
            {
                Success = false,
            };
            try
            {
                var achievement = db.Achievements.Find(id);
                if (achievement == null)
                {
                    r.Messages.Add($"Kazanım bulunamadı ({id})");
                    return r;
                }
                var userAchievements = db.UserAchievements.AsNoTracking().Where(ua => ua.AchievementId == id).Select(x => x.Id).ToList();
                if (userAchievements.Count > 0)
                {
                    foreach (var uaId in userAchievements)
                    {
                        var ur = db.DeleteUserAchievement(uaId);
                        if (!ur.Success)
                        {
                            //TODO Log
                            Console.WriteLine($"Kullanıcı kazanımı silinemedi ({uaId})");
                        }
                    }
                    //r.Messages.Add($"Kazanım silinemez, çünkü {userAchievements.Count} kullanıcı kazanımı var.");
                    //r.ErrorData.Add(false);
                    //return r;
                }
                db.Achievements.Remove(achievement);
                db.SaveChanges();
                r.Success = true;
            }
            catch (Exception ex)
            {
                r.Messages.Add(ex.Message);
            }
            return r;

        }
    }

}
