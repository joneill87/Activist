using DIT.Activist.Domain.Interfaces.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace DIT.Activist.ActiveLearning.Factories
{
    public abstract class BaseFactory<T> where T : IActivatable
    {
        protected abstract string RootNamespace { get; }

        public virtual T Create(string typeName)
        {
            return Create(typeName, new Dictionary<string, string>());
        }

        public virtual T Create(string typeName, Dictionary<string, string> parameters)
        {
            Assembly currentAssembly = Assembly.GetExecutingAssembly();
            T instance = (T)currentAssembly.CreateInstance(RootNamespace + "." + typeName);
            instance.Initialize(parameters);
            return instance;
        }
    }
}
