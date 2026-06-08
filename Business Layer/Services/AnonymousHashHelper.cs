namespace LecRate.Services
{
    public static class AnonymousHashHelper
    {
        public static string Generate(string secret, int studentId, int lecturerId, int subjectId, int questionId)
        {
            var raw = $"{studentId}|{lecturerId}|{subjectId}|{questionId}|{secret}";
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            var bytes = System.Text.Encoding.UTF8.GetBytes(raw);
            return Convert.ToHexString(sha256.ComputeHash(bytes)).ToLowerInvariant();
        }
    }
}
