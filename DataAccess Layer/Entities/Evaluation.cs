using System.ComponentModel.DataAnnotations;

namespace LecRate.Entities
{
    
    public class Evaluation
    {
        public int EvaluationId { get; set; }
        
        [MaxLength(400)]
        public string TextAnswer { get; set; } = string.Empty;  
        
        public bool IsArchived { get; set; } = false;

        [MaxLength(255)]
        public string AnonymousHash { get; set; } = string.Empty;

        
        public int QuestionId { get; set; }
        public int LecturerId { get; set; }
        public int SubjectId { get; set; }

        
        public Question Question { get; set; } = null!;
        public Lecturer Lecturer { get; set; } = null!;
        public Subject Subject { get; set; } = null!;
    }
}