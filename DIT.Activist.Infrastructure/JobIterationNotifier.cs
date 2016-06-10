using DIT.Activist.Domain.Interfaces.ActiveLoop;
using DIT.Activist.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.Infrastructure
{
    internal class JobIterationNotifier : IJobIterationNotifier
    {
        private List<IObserver<JobIteration>> observers;

        private static readonly JobIterationNotifier _instance = new JobIterationNotifier();

        public static JobIterationNotifier Instance {  get { return _instance; } }

        public IDisposable Subscribe(IObserver<JobIteration> observer)
        {
            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }

            return new Unsubscriber(observers, observer);
        }

        public void OnLabelsRequested(JobIteration newIteration)
        {
            foreach (var observer in observers)
            {
                observer.OnNext(newIteration);
            }
        }

        public void LabellingJobComplete()
        {
            foreach (var observer in observers.ToArray())
            {
                if (observers.Contains(observer))
                {
                    observer.OnCompleted();
                }
            }
            observers.Clear();
        }

        private JobIterationNotifier()
        {
            observers = new List<IObserver<JobIteration>>();
        }

        private class Unsubscriber : IDisposable
        {
            private List<IObserver<JobIteration>> _observers;
            private IObserver<JobIteration> _thisObserver;

            public Unsubscriber(List<IObserver<JobIteration>> observers, IObserver<JobIteration> thisObserver)
            {
                _observers = observers;
                _thisObserver = thisObserver;
            }

            public void Dispose()
            {
                if (_observers != null)
                {
                    _observers.Remove(_thisObserver);
                }
            }
        }
    }
}
