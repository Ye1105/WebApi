using Manager.Core.Enums;
using Manager.Core.Page;
using Manager.Infrastructure.Database;
using Manager.Infrastructure.IRepositoies;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Linq.Expressions;
using System.Reflection;

namespace Manager.Infrastructure.Repositoies
{
    public class BaseRepository : IBase
    {
        private readonly H5Context db;

        public BaseRepository(H5Context db)
        {
            this.db = db;

            //关闭全局追踪的代码
            //db.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public BaseRepository()
        {
        }

        public IQueryable<T> Entities<T>() where T : class
        {
            return db.Set<T>();
        }

        public IQueryable<T> EntitiesNoTrack<T>() where T : class
        {
            return db.Set<T>().AsNoTracking();
        }

        public int Add<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Added;
            return db.SaveChanges();
        }

        public int Del<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Deleted;
            return db.SaveChanges();
        }

        public int Del<T>(Expression<Func<T, bool>> delWhere) where T : class
        {
            List<T> listDels = db.Set<T>().Where(delWhere).ToList();
            db.RemoveRange(listDels);
            return db.SaveChanges();
        }

        public int Update<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Modified;
            return db.SaveChanges();
        }

        public int Update<T>(T model, Expression<Func<T, bool>> whereLambda, params string[] proNames) where T : class
        {
            List<T> listModifies = db.Set<T>().Where(whereLambda).ToList();
            Type t = typeof(T);
            List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            Dictionary<string, PropertyInfo> dicPros = new();
            proInfos.ForEach(p =>
            {
                if (proNames.Contains(p.Name))
                {
                    dicPros.Add(p.Name, p);
                }
            });
            foreach (string proName in proNames)
            {
                if (dicPros.ContainsKey(proName))
                {
                    PropertyInfo proInfo = dicPros[proName];
                    object newValue = proInfo.GetValue(model, null);
                    foreach (T m in listModifies)
                    {
                        proInfo.SetValue(m, newValue, null);
                    }
                }
            }
            return db.SaveChanges();
        }

        public T FirstOrDefault<T>(Expression<Func<T, bool>> whereLambda, bool isTrack = true) where T : class
        {
            var query = db.Set<T>().Where(whereLambda);

            return isTrack ? query.FirstOrDefault() : query.AsNoTracking().FirstOrDefault();
        }

        public List<T> Query<T>(Expression<Func<T, bool>> whereLambda, bool isTrack = true) where T : class
        {
            var query = db.Set<T>().Where(whereLambda);

            return isTrack ? query.ToList() : query.AsNoTracking().ToList();
        }

        public List<T> Query<T>(Expression<Func<T, bool>> whereLambda, bool isAsc = true, bool isTrack = true, string orderBy = "") where T : class
        {
            var query = db.Set<T>().Where(whereLambda);

            if (!isTrack)
            {
                query = query.AsNoTracking();
            }

            query = query.ApplySort(orderBy);

            return query.ToList();
        }

        public int SaveChange()
        {
            return db.SaveChanges();
        }

        public int ExecuteSql(string sql, params MySqlParameter[] pars)
        {
            return db.Database.ExecuteSqlRaw(sql, pars);
        }

        public IQueryable<T> ExecuteQuery<T>(string sql, bool isTrack = true, params MySqlParameter[] pars) where T : class
        {
            var query = db.Set<T>().FromSqlRaw(sql, pars);

            if (!isTrack)
            {
                return query.AsNoTracking();
            }

            return query;
        }

        public IQueryable<T> ExecuteQueryWhere<T>(string sql, Expression<Func<T, bool>> whereLambda, bool isTrack = true, params MySqlParameter[] pars) where T : class
        {
            var query = db.Set<T>().FromSqlRaw(sql, pars).Where(whereLambda);

            if (!isTrack)
            {
                return query.AsNoTracking();
            }

            return query;
        }

        /*************************下面进行方法的封装（异步）*************************/

        public async Task<int> AddAsync<T>(T model) where T : class
        {
            await db.AddAsync(model);
            return await db.SaveChangesAsync();
        }

        public async Task<int> AddRangeAsync<T>(IEnumerable<T> collection) where T : class
        {
            db.AddRange(collection);
            return await db.SaveChangesAsync();
        }

        public async Task<int> DelAsync<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Deleted;
            return await db.SaveChangesAsync();
        }

        public async Task<int> DelRangeAsync<T>(IEnumerable<T> collection) where T : class
        {
            db.RemoveRange(collection);
            return await db.SaveChangesAsync();
        }

        public async Task<int> DelAsync<T>(Expression<Func<T, bool>> delWhere) where T : class
        {
            List<T> listDels = await db.Set<T>().Where(delWhere).ToListAsync();
            db.RemoveRange(listDels);
            return await db.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync<T>(T model) where T : class
        {
            db.Entry(model).State = EntityState.Modified;
            return await db.SaveChangesAsync();
        }

        public async Task<int> UpdateRangeAsync<T>(IEnumerable<T> collection) where T : class
        {
            db.UpdateRange(collection);
            return await db.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync<T>(T model, Expression<Func<T, bool>> whereLambda, params string[] proNames) where T : class
        {
            List<T> listModifes = await db.Set<T>().Where(whereLambda).ToListAsync();
            Type t = typeof(T);
            List<PropertyInfo> proInfos = t.GetProperties(BindingFlags.Instance | BindingFlags.Public).ToList();
            Dictionary<string, PropertyInfo> dicPros = new();
            proInfos.ForEach(p =>
            {
                if (proNames.Contains(p.Name))
                {
                    dicPros.Add(p.Name, p);
                }
            });
            foreach (string proName in proNames)
            {
                if (dicPros.ContainsKey(proName))
                {
                    PropertyInfo proInfo = dicPros[proName];
                    object newValue = proInfo.GetValue(model, null);
                    foreach (T m in listModifes)
                    {
                        proInfo.SetValue(m, newValue, null);
                    }
                }
            }
            return await db.SaveChangesAsync();
        }

        public async Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> whereLambda, bool isTrack = true) where T : class
        {
            if (isTrack)
            {
                return await db.Set<T>().FirstOrDefaultAsync(whereLambda);
            }
            else
            {
                return await db.Set<T>().Where(whereLambda).AsNoTracking().FirstOrDefaultAsync();
            }
        }

        public async Task<T> FirstOrDefaultAsync<T>(Expression<Func<T, bool>> whereLambda, bool isTrack = true, string orderBy = "") where T : class
        {
            var query = db.Set<T>().Where(whereLambda);

            if (!isTrack)
            {
                query = query.AsNoTracking();
            }

            query = query.ApplySort(orderBy);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> whereLambda, bool isTrack = true, string? orderBy = null) where T : class
        {
            var query = Entities<T>().Where(whereLambda);

            if (!isTrack)
            {
                query = query.AsNoTracking();
            }

            query = query.ApplySort(orderBy);

            return await query.ToListAsync();
        }

        public async Task<List<T>> QueryAsync<T>(Expression<Func<T, bool>> whereLambda, int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "") where T : class
        {
            var query = db.Set<T>().Where(whereLambda);

            if (!isTrack)
            {
                query = query.AsNoTracking();
            }

            //排序
            query = query.ApplySort(orderBy);

            //分页
            query = query.Skip((pageIndex - 1) * pageSize + offset).Take(pageSize);

            return await query.ToListAsync();
        }

        public async Task<PagedList<T>> QueryPagedAsync<T>(Expression<Func<T, bool>> whereLambda, int pageIndex = 1, int pageSize = 10, int offset = 0, bool isTrack = true, string orderBy = "") where T : class
        {
            var query = db.Set<T>().Where(whereLambda);

            if (!isTrack)
            {
                query = query.AsNoTracking();
            }

            query = query.ApplySort(orderBy);

            return await PagedList<T>.CreateAsync(query, pageIndex, pageSize, offset);
        }

        public async Task<int> SaveChangeAsync()
        {
            return await db.SaveChangesAsync();
        }

        public async Task<int> ExecuteSqlAsync(string sql, params MySqlParameter[] pars)
        {
            return await db.Database.ExecuteSqlRawAsync(sql, pars);
        }

        public async Task<bool> BatchTransactionAsync(Dictionary<object, CrudEnum> keyValuePairs)
        {
            if (keyValuePairs.Count == 0)
                return false;

            using var transaction = db.Database.BeginTransaction();

            var createRange = keyValuePairs.Where(x => x.Value == CrudEnum.CREATE);
            if (createRange is not null && createRange.Any())
            {
                var list = createRange.Select(x => x.Key).ToList();
                db.AddRange(list);
            }

            var updateRange = keyValuePairs.Where(x => x.Value == CrudEnum.UPDATE);
            if (updateRange is not null && updateRange.Any())
            {
                var list = updateRange.Select(x => x.Key).ToList();
                db.UpdateRange(list);
            }

            var deleteRange = keyValuePairs.Where(x => x.Value == CrudEnum.DELETE);
            if (deleteRange is not null && deleteRange.Any())
            {
                var list = deleteRange.Select(x => x.Key).ToList();
                db.RemoveRange(list);
            }

            if (await SaveChangeAsync() > 0)
            {
                transaction.Commit();
                return true;
            }
            else
            {
                transaction.Rollback();
                return false;
            }
        }

        public bool BatchTransactionSync(Dictionary<object, CrudEnum> keyValuePairs)
        {
            if (keyValuePairs.Count == 0)
                return false;

            using var transaction = db.Database.BeginTransaction();

            var createRange = keyValuePairs.Where(x => x.Value == CrudEnum.CREATE);
            if (createRange is not null && createRange.Any())
            {
                var list = createRange.Select(x => x.Key).ToList();
                db.AddRange(list);
            }

            var updateRange = keyValuePairs.Where(x => x.Value == CrudEnum.UPDATE);
            if (updateRange is not null && updateRange.Any())
            {
                var list = updateRange.Select(x => x.Key).ToList();
                db.UpdateRange(list);
            }

            var deleteRange = keyValuePairs.Where(x => x.Value == CrudEnum.DELETE);
            if (deleteRange is not null && deleteRange.Any())
            {
                var list = deleteRange.Select(x => x.Key).ToList();
                db.RemoveRange(list);
            }

            if (SaveChange() > 0)
            {
                transaction.Commit();
                return true;
            }
            else
            {
                transaction.Rollback();
                return false;
            }
        }
    }
}