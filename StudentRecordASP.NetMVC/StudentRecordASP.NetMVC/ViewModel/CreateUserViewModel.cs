using System.ComponentModel.DataAnnotations;

namespace StudentRecordASP.NetMVC.ViewModel
{
    public class CreateUserViewModel :IValidatableObject
    {
        public string Email { get; set; }
        public string RoleName { get; set; }
        public int? RollNumber { get; set; }
        public IEnumerable <ValidationResult> Validate(ValidationContext validationContext)
        {
            if (RoleName == "Student" && !RollNumber.HasValue)
            {
                yield return new ValidationResult(
                    "Student Roll Number is required when role is 'Student'.",
                    new[] { nameof(RollNumber) }
                );
            }
        }
    }
}
