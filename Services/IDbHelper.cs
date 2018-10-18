using Nerdable.DbHelper.Models.EntityProperties;
using Nerdable.DbHelper.Models.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Nerdable.DbHelper.Services
{
    public interface IDbHelper
    {
        Response<TEntity> AddObject<TCreateModel, TEntity>(TCreateModel createModel) where TEntity : class, new();
        Response<TEntity> AddObject<TEntity>(TEntity entity) where TEntity : class;

        Response<TEntity> UpdateObject<TUpdateModel, TEntity>(TUpdateModel updateModel, int entityId) where TEntity : class, new();
        Response<TEntity> UpdateObject<TEntity>(int id, Func<TEntity, Response<TEntity>> updateEntity) where TEntity : class;
        Response<TEntity> UpdateObject<TEntity>(TEntity entity, Func<TEntity, Response<TEntity>> updateEntity) where TEntity : class;
        Response<TEntity> UpdateObject<TUpdateType, TEntity>(IQueryable<TEntity> query, TUpdateType updateModel) where TEntity : class, new();

        Response<TEntity> SaveObject<TEntity>(TEntity entity) where TEntity : class;

        Response<TOutput> GetObject<TEntity, TOutput>(int id) where TEntity : class where TOutput : new();
        Response<TOutput> GetObjectByQuery<TEntity, TOutput>(IQueryable<TEntity> query) where TOutput : new() where TEntity : class,new();
        Response<Collection<TOutput>> GetObjectsByQuery<TEntity, TOutput>(IQueryable<TEntity> query) where TOutput : new() where TEntity : class, new();


        Response<TEntity> GetEntity<TEntity>(int id) where TEntity : class;
        Response<TEntity> GetEntityByQuery<TEntity>(IQueryable<TEntity> query) where TEntity : class,new();
        Response<ICollection<TEntity>> GetEntitiesByQuery<TEntity>(IQueryable<TEntity> query) where TEntity : class, new();


        Response<int> RemoveEntity<TEntity>(int id) where TEntity : class;
        Response<int> RemoveEntitiesByQuery<TEntity>(IQueryable<TEntity> query) where TEntity : class, new();


        Response<TOutput> MapToNewObject<TInput, TOutput>(TInput toMap) where TOutput : new();
        Response<TOutput> MapToExistingObject<TInput, TOutput>(TInput source, TOutput destination) where TOutput : new();


        Response<DbSet<TEntity>> GetDbSet<TEntity>() where TEntity : class;

        
        Response<IList<string>> GetPrimaryKeyPropNames<TEntity>();
        Response<IList<EntityProperty<TEntity>>> GetPrimaryKeys<TEntity>(TEntity entity);

        Response<bool> UntrackEntity<TEntity>(TEntity entity) where TEntity : class;
    }
}
