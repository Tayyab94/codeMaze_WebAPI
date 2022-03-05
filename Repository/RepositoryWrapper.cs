
using Contracts;
using Entities;
using Entities.Helpes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class RepositoryWrapper : IRepositoryWrapper
    {
        private RepositoryContext _repoContext;
        private IOwnerRepository _ownerRepository;
        private IAccountRepository _accountRepository;


        private ISortHelper<Owner> _ownerSortHelper;

        private IDataSharper<Owner> _ownerDataSharper;

        //private ISortHelper<Account> _accountSortHelper;
        //public RepositoryWrapper(RepositoryContext context)
        //{
        //    this._repoContext= context;
        //}

        public RepositoryWrapper(RepositoryContext repositoryContext,
        ISortHelper<Owner> ownerSortHelper, IDataSharper<Owner> ownerDataSharper)
        {
            _repoContext = repositoryContext;
            _ownerSortHelper = ownerSortHelper;
            _ownerDataSharper = ownerDataSharper;
        }
        public IOwnerRepository OwnerRepository
        {
            get
            {
                if(_ownerRepository==null)
                {
                    _ownerRepository = new OwnerRepository(_repoContext, _ownerSortHelper, _ownerDataSharper);
                }
                return _ownerRepository;
            }
        }


        public IAccountRepository AccountRepository
        {
            get
            {
                if (_accountRepository == null)
                {
                    _accountRepository = new AccountRepository(_repoContext);
                }
                return _accountRepository;
            }
        }

        public async Task Save()
        {
           await _repoContext.SaveChangesAsync();
        }
    }
}
