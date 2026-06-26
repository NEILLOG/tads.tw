using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System.Linq.Expressions;
using TADS_Web.Models;
using TADS_Web.Models.DB;

namespace TADS_Web.Service.Base
{
    public class ServiceBase
    {
        public DBContext _context { get; set; } = null!;

        public ServiceBase(DBContext context)
        {
            try { _context = context; }
            catch { }
        }

        public IQueryable<T>? Lookup<T>(ref String ErrMsg, Expression<Func<T, bool>>? filter = null) where T : class
        {
            try
            {
                var dataList = _context.Set<T>().AsNoTracking();
                if (filter != null)
                    dataList = dataList.Where(filter);
                return dataList;
            }
            catch (Exception ex)
            {
                ErrMsg = ErrMsg + "|" + ex.Message;
                return null;
            }
        }

        public async Task<T?> Find<T>(params object[] key) where T : class
        {
            try { return await _context.Set<T>().FindAsync(key); }
            catch { return null; }
        }

        public async Task<ActionResultModel<T>> Insert<T>(T entity, IDbContextTransaction? transaction = null) where T : class
        {
            ActionResultModel<T> result = new ActionResultModel<T>();
            try
            {
                if (entity == null)
                {
                    result.IsSuccess = false;
                    result.Description = "新增失敗";
                    result.Message = "傳入的entity是null";
                }
                else
                {
                    if (transaction != null)
                        _context.Database.UseTransaction(transaction.GetDbTransaction());
                    _context.Set<T>().Add(entity);
                    await _context.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Description = "新增成功";
                    result.Data = entity;
                }
            }
            catch (Exception ex)
            {
                _context.Entry<T>(entity!).State = EntityState.Detached;
                if (transaction != null) throw;
                else
                {
                    result.IsSuccess = false;
                    result.Description = "新增失敗";
                    result.Message = ex.ToString();
                }
            }
            return result;
        }

        public async Task<ActionResultModel<T>> Update<T>(T entity, IDbContextTransaction? transaction = null) where T : class
        {
            ActionResultModel<T> result = new ActionResultModel<T>();
            try
            {
                if (entity == null)
                {
                    result.IsSuccess = false;
                    result.Description = "更新失敗";
                    result.Message = "傳入的entity是null";
                }
                else
                {
                    if (transaction != null)
                        _context.Database.UseTransaction(transaction.GetDbTransaction());
                    _context.Update(entity);
                    await _context.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Description = "更新成功";
                    result.Data = entity;
                }
            }
            catch (Exception ex)
            {
                _context.Entry<T>(entity!).State = EntityState.Detached;
                if (transaction != null) throw;
                else
                {
                    result.IsSuccess = false;
                    result.Description = "更新失敗";
                    result.Message = ex.ToString();
                }
            }
            return result;
        }

        public async Task<ActionResultModel<IEnumerable<T>>> UpdateRange<T>(IEnumerable<T> entities, IDbContextTransaction? transaction = null) where T : class
        {
            ActionResultModel<IEnumerable<T>> result = new ActionResultModel<IEnumerable<T>>();
            try
            {
                if (entities == null)
                {
                    result.IsSuccess = false;
                    result.Description = "更新失敗";
                    result.Message = "傳入的entity是null";
                }
                else if (!entities.Any())
                {
                    result.IsSuccess = true;
                }
                else
                {
                    if (transaction != null)
                        _context.Database.UseTransaction(transaction.GetDbTransaction());
                    _context.UpdateRange(entities);
                    await _context.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Description = "更新成功";
                    result.Data = entities;
                }
            }
            catch (Exception ex)
            {
                foreach (var entity in entities!)
                    _context.Entry<T>(entity).State = EntityState.Detached;
                if (transaction != null) throw;
                else
                {
                    result.IsSuccess = false;
                    result.Description = "更新失敗";
                    result.Message = ex.ToString();
                }
            }
            return result;
        }

        public async Task<ActionResultModel<T>> Delete<T>(T entity, IDbContextTransaction? transaction = null) where T : class
        {
            ActionResultModel<T> result = new ActionResultModel<T>();
            try
            {
                if (entity == null)
                {
                    result.IsSuccess = false;
                    result.Description = "刪除失敗";
                    result.Message = "傳入的entity是null";
                }
                else
                {
                    if (transaction != null)
                        _context.Database.UseTransaction(transaction.GetDbTransaction());
                    _context.Remove(entity);
                    await _context.SaveChangesAsync();
                    result.IsSuccess = true;
                    result.Description = "刪除成功";
                    result.Data = entity;
                }
            }
            catch (Exception ex)
            {
                _context.Entry<T>(entity!).State = EntityState.Detached;
                if (transaction != null) throw;
                else
                {
                    result.IsSuccess = false;
                    result.Description = "刪除失敗";
                    result.Message = ex.ToString();
                }
            }
            return result;
        }

        public void SetEntityState<T>(T entity, EntityState state) where T : class
        {
            _context.Entry(entity).State = state;
        }

        public IDbContextTransaction GetTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public async Task<bool> ExecuteSqlCommand(string query)
        {
            try
            {
                await _context.Database.ExecuteSqlRawAsync(query);
                return true;
            }
            catch { throw; }
        }

        public async Task<bool> RowLock<T>(IDbContextTransaction transaction, string condition) where T : class
        {
            try
            {
                string table_name = typeof(T).Name;
                _context.Database.UseTransaction(transaction.GetDbTransaction());

                // SQL Server 才支援 WITH(updlock, rowlock, holdlock) 鎖定提示；
                // SQLite 等其他提供者不支援，於交易內以一般 SELECT 取代（寫入鎖已由交易提供）。
                bool isSqlServer = _context.Database.ProviderName == "Microsoft.EntityFrameworkCore.SqlServer";
                string query = isSqlServer
                    ? String.Format("SELECT * FROM {0} WITH(updlock, rowlock, holdlock) {1}", table_name, condition)
                    : String.Format("SELECT * FROM {0} {1}", table_name, condition);

                await ExecuteSqlCommand(query);
                return true;
            }
            catch { throw; }
        }
    }
}
