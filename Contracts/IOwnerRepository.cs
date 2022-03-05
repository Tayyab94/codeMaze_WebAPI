
using Entities;
using Entities.Helpes;
using Entities.RequestEntitiesDTOS;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IOwnerRepository:IRepositoryBase<Owner>
    {
        Task<IEnumerable<Owner>>GetAllOwners();

        //IEnumerable<Owner> GetOwners(OwnerParameters ownerParameters);

        Task<PagedList<Owner>> GetOwners1(OwnerParameters ownerParameters);
        
        
        Task<PagedList<ExpandoObject>> GetOwners(OwnerParameters ownerParameters);
      
        ExpandoObject GetOwnerById(Guid ownerId, string fields);
        Task<Owner> GetOwnerById(Guid id);

        Task<Owner> GetOwnerWithDetail(Guid id);

        void CreateOwner(Owner owner);

        void UpdateOwner(Owner owner);
        void DeleteOwner(Owner owner);
    }
}
