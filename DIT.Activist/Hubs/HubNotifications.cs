using DIT.Activist.Domain.Models;
using DIT.Activist.Infrastructure;
using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace DIT.Activist.Hubs
{
    public static class HubNotifications
    {
        public static void RegisterHubNotifier()
        {
            var observer = new JobIterationObserver();
            JobIterationNotifier.Instance.Subscribe(observer);
        }

        private class JobIterationObserver : IObserver<JobIteration>
        {
            protected dynamic connectedClients {  get { return GlobalHost.ConnectionManager.GetHubContext<OracleHub>().Clients.All; } }

            public void OnCompleted()
            {
                connectedClients.onJobCompleted();
            }

            public void OnError(Exception error)
            {
                connectedClients.onJobError();
            }

            public void OnNext(JobIteration iteration)
            {
                connectedClients.onQueryIssued(iteration);
            }
        }
    }
}