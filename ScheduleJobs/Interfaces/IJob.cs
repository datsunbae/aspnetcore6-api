using api_aspnetcore6.Dtos;

namespace api_aspnetcore6.ScheduleJobs
{
    public interface IJob
    {
        void FireAndForgetJob();
        void DelayedJob();
        void ContinuousJob();
        void RecurringJob();
    }
}