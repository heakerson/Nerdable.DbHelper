using AutoMapper;
using Nerdable.DbHelper.Models.EntityProperties;
using Nerdable.DbHelper.Models.Response;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Nerdable.DbHelper.Services
{
    public class DbHelper : IDbHelper
    {
        private readonly DbContext _dbcontext;
        private readonly IMapper _mapper;

        public DbHelper(DbContext dbcontext, IMapper mapper)
        {
            _dbcontext = dbcontext;
            _mapper = mapper;
        }

        public Response<TEntity> AddObject<TCreateModel, TEntity>(TCreateModel createModel) where TEntity : class, new()
        {
            var mappingResponse = MapToNewObject<TCreateModel, TEntity>(createModel);

            if (mappingResponse.Success)
            {
                return AddObject(mappingResponse.Data);
            }
            else
            {
                return mappingResponse;
            }
        }

        public Response<TEntity> AddObject<TEntity>(TEntity entity) where TEntity : class
        {
            try
            {
                var updated = _dbcontext.Add(entity);
                var saved = _dbcontext.SaveChanges();

                return Response<TEntity>.BuildResponse(entity);
            }
            catch
            {
                return Response<TEntity>.BuildResponse(null, false, ReturnCode.DatabaseAddFailure, $"Failed to update the database with {entity}");
            }
        }

        public Response<int> RemoveEntity<TEntity>(int id) where TEntity : class
        {
            var entityResponse = GetEntity<TEntity>(id);

            if (entityResponse.Success)
            {
                try
                {
                    var removed = _dbcontext.Remove(entityResponse.Data);
                    var saved = _dbcontext.SaveChanges();

                    if (saved > 0)
                    {
                        return Response<int>.BuildResponse(saved);
                    }
                    else
                    {
                        return Response<int>.BuildResponse(0, false, ReturnCode.DatabaseRemoveFailure, "No changes were made in the database");
                    }
                }
                catch (Exception e)
                {
                    return Response<int>.BuildResponse(0, false, ReturnCode.DatabaseRemoveFailure, $"Exception occrured when removing {typeof(TEntity)} with id {id} from the database. Exception: {e.Message} {e.StackTrace}");
                }
            }
            else
            {
                return Response<int>.BuildResponse(0, false, entityResponse.ReturnCode, entityResponse.ReturnMessage);
            }
        }

        public Response<int> RemoveEntitiesByQuery<TEntity>(IQueryable<TEntity> query) where TEntity : class, new()
        {
            var entityResponse = GetEntitiesByQuery(query);

            if (entityResponse.Success)
            {
                try
                {
                    _dbcontext.RemoveRange(query);
                    var saved = _dbcontext.SaveChanges();

                    if (saved > 0)
                    {
                        return Response<int>.BuildResponse(saved);
                    }
                    else
                    {
                        return Response<int>.BuildResponse(0, false, ReturnCode.DatabaseRemoveFailure, "No changes were made to the database.");
                    }
                }
                catch (Exception e)
                {
                    return Response<int>.BuildResponse(0, false, ReturnCode.DatabaseRemoveFailure, $"Exception occrured when removing {typeof(TEntity)} with query {query} from the database. Exception: {e.Message} {e.StackTrace}");
                }
            }
            else
            {
                return Response<int>.BuildResponse(0, false, entityResponse.ReturnCode, entityResponse.ReturnMessage);
            }
        }

        public Response<TEntity> UpdateObject<TUpdateModel, TEntity>(TUpdateModel updateModel, int entityId) where TEntity : class, new()
        {
            var entityResponse = GetEntity<TEntity>(entityId);

            if (entityResponse.Success)
            {
                var mappingResult = MapToExistingObject(updateModel, entityResponse.Data);

                if (mappingResult.Success)
                {
                    return SaveObject(mappingResult.Data);
                }
                else
                {
                    return mappingResult;
                }
            }
            else
            {
                return entityResponse;
            }
        }

        public Response<TEntity> UpdateObject<TEntity>(int id, Func<TEntity, Response<TEntity>> updateEntity) where TEntity : class
        {
            var entityResponse = GetEntity<TEntity>(id);

            if (entityResponse.Success)
            {
                return UpdateObject(entityResponse.Data, updateEntity);
            }
            else
            {
                return entityResponse;
            }
        }

        public Response<TEntity> UpdateObject<TEntity>(TEntity entity, Func<TEntity, Response<TEntity>> updateEntity) where TEntity : class
        {
            var updateResponse = updateEntity(entity);

            if (updateResponse.Success)
            {
                return SaveObject(updateResponse.Data);
            }
            else
            {
                return updateResponse;
            }
        }

        public Response<TEntity> UpdateObject<TUpdateType, TEntity>(IQueryable<TEntity> query, TUpdateType updateModel) where TEntity : class, new()
        {
            var entityResponse = GetEntityByQuery(query);

            if (entityResponse.Success)
            {
                var mappingResult = MapToExistingObject(updateModel, entityResponse.Data);

                if (mappingResult.Success)
                {
                    return SaveObject(mappingResult.Data);
                }
                else
                {
                    return mappingResult;
                }
            }
            else
            {
                return entityResponse;
            }

        }

        public Response<TEntity> SaveObject<TEntity>(TEntity entity) where TEntity : class
        {
            try
            {
                var updated = _dbcontext.Update(entity);
                var saved = _dbcontext.SaveChanges();

                return Response<TEntity>.BuildResponse(entity);
            }
            catch (Exception e)
            {
                return Response<TEntity>.BuildResponse(entity, false, ReturnCode.DatabaseUpdateFailure, $"Failed to update the database with {entity}. Exception: {e.Message} {e.StackTrace}");
            }
        }

        public Response<TOutput> GetObject<TEntity, TOutput>(int id) where TEntity : class where TOutput : new()
        {
            var entityResponse = GetEntity<TEntity>(id);

            if (entityResponse.Success)
            {
                var mappingResult = MapToNewObject<TEntity, TOutput>(entityResponse.Data);

                if (mappingResult.Success)
                {
                    return Response<TOutput>.BuildResponse(mappingResult.Data);
                }
                else
                {
                    return mappingResult;
                }
            }
            else
            {
                return Response<TOutput>.BuildResponse(new TOutput(), false, entityResponse.ReturnCode, entityResponse.ReturnMessage);
            }
        }

        public Response<TOutput> GetObjectByQuery<TEntity, TOutput>(IQueryable<TEntity> query) where TOutput : new() where TEntity : class,new()
        {
            var getEntityResponse = GetEntityByQuery(query);

            if (getEntityResponse.Success)
            {
                var mappingResult = MapToNewObject<TEntity, TOutput>(getEntityResponse.Data);

                if (mappingResult.Success)
                {
                    return Response<TOutput>.BuildResponse(mappingResult.Data);
                }
                else
                {
                    return mappingResult;
                }
            }
            else
            {
                return Response<TOutput>.BuildResponse(new TOutput(), false, getEntityResponse.ReturnCode, getEntityResponse.ReturnMessage);
            }
        }

        public Response<Collection<TOutput>> GetObjectsByQuery<TEntity, TOutput>(IQueryable<TEntity> query) where TOutput : new() where TEntity : class, new()
        {
            var dbSetResponse = GetDbSet<TEntity>();

            if (dbSetResponse.Success)
            {
                var dbSet = dbSetResponse.Data;

                var getEntityResponse = GetEntitiesByQuery(query);

                if (getEntityResponse.Success)
                {
                    var mappingResult = MapToNewObject<ICollection<TEntity>, Collection<TOutput>>(getEntityResponse.Data);

                    if (mappingResult.Success)
                    {
                        return Response<Collection<TOutput>>.BuildResponse(mappingResult.Data);
                    }
                    else
                    {
                        return mappingResult;
                    }
                }
                else
                {
                    return Response<Collection<TOutput>>.BuildResponse(null, false, getEntityResponse.ReturnCode, getEntityResponse.ReturnMessage);
                }
            }
            else
            {
                return Response<Collection<TOutput>>.BuildResponse(null, false, dbSetResponse.ReturnCode, dbSetResponse.ReturnMessage);
            }
        }

        public Response<TEntity> GetEntity<TEntity>(int id) where TEntity : class
        {
            var dbSetResponse = GetDbSet<TEntity>();

            if (dbSetResponse.Success)
            {
                var entity = dbSetResponse.Data.Find(id);

                if (entity != null)
                {
                    //_dbcontext.Attach(entity);
                    return Response<TEntity>.BuildResponse(entity);
                }
                else
                {
                    return Response<TEntity>.BuildResponse(null, false, ReturnCode.DoesNotExist, $"Entity of type {typeof(TEntity)} with id {id} does not exist");
                }
            }
            else
            {
                return Response<TEntity>.BuildResponse(null, false, dbSetResponse.ReturnCode, dbSetResponse.ReturnMessage);
            }
        }

        public Response<TEntity> GetEntityByQuery<TEntity>(IQueryable<TEntity> query) where TEntity : class,new()
        {
            try
            {
                var entity = query.First();

                if (entity != null)
                {
                    //_dbcontext.Attach(entity);
                    return Response<TEntity>.BuildResponse(entity);
                }
                else
                {
                    return Response<TEntity>.BuildResponse(new TEntity(), false, ReturnCode.Fail, $"Failed getting {typeof(TEntity)} using query {query} from the database");
                }
            }
            catch
            {
                return Response<TEntity>.BuildResponse(new TEntity(), false, ReturnCode.Fail, $"Failed getting {typeof(TEntity)} using query {query} from the database");
            }
        }

        public Response<ICollection<TEntity>> GetEntitiesByQuery<TEntity>(IQueryable<TEntity> query) where TEntity : class, new()
        {
            try
            {
                var collection = query.ToList();

                if (collection.Any())
                {
                    //_dbcontext.Attach(collection);
                    return Response<ICollection<TEntity>>.BuildResponse(collection);
                }
                else
                {
                    return Response<ICollection<TEntity>>.BuildResponse(null, false, ReturnCode.NoEntitiesMatchQuery, $"No entities of type {typeof(TEntity)} matched the query {query}");
                }
            }
            catch(Exception e)
            {
                return Response<ICollection<TEntity>>.BuildResponse(null, false, ReturnCode.Fail, $"Failed retrieving data of type {typeof(TEntity)} from the database. Exception: {e}");
            }
        }

        public Response<TOutput> MapToNewObject<TInput, TOutput>(TInput source) where TOutput : new()
        {
            try
            {
                var mapped = _mapper.Map<TOutput>(source);

                if (mapped != null)
                {
                    return Response<TOutput>.BuildResponse(mapped);
                }
                else
                {
                    return Response<TOutput>.BuildResponse(new TOutput(), false, ReturnCode.MappingFailure, $"Automapper failed to map object of type {typeof(TInput)} to object of type {typeof(TOutput)}");
                }
            }
            catch (Exception e)
            {
                return Response<TOutput>.BuildResponse(new TOutput(), false, ReturnCode.MappingFailure, $"Automapper threw exception when trying to map object of type {typeof(TInput)} to object of type {typeof(TOutput)}. Exception: {e.StackTrace}");
            }
        }

        public Response<ICollection<TOutput>> MapToNewObjects<TInput, TOutput>(ICollection<TInput> source) where TOutput : new()
        {
            try
            {
                var mapped = _mapper.Map<ICollection<TOutput>>(source);

                if (mapped != null)
                {
                    return Response<ICollection<TOutput>>.BuildResponse(mapped);
                }
                else
                {
                    return Response<ICollection<TOutput>>.BuildResponse(new Collection<TOutput>(), false, ReturnCode.MappingFailure, $"Automapper failed to map object of type {typeof(TInput)} to object of type {typeof(TOutput)}");
                }
            }
            catch (Exception e)
            {
                return Response<ICollection<TOutput>>.BuildResponse(new Collection<TOutput>(), false, ReturnCode.MappingFailure, $"Automapper threw exception when trying to map object of type {typeof(TInput)} to object of type {typeof(TOutput)}. Exception: {e.StackTrace}");
            }
        }

        public Response<TOutput> MapToExistingObject<TInput, TOutput>(TInput source, TOutput destination) where TOutput : new()
        {
            try
            {
                var mapped = _mapper.Map(source, destination);

                if (mapped != null)
                {
                    return Response<TOutput>.BuildResponse(mapped);
                }
                else
                {
                    return Response<TOutput>.BuildResponse(new TOutput(), false, ReturnCode.MappingFailure, $"Automapper failed to map object of type {typeof(TInput)} to object of type {typeof(TOutput)}");
                }
            }
            catch (Exception e)
            {
                return Response<TOutput>.BuildResponse(new TOutput(), false, ReturnCode.MappingFailure, $"Automapper threw exception when trying to map object of type {typeof(TInput)} to object of type {typeof(TOutput)}. Exception: {e.StackTrace}");
            }

        }

        public Response<DbSet<TEntity>> GetDbSet<TEntity>() where TEntity : class
        {
            try
            {
                var dbSet = _dbcontext.Set<TEntity>();

                //to catch if exception occurred 
                var local = dbSet.Local;

                return Response<DbSet<TEntity>>.BuildResponse(dbSet);
            }
            catch
            {
                return Response<DbSet<TEntity>>.BuildResponse(null, false, ReturnCode.DbSetDoesNotExist, $"No dbSet of type {typeof(TEntity)} exists in the current dbContext.");
            }


        }

        public Response<IList<string>> GetPrimaryKeyPropNames<IEntityType>()
        {
            var entityType = _dbcontext.Model.FindEntityType(typeof(IEntityType));

            if (entityType != null)
            {
                var propNames = entityType.FindPrimaryKey().Properties.Select(p => p.Name).ToList();
                return Response<IList<string>>.BuildResponse(propNames);
            }
            else
            {
                return Response<IList<string>>.BuildResponse(null, false, ReturnCode.DbSetDoesNotExist, $"The current dbContext does not contain an entity type of {typeof(IEntityType)}");
            }
        }

        public Response<IList<EntityProperty<TEntity>>> GetPrimaryKeys<TEntity>(TEntity entity)
        {
            var propNameResponse = GetPrimaryKeyPropNames<TEntity>();

            if (propNameResponse.Success)
            {
                IList<EntityProperty<TEntity>> keys = new List<EntityProperty<TEntity>>();

                foreach (string propName in propNameResponse.Data)
                {
                    var value = entity.GetType().GetProperty(propName).GetValue(entity, null);
                    EntityProperty<TEntity> primaryKey = new EntityProperty<TEntity>(propName, value);

                    keys.Add(primaryKey);
                }

                var primaryKeyValue = entity.GetType().GetProperty(propNameResponse.Data[0]).GetValue(entity, null);

                return Response<IList<EntityProperty<TEntity>>>.BuildResponse(keys);
            }
            else
            {
                return Response<IList<EntityProperty<TEntity>>>.BuildResponse(null, false, propNameResponse.ReturnCode, propNameResponse.ReturnMessage);
            }
        }

        public Response<bool> UntrackEntity<TEntity>(TEntity entity) where TEntity : class
        {
            var success = _dbcontext.Entry(entity).State == EntityState.Detached;

            if (success)
            {
                return Response<bool>.BuildResponse(success);
            }

            return Response<bool>.BuildResponse(success, success, ReturnCode.InvalidInput, $"Entity {entity} was could not be detatched successfully. ");
        }
    }
}
