﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Collections;

namespace Sic.Data.Entity
{
    public class Repository<TEntity> where TEntity : EntityBase
    {
        protected Sic.Data.Entity.DbContext context;
        protected DbSet<TEntity> dbSet;

        public Repository(DbContext context)
        {
            this.context = context;
            this.dbSet = context.Set<TEntity>();
        }

        public void Attach(TEntity entity)
        {
            dbSet.Attach(entity);
        }

        public void Load(TEntity entity, string navigationProperty)
        {
            this.context.Entry<TEntity>(entity).Collection(navigationProperty).Load();
        }

        public IQueryable<TEntity> AsQueryable()
        {
            return dbSet;
        }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<TEntity> query = dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (!string.IsNullOrWhiteSpace(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split
                    (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {

                return query.ToList();
            }
        }

        public virtual TEntity GetByID(object id)
        {
            return dbSet.Find(id);
        }

        public virtual void Insert(TEntity entity)
        {
            if (entity is IAuditable)
            {
                (entity as IAuditable).DateInsert = Runtime.Current.GetCurrentDateTime();
                (entity as IAuditable).UserInsertId = Runtime.Current.UserId;
            }
            dbSet.Add(entity);
        }

        public virtual void Delete(object id)
        {
            TEntity entityToDelete = dbSet.Find(id);
            Delete(entityToDelete);
        }

        public virtual void Delete(TEntity entityToDelete)
        {
            if (context.Entry(entityToDelete).State == EntityState.Detached)
            {
                dbSet.Attach(entityToDelete);
            }
            dbSet.Remove(entityToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            if (entityToUpdate is IAuditable)
            {
                (entityToUpdate as IAuditable).DateUpdate = Runtime.Current.GetCurrentDateTime();
                (entityToUpdate as IAuditable).UserUpdateId = Runtime.Current.UserId;
            }

            dbSet.Attach(entityToUpdate);

            context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        public virtual IEnumerable<TEntity> GetWithRawSql(string query, params object[] parameters)
        {
            return dbSet.SqlQuery(query, parameters).ToList();
        }

        public void UpdateModel(object source, TEntity target, string[] includeProperties = null, string[] excludeProperties = null)
        {
            source.CopyTo(target, includeProperties: includeProperties, excludeProperties: excludeProperties);
            Update(target);
        }

        public void UpdateModel(IEnumerable<IIdentifiable> source, IEnumerable<TEntity> target,
            string[] includeProperties = null, string[] excludeProperties = null)
        {
            HashSet<int> keys = new HashSet<int>(source.Select(p => p.Key));

            //Update Insert Identification
            List<TEntity> insertList = new List<TEntity>();
            List<TEntity> updateList = new List<TEntity>();
            foreach (IIdentifiable entitySource in source)
            {
                TEntity entityUpdate = (TEntity)entitySource.CopyTo(target, includeProperties: includeProperties);

                if (entityUpdate != null)
                    updateList.Add(entityUpdate);
                else
                {
                    entityUpdate = (TEntity)entitySource.ConvertTo(typeof(TEntity));
                    insertList.Add(entityUpdate);
                }
            }

            //Insert
            if (insertList.Any())
            {
                IList targetList = target as IList;
                if (targetList != null)
                {
                    foreach (TEntity entityInsert in insertList)
                    {
                        Insert(entityInsert);

                        if (!targetList.Contains(entityInsert))
                            targetList.Add(entityInsert);
                    }
                }
            }

            //Update
            if (updateList.Any())
            {
                IList targetList = target as IList;
                if (targetList != null)
                {
                    foreach (TEntity entityUpdate in updateList)
                        Update(entityUpdate);
                }
            }

            //Delete
            List<TEntity> deleteList = new List<TEntity>();
            deleteList.AddRange(target.Where(p => !keys.Contains(p.Key)));

            foreach (TEntity entityDelete in deleteList)
            {
                Delete(entityDelete);
                IList targetList = source as IList;
                if (targetList != null && targetList.Contains(entityDelete))
                    targetList.Remove(entityDelete);
            }
        }
    }
}