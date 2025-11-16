using AutoMapper;
using UserService.Data.Models;
using UserService.DTOs;

namespace UserService.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Map User > UserDto
            CreateMap<User, UserDto>().ReverseMap();
        }

    }
}
