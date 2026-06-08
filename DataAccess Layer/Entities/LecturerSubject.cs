namespace LecRate.Entities
{
    
    public class LecturerSubject
    {
        public int LecturerSubjectId { get; set; }

        
        public int LecturerId { get; set; }
        public int SubjectId { get; set; }

        
        public Lecturer Lecturer { get; set; } = null!;
        public Subject Subject { get; set; } = null!;
    }
}