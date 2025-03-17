using Azure;
using onlineshop.Models;

namespace onlineshop.Repositroy
{
    public interface IUser
    {
        Task<ResponseVM> RegisterAsync(RegisterVM model);
        Task<ResponseVM> LoginAsync(LoginVM model);
        Task<ResponseVM> UpdateAsync(Update_UserVM model);
        List<T_User> UsersList();
        Task<ResponseVM> DeleteAsync(string id);
        Task Logout();
        //Task ChangePass(string userId, ChangePasswordVM changePass);
    }
}
