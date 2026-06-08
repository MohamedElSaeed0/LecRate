using System.ComponentModel.DataAnnotations;

namespace ProfRate.Entities
{
    
    public class Subject
    {
        public int SubjectId { get; set; }
        
        [MaxLength(50)]
        public string SubjectName { get; set; } = string.Empty;

        
        [System.Text.Json.Serialization.JsonIgnore]
        public List<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
        [System.Text.Json.Serialization.JsonIgnore]
        public List<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
        [System.Text.Json.Serialization.JsonIgnore]
        public List<LecturerSubject> LecturerSubjects { get; set; } = new List<LecturerSubject>();
    }
}