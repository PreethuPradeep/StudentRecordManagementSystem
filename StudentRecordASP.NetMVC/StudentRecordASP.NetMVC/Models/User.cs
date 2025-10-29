namespace StudentRecordASP.NetMVC.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string UserRole { get; set; }
        public bool IsDefaultPassword { get; set; }
        public int? StudentId { get; set; }
    }
}
