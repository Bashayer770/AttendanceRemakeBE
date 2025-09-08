using Newtonsoft.Json;

namespace AttendanceRemake.Resources.APIs
{
    public class DB2Data
    {
        private IConfiguration _configuration;
        private HttpClient _httpClient = new HttpClient();
        private string _url;
        public DB2Data(IConfiguration configuration)
        {
            _configuration = configuration;
            _url = configuration["APIs:DB2"];
        }

        public async Task<UserData> GetUserByID(string username)
        {
            try
            {
                var result = await _httpClient.GetAsync($"{_url}Employee/{username.ToLower()}");
                if (result.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<UserData>(result.Content.ReadAsStringAsync().Result);
                }
                return null;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
    public class UserData()
    {
        public string? Address { get; set; }
        public int? MobileNo { get; set; }
        public int? EmpNo { get; set; }
        public int? Activity { get; set; }
        public int? Gender { get; set; }
        public int? DeptCode { get; set; }
        public int? DptActivity { get; set; }
        public string? LoginName { get; set; }
        public int? JobCode { get; set; }
        public string? JobTitle { get; set; }
        public int? SectorCode { get; set; }
        public string? ClerkCode { get; set; }
        public string? FirstName { get; set; }
        public string? SecondName { get; set; }
        public string? ThirdName { get; set; }
        public string? FamilyName { get; set; }
        public string? CivilID { get; set; }


        public string? FullName { get; set; }
        public string? ShortName { get; set; }
    }
}
