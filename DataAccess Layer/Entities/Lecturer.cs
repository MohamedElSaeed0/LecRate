using System.ComponentModel.DataAnnotations;

namespace ProfRate.Entities
{
    
    public class Lecturer
    {
        public int LecturerId { get; set; }

        
        public int MemberId { get; set; }

        
        public byte? AdminRating { get; set; }

        
        public int AdminId { get; set; }

        
        [System.Text.Json.Serialization.JsonIgnore]
        public Member Member { get; set; } = null!;
        public Admin Admin { get; set; } = null!;
        [System.Text.Json.Serialization.JsonIgnore]
        public List<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
        [System.Text.Json.Serialization.JsonIgnore]
        public List<LecturerSubject> LecturerSubjects { get; set; } = new List<LecturerSubject>();

        
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string FirstName => Member?.FirstName ?? string.Empty;
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string LastName => Member?.LastName ?? string.Empty;
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Username => Member?.Username ?? string.Empty;
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Password => Member?.Password ?? string.Empty;
        [System.ComponentModel.DataAnnotations.Schema.NotMapped]
        public string Gender => Member != null ? (Member.Gender == 0 ? "Male" : "Female") : "Male";
    }
}