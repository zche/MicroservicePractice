using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace User.Identity.Services
{
    public class UserService : IUserService
    {
        private HttpClient _httpClient;
        private readonly string _uesrServiceUrl = "http://localhost:5678";

        public UserService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<int> CheckOrCreate(string tel)
        {
            var content = new Dictionary<string, string> { { "tel", tel } };
            var formContent = new FormUrlEncodedContent(content);
            var response = await _httpClient.PostAsync(_uesrServiceUrl + "/api/users/check-or-create", formContent);
            if(response.IsSuccessStatusCode)
            {
                var userId = await response.Content.ReadAsStringAsync();
                int.TryParse(userId,out int intUserId);
                return intUserId;
            }
            return 0;
        }
    }
}
