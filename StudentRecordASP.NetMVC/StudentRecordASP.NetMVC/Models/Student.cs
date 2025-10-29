namespace StudentRecordASP.NetMVC.Models
{
    public class Student
    {
        public int Id { get; set; }
        public int RolLNumber { get; set; }
        public string Name { get; set; }
        public int Maths { get; set; }
        public int Physics { get; set; }
        public int Chemistry { get; set; }
        public int English { get; set; }
        public int Programming { get; set; }
        public bool IsActive { get; set; }
    }
}
