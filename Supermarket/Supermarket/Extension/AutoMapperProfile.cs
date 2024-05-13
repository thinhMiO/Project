using AutoMapper;
using Supermarket.DataModels;
using Supermarket.Models;

namespace Supermarket.Extension
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<RegisterVM, Customer>();
                //.ForMember(c => c.CustomerName, option => option.MapFrom(RegisterDM => RegisterDM.CustomerName)).ReverseMap();
        }
    }
}
