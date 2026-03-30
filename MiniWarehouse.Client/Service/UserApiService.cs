using System.Net.Http.Json;
using MiniWarehouse.Client.IService;
using MiniWarehouse.Shared.Dto;
using MiniWarehouse.Shared.Model;

namespace MiniWarehouse.Client.Service
{
    public class UserApiService(HttpClient http) : IUserApiService
    {

        public async Task<List<User>> GetAllUsers() =>
            (await http.GetFromJsonAsync<List<User>>("api/user")) ?? [];
        public async Task<bool> Register(UserDto user)
            => (await http.PostAsJsonAsync("api/user/register", user)).IsSuccessStatusCode;

        public async Task<bool> DeleteAccount(string userId)
            => (await http.DeleteAsync($"api/user/{userId}")).IsSuccessStatusCode;

        public async Task<User> GetUserById(string id) => (await http.GetFromJsonAsync<User>($"api/user/byId/{id}"))!;

        public async Task<User> GetUserByEmail(string email) =>
            (await http.GetFromJsonAsync<User>($"api/user/byMail/{email}"))!;

        public async Task<User> UpdateUser(User user) =>
            (await http.PutAsync($"api/user/{user.Id}", JsonContent.Create(user))).IsSuccessStatusCode
                ? user
                : throw new Exception("Fehler beim Aktualisieren des Benutzers.");
        
        public async Task<bool> DeleteUser(User user) =>
            (await http.DeleteAsync($"api/user/{user.Id}")).IsSuccessStatusCode;
    }
}
