using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProfRate.Data;
using ProfRate.Entities;

namespace ProfRate.Controllers
{
    [Route("api/seed")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly AppDbContext _context;
        private const string HASH_SECRET = "EvalProf_AnonymousEval_2026_SecretKey";

        public SeedController(AppDbContext context)
        {
            _context = context;
        }

        private string GenerateAnonymousHash(int studentId, int lecturerId, int subjectId, int questionId)
        {
            var raw = $"{studentId}|{lecturerId}|{subjectId}|{questionId}|{HASH_SECRET}";
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(raw);
                var hashBytes = sha256.ComputeHash(bytes);
                return Convert.ToHexString(hashBytes).ToLowerInvariant();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Seed()
        {
            try
            {
                
                var subjects = new[]
                {
                    new Subject { SubjectName = "هندسة البرمجيات" },
                    new Subject { SubjectName = "قواعد البيانات" },
                    new Subject { SubjectName = "ذكاء اصطناعي" },
                    new Subject { SubjectName = "برمجة الويب" },
                    new Subject { SubjectName = "شبكات الحاسوب" },
                    new Subject { SubjectName = "تحليل خوارزميات" },
                    new Subject { SubjectName = "أمن المعلومات" },
                    new Subject { SubjectName = "نظم التشغيل" }
                };

                foreach (var sub in subjects)
                {
                    if (!await _context.Subjects.AnyAsync(s => s.SubjectName == sub.SubjectName))
                    {
                        _context.Subjects.Add(sub);
                    }
                }
                await _context.SaveChangesAsync();

                
                var dbSubjects = await _context.Subjects.ToListAsync();

                
                var lecturerData = new[]
                {
                    new { First="أحمد", Last="علي", User="ahmed_ali", Pass="password123", Gender=(byte)0, Rating=(byte)85 },
                    new { First="ليلى", Last="محمود", User="layla_m", Pass="password123", Gender=(byte)1, Rating=(byte)92 },
                    new { First="خالد", Last="حسن", User="khaled_h", Pass="password123", Gender=(byte)0, Rating=(byte)74 },
                    new { First="سارة", Last="كمال", User="sara_k", Pass="password123", Gender=(byte)1, Rating=(byte)60 },
                    new { First="منى", Last="سعيد", User="mouna_s", Pass="password123", Gender=(byte)1, Rating=(byte)88 },
                    new { First="عادل", Last="سليمان", User="adel_s", Pass="password123", Gender=(byte)0, Rating=(byte)79 }
                };

                var dbLecturers = new List<Lecturer>();
                foreach (var l in lecturerData)
                {
                    var member = await _context.Members.FirstOrDefaultAsync(m => m.Username == l.User);
                    if (member == null)
                    {
                        member = new Member
                        {
                            FirstName = l.First,
                            LastName = l.Last,
                            Username = l.User,
                            Password = l.Pass,
                            Gender = l.Gender
                        };
                        _context.Members.Add(member);
                        await _context.SaveChangesAsync();
                    }

                    var lecturer = await _context.Lecturers.FirstOrDefaultAsync(lec => lec.MemberId == member.MemberId);
                    if (lecturer == null)
                    {
                        lecturer = new Lecturer
                        {
                            MemberId = member.MemberId,
                            AdminId = 1,
                            AdminRating = l.Rating
                        };
                        _context.Lecturers.Add(lecturer);
                        await _context.SaveChangesAsync();
                    }
                    dbLecturers.Add(lecturer);
                }

                
                var studentData = new[]
                {
                    new { First="عمر", Last="فاروق", User="omar_f", Pass="password123", Gender=(byte)0 },
                    new { First="مريم", Last="يوسف", User="maryam_y", Pass="password123", Gender=(byte)1 },
                    new { First="مصطفى", Last="أمين", User="mostafa_a", Pass="password123", Gender=(byte)0 },
                    new { First="هند", Last="جمال", User="hind_j", Pass="password123", Gender=(byte)1 },
                    new { First="ياسين", Last="طه", User="yassin_t", Pass="password123", Gender=(byte)0 },
                    new { First="زينب", Last="علي", User="zeinab_a", Pass="password123", Gender=(byte)1 },
                    new { First="عبد الرحمن", Last="محمد", User="abdo_m", Pass="password123", Gender=(byte)0 },
                    new { First="فاطمة", Last="أحمد", User="fatma_a", Pass="password123", Gender=(byte)1 },
                    new { First="محمود", Last="سعد", User="mahmoud_s", Pass="password123", Gender=(byte)0 },
                    new { First="نور الهدى", Last="حسن", User="nour_h", Pass="password123", Gender=(byte)1 }
                };

                var dbStudents = new List<Student>();
                foreach (var s in studentData)
                {
                    var member = await _context.Members.FirstOrDefaultAsync(m => m.Username == s.User);
                    if (member == null)
                    {
                        member = new Member
                        {
                            FirstName = s.First,
                            LastName = s.Last,
                            Username = s.User,
                            Password = s.Pass,
                            Gender = s.Gender
                        };
                        _context.Members.Add(member);
                        await _context.SaveChangesAsync();
                    }

                    var student = await _context.Students.FirstOrDefaultAsync(st => st.MemberId == member.MemberId);
                    if (student == null)
                    {
                        student = new Student
                        {
                            MemberId = member.MemberId,
                            AdminId = 1
                        };
                        _context.Students.Add(student);
                        await _context.SaveChangesAsync();
                    }
                    dbStudents.Add(student);
                }

                
                
                var sub0 = dbSubjects.FirstOrDefault(s => s.SubjectName == "هندسة البرمجيات")?.SubjectId ?? 0;
                var sub1 = dbSubjects.FirstOrDefault(s => s.SubjectName == "قواعد البيانات")?.SubjectId ?? 0;
                var sub2 = dbSubjects.FirstOrDefault(s => s.SubjectName == "ذكاء اصطناعي")?.SubjectId ?? 0;
                var sub3 = dbSubjects.FirstOrDefault(s => s.SubjectName == "برمجة الويب")?.SubjectId ?? 0;
                var sub4 = dbSubjects.FirstOrDefault(s => s.SubjectName == "شبكات الحاسوب")?.SubjectId ?? 0;
                var sub5 = dbSubjects.FirstOrDefault(s => s.SubjectName == "تحليل خوارزميات")?.SubjectId ?? 0;
                var sub6 = dbSubjects.FirstOrDefault(s => s.SubjectName == "أمن المعلومات")?.SubjectId ?? 0;
                var sub7 = dbSubjects.FirstOrDefault(s => s.SubjectName == "نظم التشغيل")?.SubjectId ?? 0;

                var lec0 = dbLecturers[0].LecturerId;
                var lec1 = dbLecturers[1].LecturerId;
                var lec2 = dbLecturers[2].LecturerId;
                var lec3 = dbLecturers[3].LecturerId;
                var lec4 = dbLecturers[4].LecturerId;
                var lec5 = dbLecturers[5].LecturerId;

                var lecSubPairs = new[]
                {
                    new { Lec=lec0, Sub=sub0 },
                    new { Lec=lec0, Sub=sub3 },
                    new { Lec=lec1, Sub=sub2 },
                    new { Lec=lec1, Sub=sub5 },
                    new { Lec=lec2, Sub=sub1 },
                    new { Lec=lec2, Sub=sub4 },
                    new { Lec=lec3, Sub=sub4 },
                    new { Lec=lec3, Sub=sub7 },
                    new { Lec=lec4, Sub=sub6 },
                    new { Lec=lec4, Sub=sub0 },
                    new { Lec=lec5, Sub=sub7 },
                    new { Lec=lec5, Sub=sub1 }
                };

                foreach (var pair in lecSubPairs)
                {
                    if (pair.Lec > 0 && pair.Sub > 0 && !await _context.LecturerSubjects.AnyAsync(ls => ls.LecturerId == pair.Lec && ls.SubjectId == pair.Sub))
                    {
                        _context.LecturerSubjects.Add(new LecturerSubject { LecturerId = pair.Lec, SubjectId = pair.Sub });
                    }
                }
                await _context.SaveChangesAsync();

                
                
                var registrationList = new[]
                {
                    new { Sub=sub3, Lec=lec0 }, 
                    new { Sub=sub1, Lec=lec2 }, 
                    new { Sub=sub2, Lec=lec1 }, 
                    new { Sub=sub4, Lec=lec3 }, 
                    new { Sub=sub6, Lec=lec4 }, 
                    new { Sub=sub7, Lec=lec5 }  
                };

                for (int i = 0; i < dbStudents.Count; i++)
                {
                    var student = dbStudents[i];
                    
                    for (int j = 0; j < 3; j++)
                    {
                        var reg = registrationList[(i + j) % registrationList.Length];
                        if (reg.Sub > 0 && reg.Lec > 0 && !await _context.StudentSubjects.AnyAsync(ss => ss.StudentId == student.StudentId && ss.SubjectId == reg.Sub && ss.LecturerId == reg.Lec))
                        {
                            _context.StudentSubjects.Add(new StudentSubject
                            {
                                StudentId = student.StudentId,
                                SubjectId = reg.Sub,
                                LecturerId = reg.Lec
                            });
                        }
                    }
                }
                await _context.SaveChangesAsync();

                
                var evalTexts = new[]
                {
                    "شرح ممتاز ومتميز جداً وطريقة توصيل المعلومة رائعة",
                    "الدكتور متمكن جداً من المادة العلمية ويساعد الطلاب دائماً",
                    "الشرح يحتاج لمزيد من الأمثلة العملية وتطبيق المشاريع",
                    "أسلوب التدريس تقليدي وممل بعض الشيء ونأمل التطوير",
                    "الاختبارات صعبة جداً ولكن الدكتور متعاون في الساعات المكتبية",
                    "ملتزم جداً بالوقت والشرح وافي ومفهوم",
                    "أفضل دكتور مر علي في هذا التخصص، أسلوب رائع وأسئلة واضحة",
                    "طريقة الشرح ممتعة وتشد الانتباه دائمًا وأنصح بالمادة معه",
                    "المحاضرة تفاعلية وممتازة لكن نحتاج مرونة أكثر في تسليم الواجبات",
                    "رائع جداً وحريص على فهم الجميع للمادة العلمية بالتفصيل"
                };

                
                var questions = await _context.Questions.ToListAsync();
                if (questions.Any())
                {
                    
                    var rng = new Random(12345); 
                    var studentSubjects = await _context.StudentSubjects.ToListAsync();

                    foreach (var ss in studentSubjects)
                    {
                        if (ss.LecturerId == null) continue;

                        
                        var questionsToAnswer = questions.OrderBy(q => rng.Next()).Take(rng.Next(2, 5)).ToList();
                        
                        foreach (var q in questionsToAnswer)
                        {
                            var hash = GenerateAnonymousHash(ss.StudentId, ss.LecturerId.Value, ss.SubjectId, q.QuestionId);
                            
                            
                            if (!await _context.Evaluations.AnyAsync(e => e.AnonymousHash == hash && !e.IsArchived))
                            {
                                var randomComment = evalTexts[rng.Next(evalTexts.Length)];
                                _context.Evaluations.Add(new Evaluation
                                {
                                    AnonymousHash = hash,
                                    LecturerId = ss.LecturerId.Value,
                                    SubjectId = ss.SubjectId,
                                    QuestionId = q.QuestionId,
                                    TextAnswer = randomComment,
                                    IsArchived = false
                                });
                            }
                        }
                    }
                    await _context.SaveChangesAsync();
                }

                return Ok(new { success = true, message = "تم إدخال البيانات التجريبية بنجاح!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, error = ex.Message, inner = ex.InnerException?.Message });
            }
        }
    }
}