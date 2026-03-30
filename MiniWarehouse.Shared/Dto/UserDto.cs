namespace MiniWarehouse.Shared.Dto
{
    using System.ComponentModel.DataAnnotations;
    public class UserDto    
    {
        [Required(ErrorMessage = "E-Mail ist ein Pflichtfeld")]
        [EmailAddress(ErrorMessage = "Ungültiges E-Mail-Format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Passwort wird benötigt")]
        [MinLength(6, ErrorMessage = "Passwort muss mindestens 6 Zeichen lang sein")]
        public string Password { get; set; } = string.Empty;

        public int Role { get; set; } = 0; // 0: Regular User, 1: Admin
    }
}