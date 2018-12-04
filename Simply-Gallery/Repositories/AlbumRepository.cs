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
    public class AlbumRepository : IRepository<Album>
    {
        private readonly IRepository<Photo> _photoRepository;

        public AlbumRepository()
        {
        }

        public AlbumRepository(IRepository<Photo> photoRepository)
        {
            _photoRepository = photoRepository;
        }

        public async Task<IQueryable<Album>> GetAllAsync(Expression<Func<Album, bool>> predicate)
        {
            var result = new List<Album>();

            using (var applicationContext = new ApplicationContext())
            {
                result = await applicationContext.Albums.Where(predicate).ToListAsync();

                foreach (var album in result)
                {
                    album.Photos = await _photoRepository.GetAllAsync(x => x.AlbumId == album.Id);

                    foreach (var photo in album.Photos)
                    {
                        photo.Album = album;
                    }
                }
            }

            return result.AsQueryable();
        }

        public async Task<Album> GetAsync(Expression<Func<Album, bool>> predicate)
        {
            Album result = null;

            using (var applicationContext = new ApplicationContext())
            {
                result = await applicationContext.Albums.FirstOrDefaultAsync(predicate);

                if(result != null)
                {
                    result.Photos = await _photoRepository.GetAllAsync(x => x.AlbumId == result.Id);

                    foreach (var photo in result.Photos)
                    {
                        photo.Album = result;
                    }
                }
            }

            return result;
        }

        public async Task<Album> AddAsync(Album item)
        {
            Album result = null;

            using (var applicationContext = new ApplicationContext())
            {
                result = applicationContext.Albums.Add(item);
                await applicationContext.SaveChangesAsync();
            }

            return result;
        }

        public async Task<Album> UpdateAsync(Album item)
        {
            using (var applicationContext = new ApplicationContext())
            {
                applicationContext.Entry(item).State = EntityState.Modified;

                await applicationContext.SaveChangesAsync();
            }

            return item;
        }

        public async Task DeleteAsync(Expression<Func<Album, bool>> predicate)
        {
            using (var applicationContext = new ApplicationContext())
            {
                var album = await applicationContext.Albums.FirstOrDefaultAsync(predicate);

                applicationContext.Entry(album).State = EntityState.Deleted;

                await applicationContext.SaveChangesAsync();
            }
        }

        public async Task<bool> IsFindAsync(Expression<Func<Album, bool>> predicate)
        {
            bool result;

            using (var applicationContext = new ApplicationContext())
            {
                result = await applicationContext.Albums.AnyAsync(predicate);
            }

            return result;
        }
    }
}