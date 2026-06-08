namespace ProfRate.Entities
{
    
    public class AppSettings
    {
        public int SettingId { get; set; }
        public bool IsEvaluationOpen { get; set; } = true; 
        
        public DateTime? EvaluationOpenDate { get; set; }
        public DateTime? EvaluationCloseDate { get; set; }
        
        public int AdminId { get; set; }
        public Admin Admin { get; set; } = null!;
    }
}