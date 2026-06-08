using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProfRate.Entities
{
    
    public class Student
    {
        public int StudentId { get; set; }

        
        public int MemberId { get; set; }

        
        public int AdminId { get; set; }

        
        [System.Text.Json.Serialization.JsonIgnore]
        public Member Member { get; set; } = null!;
        public Admin Admin { get; set; } = null!;
        [System.Text.Json.Serialization.JsonIgnore]
        public List<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();

        
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