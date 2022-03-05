using AutoMapper;
using Entities;
using Entities.DataSendObjects;
using Entities.DataTransferObjects;
using Entities.RequestEntitiesDTOS;

namespace CodeMaze_WebAPI.MapperFoder
{
    public class UserProfile:Profile
    {

        public UserProfile()
        {
            CreateMap<Owner, SendOwnerDto>();
            CreateMap<SendOwnerDto, Owner>();
            CreateMap<Owner, OwnerForUpdateDto>();
            CreateMap<Account, AccountDto>().ReverseMap();

            //CreateMap<Owner, OwnerDto>().ForMember(s=>s.Id, opt=>opt.MapFrom(s=>s.OwnerId))
            //    .ForMember(s=>s.OwnerAccount, opt=>opt.MapFrom(s=>s.Accounts))
            //    .ReverseMap();

            CreateMap<Owner, OwnerDto>().ForMember(s=>s.OwnerAccount, opt=> opt.MapFrom(s=>s.Accounts));

            //CreateMap<OwnerDto, Owner>();
            
            //CreateMap<AccountDto, Account>();
        }
    }
}
