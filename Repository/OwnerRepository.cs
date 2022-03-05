
using Contracts;
using Entities;
using Entities.Helpes;
using Entities.RequestEntitiesDTOS;
using System.Reflection;
using System.Text;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace Repository
{
    public class OwnerRepository : RepositoryBase<Owner>, IOwnerRepository
    {
        //public OwnerRepository(RepositoryContext context) : base(context)
        //{
        //}

        private ISortHelper<Owner> _sortHelper;
        private readonly IDataSharper<Owner> dataSharper;

        public OwnerRepository(RepositoryContext repositoryContext, ISortHelper<Owner> sortHelper,
            IDataSharper<Owner> dataSharper)
            : base(repositoryContext)
        {
            _sortHelper = sortHelper;
            this.dataSharper = dataSharper;
        }

        public void CreateOwner(Owner owner)
        {
            Create(owner);
        }

        public void UpdateOwner(Owner owner)
        {
            Update(owner);
        }

        public void DeleteOwner(Owner owner)
        {
            Delete(owner);
        }
        public async Task<IEnumerable<Owner>> GetAllOwners()
        {
            return await FindAll().OrderBy(s => s.Name).ToListAsync();
        }

        public async Task<Owner> GetOwnerById(Guid id)
        {
            return await FindByCondition(s => s.OwnerId == id).FirstOrDefaultAsync();
        }

        //public IEnumerable<Owner> GetOwners(OwnerParameters ownerParameters)
        //{
        //    return FindAll().OrderBy(s=>s.Name)
        //        .Skip((ownerParameters.PageNumber -1) * ownerParameters.PageSize).Take(ownerParameters.PageSize).ToList();
        //}


        // this function is using to get the specifiic properties from the query
        public async Task<PagedList<ExpandoObject>> GetOwners(OwnerParameters ownerParameters)
        {
            IQueryable<Owner> owners = FindByCondition(s => ownerParameters.MinYearOfBirth == 0 || (s.DateOfBirth.Year >= ownerParameters.MinYearOfBirth &&
                                    s.DateOfBirth.Year <= ownerParameters.MaxYearOfBirth)).OrderBy(s => s.Name);

            SearchByName(ref owners, ownerParameters.Name);

            //ApplySort(ref owners, ownerParameters.OrderBy);
            var sortedOwners = _sortHelper.ApplySort(owners, ownerParameters.OrderBy);

            // Shaped Data
            var shapedOwners = dataSharper.ShapeData(owners, ownerParameters.Fields);

            return await PagedList<ExpandoObject>.ToPagedList(shapedOwners, ownerParameters.PageNumber, ownerParameters.PageSize);


            // with filter parameters
            //return await PagedList<Owner>.ToPagedList(FindAll().OrderBy(s => s.Name),
            //        ownerParameters.PageNumber,
            //        ownerParameters.PageSize);
        }

        public ExpandoObject GetOwnerById(Guid ownerId, string fields)
        {
            var owner = FindByCondition(owner => owner.OwnerId.Equals(ownerId))
                                    .DefaultIfEmpty(new Owner())
                                    .FirstOrDefault();
            return dataSharper.ShapeData(owner, fields);
        }



        public async Task<PagedList<Owner>> GetOwners1(OwnerParameters ownerParameters)
        {
            IQueryable<Owner> owners = FindByCondition(s => ownerParameters.MinYearOfBirth == 0 || (s.DateOfBirth.Year >= ownerParameters.MinYearOfBirth &&
                                    s.DateOfBirth.Year <= ownerParameters.MaxYearOfBirth)).OrderBy(s => s.Name);

            SearchByName(ref owners, ownerParameters.Name);

            //ApplySort(ref owners, ownerParameters.OrderBy);
            var sortedOwners = _sortHelper.ApplySort(owners, ownerParameters.OrderBy);

            return await PagedList<Owner>.ToPagedList(owners, ownerParameters.PageNumber, ownerParameters.PageSize);

            // with filter parameters
            //return await PagedList<Owner>.ToPagedList(FindAll().OrderBy(s => s.Name),
            //        ownerParameters.PageNumber,
            //        ownerParameters.PageSize);
        }

        private void SearchByName(ref IQueryable<Owner> owners, string ownerName)
        {
            if (!owners.Any() || string.IsNullOrEmpty(ownerName))
            {
                return;
            }

            owners = owners.Where(s => s.Name.ToLower().Contains(ownerName.ToLower()));
        }

        private void ApplySort(ref IQueryable<Owner> owners, string orderByQueryString)
        {
            if (!owners.Any())
                return;
            if (string.IsNullOrWhiteSpace(orderByQueryString))
            {
                owners = owners.OrderBy(x => x.Name);
                return;
            }
            var orderParams = orderByQueryString.Trim().Split(',');
            var propertyInfos = typeof(Owner).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var orderQueryBuilder = new StringBuilder();
            foreach (var param in orderParams)
            {
                if (string.IsNullOrWhiteSpace(param))
                    continue;
                var propertyFromQueryName = param.Split(" ")[0];
                var objectProperty = propertyInfos.FirstOrDefault(pi => pi.Name.Equals(propertyFromQueryName, StringComparison.InvariantCultureIgnoreCase));
                if (objectProperty == null)
                    continue;
                var sortingOrder = param.EndsWith(" desc") ? "descending" : "ascending";
                orderQueryBuilder.Append($"{objectProperty.Name.ToString()} {sortingOrder}, ");
            }
            var orderQuery = orderQueryBuilder.ToString().TrimEnd(',', ' ');
            if (string.IsNullOrWhiteSpace(orderQuery))
            {
                owners = owners.OrderBy(x => x.Name);
                return;
            }
            owners = owners.OrderBy(orderQuery);
        }
        public async Task<Owner> GetOwnerWithDetail(Guid id)
        {
            return await FindByCondition(s => s.OwnerId == id).Include(s => s.Accounts).FirstOrDefaultAsync();
        }

       
    }
}