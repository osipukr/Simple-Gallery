using Simply_Gallery.DAL.Entities;
using System;

namespace Simply_Gallery.DAL.Interfaces
{
    public interface IClientManager : IDisposable
    {
        void Create(ClientProfile profile);
    }
}