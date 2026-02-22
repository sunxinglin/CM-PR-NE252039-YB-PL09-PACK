using AsZero.DbContexts;
using FutureTech.Dal.Entities;
using FutureTech.Dal.Repository;
using System;
using System.Collections.Generic;
using System.Text;

namespace AsZero.Core.Services.Repos
{
    public class AsZeroEfRepo<TKey, TEntity> : EfRepository<AsZeroDbContext, TKey, TEntity>
        where TEntity: GenericEntity<TKey>
    {
        public AsZeroEfRepo(AsZeroDbContext dbContext, IServiceProvider sp) : base(dbContext, sp)
        {
        }
    }

}
