using DIT.Activist.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Domain.Interfaces.ActiveLoop
{
    public interface ILoopNotificationHub
    {
        void Notify(JobIteration jobIteration);

        void Subscribe(Action<JobIteration> handler);
    }
}
