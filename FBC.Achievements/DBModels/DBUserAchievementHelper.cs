namespace FBC.Achievements.DBModels
{
    public static class DBUserAchievementHelper
    {
        public static List<string> ValidateUserAchievement(this DBUserAchievement userAchievement)
        {
            var messages = new List<string>();
            if (userAchievement == null)
            {
                messages.Add("Kullanıcı kazanımı geçersiz bir değere sahip");
                return messages;
            }
            if (userAchievement.UserId <= 0)
            {
                messages.Add("Kullanıcı ID'si geçersiz");
            }
            if (userAchievement.AchievementId <= 0)
            {
                messages.Add("Kazanım ID'si geçersiz");
            }
            return messages;
        }
        public static DataOperationResult<DBUserAchievement> AddUserAchievement(this DB db, DBUserAchievement userAchievement)
        {
            var r = new DataOperationResult<DBUserAchievement>()
            {
                Success = false,
            };
            try
            {
                var v = ValidateUserAchievement(userAchievement);
                if (v.Count > 0)
                {
                    r.Messages.AddRange(v);
                    r.ErrorData.Add(userAchievement);
                    return r;
                }
                db.UserAchievements.Add(userAchievement);
                db.SaveChanges();
                r.Success = true;
                r.SuccessData.Add(userAchievement);
            }
            catch (Exception ex)
            {
                r.Messages.Add(ex.Message);
                r.ErrorData.Add(userAchievement);
            }
            return r;
        }

        public static DataOperationResult UpdateUserGoalProgress(this DB db, int userAchievementId, int progress)
        {
            var r = new DataOperationResult()
            {
                Success = false,
            };
            try
            {
                var userAchievement = db.UserAchievements.Find(userAchievementId);
                if (userAchievement == null)
                {
                    r.Messages.Add($"Kullanıcı kazanımı bulunamadı ({userAchievementId})");
                    return r;
                }
                //Check if progress is less than 0
                if (progress < 0)
                {
                    r.Messages.Add($"Kullanıcı kazanım hedefi negatif olamaz ({userAchievementId})");
                    return r;
                }
                //Check if progress is greater than goal
                var achievement = db.Achievements.Find(userAchievement.AchievementId);
                if (achievement == null)
                {
                    r.Messages.Add($"Kazanım bulunamadı ({userAchievement.AchievementId})");
                    return r;
                }
                if (progress > achievement.Goal)
                {
                    r.Messages.Add($"Kullanıcı kazanım hedefi aşıldı ({userAchievementId})");
                    return r;
                }


                userAchievement.CurrentGoalProgress = progress;
                db.SaveChanges();
                r.Success = true;
            }
            catch (Exception ex)
            {
                r.Messages.Add(ex.Message);
            }
            return r;
        }

        public static DataOperationResult<DBUserAchievement> UpdateUserAchievement(this DB db, DBUserAchievement userAchievement)
        {
            var r = new DataOperationResult<DBUserAchievement>()
            {
                Success = false,
            };
            try
            {
                var v = ValidateUserAchievement(userAchievement);
                if (v.Count > 0)
                {
                    r.Messages.AddRange(v);
                    r.ErrorData.Add(userAchievement);
                    return r;
                }
                var existingUserAchievement = db.UserAchievements.Find(userAchievement.Id);
                if (existingUserAchievement == null)
                {
                    r.Messages.Add("Kullanıcı kazanımı bulunamadı");
                    r.ErrorData.Add(userAchievement);
                    return r;
                }
                //existingUserAchievement.CompletedGoals = userAchievement.CompletedGoals;
                existingUserAchievement.CurrentGoalProgress = userAchievement.CurrentGoalProgress;
                db.SaveChanges();
                r.Success = true;
                r.SuccessData.Add(userAchievement);
            }
            catch (Exception ex)
            {
                r.Messages.Add(ex.Message);
                r.ErrorData.Add(userAchievement);
            }
            return r;
        }
        public static DataOperationResult DeleteUserAchievement(this DB db, int id)
        {
            var r = new DataOperationResult()
            {
                Success = false,
            };
            try
            {
                var userAchievement = db.UserAchievements.Find(id);
                if (userAchievement == null)
                {
                    r.Messages.Add($"Kullanıcı kazanımı bulunamadı ({id})");
                    return r;
                }
                db.UserAchievements.Remove(userAchievement);
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
