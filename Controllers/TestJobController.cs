using api_aspnetcore6.Dtos;
using api_aspnetcore6.ScheduleJobs;
using api_aspnetcore6.Services.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api_aspnetcore6.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestJobController : ControllerBase
    {
        private readonly IJob _job;
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly IRecurringJobManager _recurringJobManager;
        public TestJobController(IJob job, IBackgroundJobClient backgroundJobClient, IRecurringJobManager recurringJobManager)
        {
            _job = job;
            _backgroundJobClient = backgroundJobClient;
            _recurringJobManager = recurringJobManager;
        }

        [HttpGet("fireandforgot")]
        public ActionResult CreateFireAndForgorJob()
        {
            try
            {
                _backgroundJobClient.Enqueue(() => _job.FireAndForgetJob());
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }

        [HttpGet("delayed")]
        public ActionResult CreateDelayedJob()
        {
            try
            {
                _backgroundJobClient.Schedule(() => _job.DelayedJob(), TimeSpan.FromSeconds(60));
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }

        [HttpGet("recurring")]
        public ActionResult CreateRecurringJob()
        {
            try
            {
                _recurringJobManager.AddOrUpdate("testJob", () => _job.RecurringJob(), Cron.Minutely);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }

        [HttpGet("continuations")]
        public ActionResult CreateContinuationsJob()
        {
            try
            {
                var parentJobId = _backgroundJobClient.Enqueue(() => _job.FireAndForgetJob());
                _backgroundJobClient.ContinueJobWith(parentJobId, () => _job.ContinuousJob());
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An internal server error occurred: " + ex.Message);
            }
        }
    }
}