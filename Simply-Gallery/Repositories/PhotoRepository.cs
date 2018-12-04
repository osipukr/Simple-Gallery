using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Simply_Gallery.Interfaces;
using Simply_Gallery.Models;
using Simply_Gallery.Models.Gallery;

namespace Simply_Gallery.Repositories
{
    public class PhotoRepository : IRepository<Photo>
    {
        public async Task<IQueryable<Photo>> GetAllAsync(Expression<Func<Photo, bool>> predicate)
        {
            var result = new List<Photo>();

            using (var applicationContext = new ApplicationContext())
            {
                result = await applicationContext.Photos.Where(predicate).ToListAsync();
            }

            return result.AsQueryable();
        }

        public async Task<Photo> GetAsync(Expression<Func<Photo, bool>> predicate)
        {
            Photo result = null;

            using (var applicationContext = new ApplicationContext())
            {
                result = await applicationContext.Photos.FirstOrDefaultAsync(predicate);
            }

            return result;
        }

        public async Task<Photo> AddAsync(Photo item)
        {
            Photo result = null;

            using (var applicationContext = new ApplicationContext())
            {
                result = applicationContext.Photos.Add(item);
                await applicationContext.SaveChangesAsync();
            }

            return result;
        }

        public async Task<Photo> UpdateAsync(Photo item)
        {
            using (var applicationContext = new ApplicationContext())
            {
                applicationContext.Entry(item).State = EntityState.Modified;

                await applicationContext.SaveChangesAsync();
            }

            return item;
        }

        public async Task DeleteAsync(Expression<Func<Photo, bool>> predicate)
        {
            using (var applicationContext = new ApplicationContext())
            {
                var album = await applicationContext.Photos.FirstOrDefaultAsync(predicate);

                applicationContext.Entry(album).State = EntityState.Deleted;

                await applicationContext.SaveChangesAsync();
            }
        }

        public async Task<bool> IsFindAsync(Expression<Func<Photo, bool>> predicate)
        {
            bool result;

            using (var applicationContext = new ApplicationContext())
            {
                result = await applicationContext.Photos.AnyAsync(predicate);
            }

            return result;
        }
    }
}