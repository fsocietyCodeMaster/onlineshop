using AutoMapper;
using onlineshop.Models;

namespace onlineshop.Maping
{
    public class ServicesMapper : Profile
    {
        public ServicesMapper()
        {
            CreateMap<Create_CategoryVM,T_L_Category>();
            CreateMap<T_L_Category,Update_CategoryVM>();
            CreateMap<T_L_Category,GetCategoryVM>();
            CreateMap<Update_CategoryVM,T_L_Category>();
            CreateMap<Create_ProductVM,T_Product>();
            CreateMap<Update_ProductVM,T_Product>();
            CreateMap<T_Basket,BasketViewModel>();
        }
    }
}
