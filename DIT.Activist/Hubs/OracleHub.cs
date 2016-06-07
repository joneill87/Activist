using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Threading.Tasks;
using DIT.Activist.Domain.Models;
using DIT.Activist.Infrastructure;

namespace DIT.Activist.Hubs
{
    public class OracleHub : Hub
    {
        public async Task OnQueryIssued(JobIteration iteration)
        {
           await Clients.All.onQueryIssued(iteration);
        }

        public async Task OnJobCompleted()
        {
            await Clients.All.onJobCompleted();
        }

        public async Task BeginTest()
        {
            await AdHocTesting.AdHocTesting.RunCreateJobTest();
        }

        public async Task LabelsProvided(JobIteration iteration)
        {
            await AdHocTesting.AdHocTesting.RunIterationTest(iteration.IterationID, iteration.QueryLabels);
        }

        public async Task OnError(Exception e)
        {
            await Clients.All.onJobError(e);
        }

        public async Task Register(Guid jobId)
        {
            await Groups.Add(Context.ConnectionId, jobId.ToString());
        }

        public async Task Deregister(Guid jobId)
        {
            await Groups.Remove(Context.ConnectionId, jobId.ToString());
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}