﻿namespace Ajax.Models.ViewModels
{
    public class UserDto
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public int? Age { get; set; } = 26;
        public IFormFile? Avatar { get; set; }  //input 的name
    }
}
