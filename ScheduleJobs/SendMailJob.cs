using api_aspnetcore6.Dtos;
using api_aspnetcore6.Models;
using api_aspnetcore6.Services.Interfaces;
using Dapper;
using Hangfire;

namespace api_aspnetcore6.ScheduleJobs
{
    public class SendMailJob : ISendMailJob
    {
        private readonly ISendMailService _sendMailJobService;
        private readonly IRecurringJobManager _recurringJobManager;
        public SendMailJob(ISendMailService sendMailService, IRecurringJobManager recurringJobManager)
        {
            _sendMailJobService = sendMailService;
            _recurringJobManager = recurringJobManager;
        }

        public void RecurringJobSendMailRevenueDaily()
        {
            _recurringJobManager.AddOrUpdate("daily-revenue-report", () => SendMailRevenueDaily(), Cron.Daily);
        }

        public async Task SendMailRevenueDaily()
        {
            using (var connection = DapperContext.CreateConnection())
            {
                var revenue = await connection.QueryFirstOrDefaultAsync("GetDailyRevenue");

                var toEmail = new string[] { "vandatskt1@gmail.com" };
                var subject = "Daily Revenue Report";
                var body = $"Here is your daily revenue report: {revenue.ToString()}";

                var message = new MailMessage(toEmail, subject, body);

                await _sendMailJobService.SendEmailAsync(message);
            }

        }
    }
}