using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Linq.Dynamic.Core;

namespace FBC.Achievements.DBModels
{

    public class DataOperationResult
    {
        public bool Success { get; set; }
        public List<string> Messages { get; private set; } = new List<string>();
    }


    public class DataOperationResult<T> : DataOperationResult
    {
        public List<T> SuccessData { get; private set; } = new List<T>();
        public List<T> ErrorData { get; private set; } = new List<T>();
    }

    public class DataResult<T>
    {
        public bool Success { get; set; }
        public List<string> Messages { get; private set; } = new List<string>();
        public List<T>? Data { get; set; }
        public int TotalCount { get; set; }
    }
    public static class DBGenericHelper
    {
        public static DataResult<T> LoadData<T>(this DbSet<T> collection, LoadDataArgs args) where T : class
        {
            var r = new DataResult<T>()
            {
                Success = false,
            };
            try
            {
                var q = collection.AsNoTracking().AsQueryable();
                r.TotalCount = q.Count();
                if (args.Filter != null)
                {

                    q = q.Where(args.Filter);
                }
                if (args.OrderBy != null)
                {
                    q = q.OrderBy(args.OrderBy);
                }
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
                r.Data = q.ToList();
                r.Success = true;

            }
            catch (Exception exc)
            {
                r.Data = null;
                r.Messages.Add(exc.Message);
            }
            return r;
        }
    }
}
