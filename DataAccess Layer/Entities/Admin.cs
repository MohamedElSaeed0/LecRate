using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfRate.Entities
{
    public class Admin
    {
        public int AdminId { get; set; }

        
        public int MemberId { get; set; }

        public bool IsActive { get; set; } = true;

        
        [System.Text.Json.Serialization.JsonIgnore]
        public Member Member { get; set; } = null!;
        public List<Student> Students { get; set; } = new List<Student>();
        public List<Lecturer> Lecturers { get; set; } = new List<Lecturer>();
        public List<Question> Questions { get; set; } = new List<Question>();

        
        [NotMapped]
        public string FirstName => Member?.FirstName ?? string.Empty;
        [NotMapped]
        public string LastName => Member?.LastName ?? string.Empty;
        [NotMapped]
        public string Username => Member?.Username ?? string.Empty;
        [NotMapped]
        public string Password => Member?.Password ?? string.Empty;
        [NotMapped]
        public string Gender => Member != null ? (Member.Gender == 0 ? "Male" : "Female") : "Male";
    }
}