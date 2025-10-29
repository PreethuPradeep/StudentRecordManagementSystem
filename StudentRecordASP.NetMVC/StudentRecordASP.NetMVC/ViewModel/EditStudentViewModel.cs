using System.ComponentModel.DataAnnotations;

namespace StudentRecordASP.NetMVC.ViewModel
{
    public class EditStudentViewModel
    {
        //hidden
        [Required]
        public int RollNumber { get; set; }
        //readonly
        [Display(Name = "Student Name")]
        public string Name { get; set; }
        [Required]
        [Range(1, 100)]
        public int Maths { get; set; }

        [Required]
        [Range(1, 100)]
        public int Physics { get; set; }

        [Required]
        [Range(1, 100)]
        public int Chemistry { get; set; }

        [Required]
        [Range(1, 100)]
        public int English { get; set; }

        [Required]
        [Range(1, 100)]
        public int Programming { get; set; }
    }
}
