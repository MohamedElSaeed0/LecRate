using Microsoft.EntityFrameworkCore;
using LecRate.Entities;

namespace LecRate.Data
{
    
    public class AppDbContext : DbContext
    {
        
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        
        public DbSet<Member> Members { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<StudentSubject> StudentSubjects { get; set; }
        public DbSet<LecturerSubject> LecturerSubjects { get; set; }
        public DbSet<AppSettings> AppSettings { get; set; } 

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            
            modelBuilder.Entity<Member>()
                .HasIndex(m => m.Username)
                .IsUnique();

            modelBuilder.Entity<Member>()
                .HasIndex(m => m.FirstName);

            modelBuilder.Entity<Member>()
                .HasIndex(m => m.LastName);

            
            modelBuilder.Entity<Admin>()
                .HasOne(a => a.Member)
                .WithOne(m => m.Admin)
                .HasForeignKey<Admin>(a => a.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Student>()
                .HasOne(s => s.Member)
                .WithOne(m => m.Student)
                .HasForeignKey<Student>(s => s.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Lecturer>()
                .HasOne(l => l.Member)
                .WithOne(m => m.Lecturer)
                .HasForeignKey<Lecturer>(l => l.MemberId)
                .OnDelete(DeleteBehavior.Cascade);

            
            modelBuilder.Entity<AppSettings>()
                .HasKey(s => s.SettingId);

            modelBuilder.Entity<AppSettings>()
                .HasOne(s => s.Admin)
                .WithMany()
                .HasForeignKey(s => s.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Admin)
                .WithMany(a => a.Students)
                .HasForeignKey(s => s.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<Lecturer>()
                .HasOne(l => l.Admin)
                .WithMany(a => a.Lecturers)
                .HasForeignKey(l => l.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<Question>()
                .HasOne(q => q.Admin)
                .WithMany(a => a.Questions)
                .HasForeignKey(q => q.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<Evaluation>()
                .HasIndex(e => e.AnonymousHash);

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.Lecturer)
                .WithMany(l => l.Evaluations)
                .HasForeignKey(e => e.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.Question)
                .WithMany(q => q.Evaluations)
                .HasForeignKey(e => e.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Evaluation>()
                .HasOne(e => e.Subject)
                .WithMany(s => s.Evaluations)
                .HasForeignKey(e => e.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<StudentSubject>()
                .HasOne(ss => ss.Student)
                .WithMany(s => s.StudentSubjects)
                .HasForeignKey(ss => ss.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentSubject>()
                .HasOne(ss => ss.Subject)
                .WithMany(s => s.StudentSubjects)
                .HasForeignKey(ss => ss.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentSubject>()
                .HasOne(ss => ss.Lecturer)
                .WithMany()
                .HasForeignKey(ss => ss.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);

            
            modelBuilder.Entity<LecturerSubject>()
                .HasOne(ls => ls.Lecturer)
                .WithMany(l => l.LecturerSubjects)
                .HasForeignKey(ls => ls.LecturerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LecturerSubject>()
                .HasOne(ls => ls.Subject)
                .WithMany(s => s.LecturerSubjects)
                .HasForeignKey(ls => ls.SubjectId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}