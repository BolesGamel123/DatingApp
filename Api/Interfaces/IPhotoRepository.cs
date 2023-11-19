using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Dtos;
using Api.Entities;

namespace Api.Interfaces
{
    public interface IPhotoRepository
    {
        Task<IEnumerable<PhotoForApprovalDto>> GetUnapprovedPhotos();
        Task<photo> GetPhotoById(int id);
         void RemovePhoto(photo photo);
    }
}