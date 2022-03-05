
using Contracts;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class AccountRepository : RepositoryBase<Account>, IAccountRepository
    {
        public AccountRepository(RepositoryContext context) : base(context)
        {
        }

        public IEnumerable<Account> GetAccountByOwner(Guid id)
        {
            return FindByCondition(s => s.OwnerId == id).ToList();
        }
    }
}
