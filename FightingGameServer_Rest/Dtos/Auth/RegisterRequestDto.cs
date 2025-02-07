using System.ComponentModel.DataAnnotations;

namespace FightingGameServer_Rest.Dtos;

public class RegisterRequestDto
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(20, MinimumLength = 5, ErrorMessage = "Username must be between 5 and 20 characters.")]
    public required string LoginId { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d]{8,}$",
        ErrorMessage = "The password must be at least 8 characters long and include numbers and letters.")]
    public required string LoginPassword { get; set; }
}