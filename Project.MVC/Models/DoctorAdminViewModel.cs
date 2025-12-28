namespace Project.MVC.Models
{
    public class DoctorAdminViewModel
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Specialty { get; set; }
        public bool IsActive { get; set; }
        public bool IsApproved { get; set; }
    }
}
