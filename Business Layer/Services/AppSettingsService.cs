using Microsoft.EntityFrameworkCore;
using LecRate.Data;
using LecRate.Entities;

namespace LecRate.Services
{


    public class AppSettingsService : IAppSettingsService
    {
        private readonly AppDbContext _context;

        public AppSettingsService(AppDbContext context)
        {
            _context = context;
        }

        
        public async Task<bool> IsEvaluationOpen()
        {
            var settings = await _context.AppSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                
                var firstAdminId = await _context.Admins.Select(a => a.AdminId).FirstOrDefaultAsync();
                if (firstAdminId == 0)
                {
                    throw new InvalidOperationException("لا يمكن إنشاء إعدادات افتراضية لعدم وجود مديرين (Admin) في النظام.");
                }

                settings = new AppSettings 
                { 
                    IsEvaluationOpen = true,
                    AdminId = firstAdminId
                };
                _context.AppSettings.Add(settings);
                await _context.SaveChangesAsync();
            }
            return settings.IsEvaluationOpen;
        }

        
        public async Task<bool> ToggleEvaluation(bool isOpen)
        {
            var settings = await _context.AppSettings.FirstOrDefaultAsync();
            if (settings == null)
            {
                var firstAdminId = await _context.Admins.Select(a => a.AdminId).FirstOrDefaultAsync();
                if (firstAdminId == 0)
                {
                    throw new InvalidOperationException("لا يمكن إنشاء إعدادات افتراضية لعدم وجود مديرين (Admin) في النظام.");
                }

                settings = new AppSettings 
                { 
                    IsEvaluationOpen = isOpen,
                    AdminId = firstAdminId
                };
                _context.AppSettings.Add(settings);
            }
            else
            {
                settings.IsEvaluationOpen = isOpen;
            }
            await _context.SaveChangesAsync();
            return settings.IsEvaluationOpen;
        }
    }
}