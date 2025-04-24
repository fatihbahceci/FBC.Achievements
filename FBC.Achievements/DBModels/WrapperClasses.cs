using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Linq.Dynamic.Core;

namespace FBC.Achievements.DBModels
{
    public class WUserWithAchievements
    {
        /// <summary>
        /// From DBUser
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// From DBUser
        /// </summary>
        public string UserFullName { get; set; } = String.Empty; // Kullanıcının tam adı
        /// <summary>
        /// From DBUser
        /// </summary>
        public string UserName { get; set; } = String.Empty; // Kullanıcının kullanıcı adı
        /// <summary>
        /// Calculated property
        /// </summary>
        public double Score { get; set; }
        /// <summary>
        /// For UI
        /// </summary>
        public bool ShowDetails { get; set; } = false; // Kullanıcının detaylarını gösterip göstermeyeceği

        public List<WUserAchievement> UserAchievements { get; set; } = new List<WUserAchievement>(); // Kullanıcının tüm görevleri

        public static DataResult<WUserWithAchievements> LoadData(DB db, LoadDataArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            var r = new DataResult<WUserWithAchievements>()
            {
                Success = false,
            };
            var tr = WUserAchievement.LoadData(db, args, true);
            if (tr.Success)
            {
                r.Data = tr.Data?.GroupBy(x => x.UserId)
                    .Select(g => new WUserWithAchievements
                    {
                        UserId = g.Key,
                        UserFullName = g.FirstOrDefault()!.UserFullName,
                        UserName = g.FirstOrDefault()!.UserName,
                        UserAchievements = g.ToList(),
                        Score =Math.Round( g.Sum(x => x.Score),2) // / g.Count()
                    }).ToList();

                r.TotalCount = r.Data?.Count ?? 0;
                if (r.Data?.Count > 0)
                {
                    var q = r.Data.AsQueryable();
                    if (args.Filter != null)
                    {
                        q = q.Where(args.Filter);
                    }
                    if (args.OrderBy != null)
                    {
                        q = q.OrderBy(args.OrderBy);
                    }
                    //if (!byPassTakeSkip)
                    //{
                    int skip = args.Skip ?? 0;
                    int take = args.Top ?? 0;
                    if (skip > 0)
                    {
                        q = q.Skip(skip);
                    }
                    if (take > 0)
                    {
                        q = q.Take(take);
                    }
                    //}
                    r.Data = q.ToList();
                }
                r.Success = true;
                return r;
            }
            else
            {
                r.Messages.AddRange(tr.Messages);
                return r;
            }
        }
    }
    public class WUserAchievement
    {
        /// <summary>
        /// From DBUserAchievement
        /// </summary>
        public int UserAchievementId { get; set; }
        /// <summary>
        /// From DBUserAchievement
        /// </summary>
        public int UserId { get; set; }
        /// <summary>
        /// From DBUserAchievement
        /// </summary>
        public int AchievementId { get; set; }
        /// <summary>
        /// From DBAchievement
        /// </summary>
        public string AchievementTitle { get; set; } = String.Empty; // Görev adı
        /// <summary>
        /// From DBAchievement
        /// </summary>
        public string AchievementDescription { get; set; } = String.Empty; // Görev açıklaması

        /// <summary>
        /// From DBUserAchievement
        /// </summary>
        //public int CompletedGoals { get; set; } // Kullanıcının görevi kaç kez tamamladığı
        /// <summary>
        /// From DBUserAchievement
        /// </summary>
        public int CurrentGoalProgress { get; set; } // Kullanıcının son tamamlanmamış görevi tamamlamak için kaç adım attığı
        /// <summary>
        /// From DBAchievement
        /// </summary>
        public int Goal { get; set; } // Görevin tamamlanması için gereken sayı (örneğin: 100 plastik atık)
        /// <summary>
        /// From DBUser
        /// </summary>
        public string UserName { get; set; } = String.Empty; // Kullanıcının kullanıcı adı
        /// <summary>
        /// From DBUser
        /// </summary>
        public string UserFullName { get; set; } = String.Empty; // Kullanıcının tam adı

        public double Score { get; set; }
        //    get
        //    {
        //        if (Goal == 0 || CurrentGoalProgress == 0)
        //        {
        //            return 0;
        //        }

        //        return Math.Round((double)CurrentGoalProgress / Goal * 100, 2);
        //    }
        //}

        // Kullanıcının görevi tamamlamak için gereken puanı

        public static WUserAchievement? LoadByUserAchievementId(DB db, int DBUserAchievementId)
        {
            // select DBUserAchievement join DBAchievement
            var query = from u in db.UserAchievements.AsNoTracking()
                        join a in db.Achievements.AsNoTracking() on u.AchievementId equals a.Id
                        join us in db.Users.AsNoTracking() on u.UserId equals us.Id
                        where u.Id == DBUserAchievementId
                        select new WUserAchievement
                        {
                            UserAchievementId = u.Id,
                            UserId = u.UserId,
                            AchievementId = u.AchievementId,
                            //CompletedGoals = u.CompletedGoals,
                            CurrentGoalProgress = u.CurrentGoalProgress,
                            Goal = a.Goal,
                            UserFullName = us.FullName,
                            UserName = us.UserName,
                            AchievementTitle = a.Title,
                            AchievementDescription = a.Description ?? "",
                        };
            return query.FirstOrDefault();

        }

        public static List<WUserAchievement> LoadByAchievementId(DB db, int DBAchievementId)
        {

            // select DBUserAchievement join DBAchievement
            var query = from u in db.UserAchievements.AsNoTracking()
                        join a in db.Achievements.AsNoTracking() on u.AchievementId equals a.Id
                        join us in db.Users.AsNoTracking() on u.UserId equals us.Id
                        where u.AchievementId == DBAchievementId
                        select new WUserAchievement
                        {
                            UserAchievementId = u.Id,
                            UserId = u.UserId,
                            AchievementId = u.AchievementId,
                            //CompletedGoals = u.CompletedGoals,
                            CurrentGoalProgress = u.CurrentGoalProgress,
                            Goal = a.Goal,
                            UserFullName = us.FullName,
                            UserName = us.UserName,
                            AchievementTitle = a.Title,
                            AchievementDescription = a.Description ?? "",
                        };
            return query.ToList();
        }
        public static List<WUserAchievement> LoadByUserId(DB db, int DBUserId)
        {
            // select DBUserAchievement join DBAchievement
            var query = from u in db.UserAchievements.AsNoTracking()
                        join a in db.Achievements.AsNoTracking() on u.AchievementId equals a.Id
                        join us in db.Users.AsNoTracking() on u.UserId equals us.Id
                        where u.UserId == DBUserId
                        select new WUserAchievement
                        {
                            UserAchievementId = u.Id,
                            UserId = u.UserId,
                            AchievementId = u.AchievementId,
                            //CompletedGoals = u.CompletedGoals,
                            CurrentGoalProgress = u.CurrentGoalProgress,
                            Goal = a.Goal,
                            UserFullName = us.FullName,
                            UserName = us.UserName,
                            AchievementTitle = a.Title,
                            AchievementDescription = a.Description ?? "",
                        };
            return query.ToList();
        }

        public static DataResult<WUserAchievement> LoadData(DB db, LoadDataArgs args, bool byPassTakeSkip)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }
            var r = new DataResult<WUserAchievement>()
            {
                Success = false,
            };
            try
            {
                //Users, UserAchievements, Achievements birleştir sonra UserAchievements'i UserId'e göre gruplandır ve bu şekilde class oluştur sonra filteryi uygula
                var q = from u in db.Users.AsNoTracking()
                        join ua in db.UserAchievements.AsNoTracking() on u.Id equals ua.UserId into userAchievements
                        from ua in userAchievements.DefaultIfEmpty()
                        join a in db.Achievements.AsNoTracking() on ua.AchievementId equals a.Id into achievements
                        from a in achievements.DefaultIfEmpty()
                        where ua != null
                        select new WUserAchievement()
                        {
                            UserAchievementId = ua.Id,
                            UserId = u.Id,
                            AchievementId = a.Id,
                            CurrentGoalProgress = ua.CurrentGoalProgress,
                            Goal = a.Goal,
                            UserFullName = u.FullName,
                            Score = a.Goal == 0 ? 0 : Math.Round((double)ua.CurrentGoalProgress / a.Goal * 100, 2),
                            UserName = u.UserName,
                            AchievementTitle = a.Title,
                            AchievementDescription = a.Description ?? "",
                        };

                r.TotalCount = q.Count();
                if (args.Filter != null)
                {
                    q = q.Where(args.Filter);
                }
                if (args.OrderBy != null)
                {
                    q = q.OrderBy(args.OrderBy);
                }
                if (!byPassTakeSkip)
                {
                    int skip = args.Skip ?? 0;
                    int take = args.Top ?? 0;
                    if (skip > 0)
                    {
                        q = q.Skip(skip);
                    }
                    if (take > 0)
                    {
                        q = q.Take(take);
                    }
                }
                r.Data = q.ToList();
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