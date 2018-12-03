using Simply_Gallery.Repositories;
using Simply_Gallery.Services;
using Simply_Gallery.Controllers;
using System.Web.Mvc;
using Unity;
using Unity.AspNet.Mvc;
using Unity.Injection;
using Simply_Gallery.Interfaces;
using Simply_Gallery.Models.Gallery;

namespace Simply_Gallery
{
    public static class UnityConfig
    {
        public static IUnityContainer Initialise()
        {
            var container = BuildUnityContainer();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            return container;
        }

        private static IUnityContainer BuildUnityContainer()
        {
            var container = new UnityContainer();

            //container.RegisterType<IPhotoRepository, PhotoRepository>();
            //container.RegisterType<IPhotoService, PhotoService>();
            //container.RegisterType<IAlbumRepository, AlbumRepository>();
            //container.RegisterType<IAlbumService, AlbumService>();

            container.RegisterType<IRepository<Photo>, PhotoRepository>();
            container.RegisterType<IPhotoService, PhotoService>();
            container.RegisterType<IRepository<Album>, AlbumRepository>();
            container.RegisterType<IAlbumService, AlbumService>();

            RegisterTypes(container);
            return container;
        }

        public static void RegisterTypes(IUnityContainer container)
        {
            container.RegisterType<HomeController>(new InjectionConstructor());
            container.RegisterType<AdminController>(new InjectionConstructor());
        }
    }
}