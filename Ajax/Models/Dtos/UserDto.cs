namespace Ajax.Models.Dtos
{
    public class UserDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int? Age { get; set; } = 26;
        public string? Password { get; set; }
        public IFormFile? Avatar { get; set; }  //input 的name
    }
}
