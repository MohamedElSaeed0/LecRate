using System.ComponentModel.DataAnnotations;

namespace ProfRate.Entities
{
    
    public class Member
    {
        public int MemberId { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string Username { get; set; } = string.Empty;

        [MaxLength(255)]
        public string Password { get; set; } = string.Empty;

        public byte Gender { get; set; } 

        
        [System.Text.Json.Serialization.JsonIgnore]
        public Admin? Admin { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Student? Student { get; set; }
        [System.Text.Json.Serialization.JsonIgnore]
        public Lecturer? Lecturer { get; set; }
    }
}