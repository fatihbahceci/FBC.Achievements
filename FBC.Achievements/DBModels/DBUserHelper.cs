using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Linq.Dynamic.Core;

namespace FBC.Achievements.DBModels
{
    public static class DBUserHelper
    {
        public static List<string> ValidateUser(this DBUser user)
        {
            var messages = new List<string>();
            if (user == null)
            {
                messages.Add("Kullanıcı geçersiz bir değere sahip");
                return messages;
            }
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                messages.Add("Kullanıcı adı boş olamaz");
            }
            if (string.IsNullOrEmpty(user.FullName))
            {
                messages.Add("Kullanıcı ismi boş olamaz");
            }
            //if (string.IsNullOrWhiteSpace(user.Password))
            //{
            //    messages.Add("Şifre boş olamaz");
            //}
            if (user.UserType == UserType.NotSet)
            {
                messages.Add("Kullanıcı tipi seçilmemiş");
            }
            return messages;
        }
        public static DataOperationResult<DBUser> AddUser(this DB db, DBUser user)
        {
            var r = new DataOperationResult<DBUser>()
            {
                Success = false,
            };
            try
            {
                var v = ValidateUser(user);
                if (v.Count > 0)
                {
                    r.Messages.AddRange(v);
                    r.ErrorData.Add(user);
                    return r;
                }
                var existsingUser = db.Users.AsNoTracking().FirstOrDefault(x => x.UserName == user.UserName);
                if (existsingUser != null)
                {
                    r.Messages.Add("Kullanıcı adı zaten var");
                    r.ErrorData.Add(user);
                    return r;
                }
                db.Users.Add(user);
                db.SaveChanges();
                r.Success = true;
                r.SuccessData.Add(user);
            }
            catch (Exception ex)
            {
                r.Messages.Add(ex.Message);
                r.ErrorData.Add(user);
            }
            return r;
        }
        public static DataOperationResult<DBUser> UpdateUser(this DB db, DBUser user)
        {
            var r = new DataOperationResult<DBUser>()
            {
                Success = false,
            };
            try
            {
                var v = ValidateUser(user);
                if (v.Count > 0)
                {
                    r.Messages.AddRange(v);
                    r.ErrorData.Add(user);
                    return r;
                }
                var existingUser = db.Users.Find(user.Id);
                if (existingUser == null)
                {
                    r.Messages.Add("Kullanıcı bulunamadı");
                    r.ErrorData.Add(user);
                    return r;
                }
                //Check is only admin user
                if (existingUser.UserType == UserType.Admin && user.UserType != UserType.Admin && db.Users.Count(x => x.UserType == UserType.Admin) <= 1)
                {
                    r.Messages.Add("Eğer sadece bir tane admin kullanıcınız varsa, bu kullanıcıyı admin olarak bırakmalısınız.");
                    r.ErrorData.Add(user);
                    return r;
                }
                existingUser.UserName = user.UserName;
                existingUser.Password = user.Password;
                existingUser.UserType = user.UserType;
                existingUser.Email = user.Email;
                existingUser.FullName = user.FullName;
                db.SaveChanges();
                r.Success = true;
                r.SuccessData.Add(user);
            }
            catch (Exception ex)
            {
                r.Messages.Add(ex.Message);
                r.ErrorData.Add(user);
            }
            return r;
        }
        public static DataOperationResult DeleteUser(this DB db, int id)
        {
            var r = new DataOperationResult()
            {
                Success = false,
            };
            try
            {
                var user = db.Users.Find(id);
                if (user == null)
                {
                    r.Messages.Add($"Kullanıcı bulunamadı ({id})");
                    return r;
                }
                //Check is only admin user
                if (user.UserType == UserType.Admin && db.Users.Count(x => x.UserType == UserType.Admin) <= 1)
                {
                    r.Messages.Add("Eğer sadece bir tane admin kullanıcınız varsa, bu kullanıcıyı silemezsiniz.");
                    return r;
                }
                var userAchievements = db.UserAchievements.AsNoTracking().Where(ua => ua.UserId == id).Select(x => x.Id).ToList();
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
                    //r.Messages.Add($"Kullanıcı silinemez, çünkü {userAchievements.Count} kullanıcı kazanımı var.");
                    //r.ErrorData.Add(false);
                    //return r;
                }
                db.Users.Remove(user);
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
