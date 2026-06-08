namespace LecRate.DTOs
{
    public class EvaluationResponseDTO
    {
        public int EvaluationId { get; set; }
        public string TextAnswer { get; set; } = ""; 
        public bool IsArchived { get; set; }
        
        public int LecturerId { get; set; } 
        public string StudentName { get; set; } = "";
        public string QuestionText { get; set; } = "";
        public string LecturerName { get; set; } = "";
        public string SubjectName { get; set; } = "";
    }
}