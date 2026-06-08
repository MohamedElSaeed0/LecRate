namespace LecRate.Entities
{
    
    public class StudentSubject
    {
        public int StudentSubjectId { get; set; }

        
        public int StudentId { get; set; }
        public int SubjectId { get; set; }
        public int? LecturerId { get; set; }

        
        public Student Student { get; set; } = null!;
        public Subject Subject { get; set; } = null!;
        public Lecturer? Lecturer { get; set; }
    }
}