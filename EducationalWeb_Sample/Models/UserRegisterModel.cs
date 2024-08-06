using System.ComponentModel.DataAnnotations;
namespace EducationalWeb_Sample.Models
{
    public class UserRegisterModel : UserModel
    {
        [Required(ErrorMessage = "Retyped Password must not be null")]
        public string ReTypePassword { get; set; }
    }
}
