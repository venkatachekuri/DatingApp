using System.ComponentModel.DataAnnotations;

namespace DatingAppAPI.Dtos
{
    public class UserForRegisterDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [StringLength(8,MinimumLength=4,ErrorMessage="You must enter the password between 4 to 8 characters!!")]
        public string Password { get; set; }
    }
}