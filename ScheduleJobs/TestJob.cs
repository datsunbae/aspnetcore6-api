
namespace api_aspnetcore6.ScheduleJobs
{
    public class TestJob : IJob
    {
        public void ContinuousJob()
        {
            Console.WriteLine("Hello from continuations job!");
        }

        public void DelayedJob()
        {
            Console.WriteLine("Hello from delayed job!");
        }

        public void FireAndForgetJob()
        {
            Console.WriteLine("Hello from fire and forget job!");
        }

        public void RecurringJob()
        {
            Console.WriteLine("Helle from scheduled job!");
        }
    }
}