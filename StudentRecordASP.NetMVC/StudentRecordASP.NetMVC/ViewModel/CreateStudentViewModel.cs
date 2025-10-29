using System.ComponentModel.DataAnnotations;

namespace StudentRecordASP.NetMVC.ViewModel
{
    public class CreateStudentViewModel
    {
        private string _name;
        [Required]
        [StringLength(30)]
        public string Name
        {
            get => _name;
            set => _name = value?.Trim();
        }
        [Required]
        [Range(1,100)]
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
