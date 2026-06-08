using System.ComponentModel.DataAnnotations;

namespace ProfRate.Entities
{
    
    public class Question
    {
        public int QuestionId { get; set; }
        
        [MaxLength(200)]
        public string QuestionText { get; set; } = string.Empty;

        
        public int AdminId { get; set; }

        
        public Admin Admin { get; set; } = null!;
        public List<Evaluation> Evaluations { get; set; } = new List<Evaluation>();
    }
}