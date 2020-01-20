using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using EntityFramework.Metadata.Extensions;
using SATI.Context;

namespace SATI.Repositories
{
    public class BaseRepo
    {
        public ApplicationDbContext _db;

        public BaseRepo()
        {
            _db = Db.GetInstance();
        }

        public T Single<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return _db.Set<T>().FirstOrDefault(predicate);
        }

        public IQueryable<T> Where<T>(Expression<Func<T, bool>> predicate = null) where T : class
        {
            //if null predicate then return all, else filter by predicte
            return predicate == null ? 
                _db.Set<T>().AsNoTracking() : _db.Set<T>().Where(predicate).AsNoTracking();
        }

        public List<T> All<T>() where T : class
        {
            return _db.Set<T>().ToList(); 
        }

        public void CreateRange<T>(IEnumerable<T> entities) where T : class
        {
            if (entities == null && !entities.Any()) return;
            _db.Set<T>().AddRange(entities);
            _db.SaveChanges();
        }

        public T Save<T>(T entity) where T : class
        {
            //get entity's primary key
            object propertyValue = entity.GetType().GetProperty(GetPrimaryKeyPropertyNames(_db, entity.GetType())).GetValue(entity, null);

            //check if entity exists
            if (Int32.Parse(propertyValue.ToString()) > 0)
            {
                _db.Entry(entity).State = EntityState.Modified;
            }
            else
            {
                _db.Set<T>().Add(entity);
            }

            SaveChanges(_db);

            return entity;
        }

        public void Delete<T>(T entity) where T : class
        {
            if (entity == null) return;
            _db.Set<T>().Remove(entity);
            _db.SaveChanges();
        }

        public void SoftDelete<T>(T entity) where T : class 
        {
            Type type = entity.GetType();

            PropertyInfo prop = type.GetProperty("IsDeleted");

            prop?.SetValue(entity, true, null);

            SaveChanges(_db);
        }

        public void Undelete<T>(T entity) where T : class 
        {
            Type type = entity.GetType();

            PropertyInfo prop = type.GetProperty("IsDeleted");

            prop?.SetValue(entity, false, null);

            SaveChanges(_db);
        }

        public void DeleteRange<T>(IEnumerable<T> entities) where T : class
        {
            if (entities == null || !entities.Any()) return;
            _db.Set<T>().RemoveRange(entities);
            _db.SaveChanges();
        }

        private static string GetPrimaryKeyPropertyNames(DbContext db, Type entityType)
        {
            return db.Db(entityType).Pks.Select(x => x.PropertyName).FirstOrDefault();
        }

        public void SaveChanges(ApplicationDbContext context)
        {
            try
            {
                context.SaveChanges();
            }
            catch (DbEntityValidationException ex)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var failure in ex.EntityValidationErrors)
                {
                    sb.AppendFormat("{0} failed validation\n", failure.Entry.Entity.GetType());
                    foreach (var error in failure.ValidationErrors)
                    {
                        sb.AppendFormat("- {0} : {1}", error.PropertyName, error.ErrorMessage);
                        sb.AppendLine();
                    }
                }

                throw new DbEntityValidationException(
                    "Entity Validation Failed - errors follow:\n" +
                    sb.ToString(), ex
                );
            }
        }
    }
}