using BaseClass.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Base.Interface
{
    public interface IBaseProvider
    {
        public void RegisterInstance<T>(T instance);
        public void RegisterTransient<TInterface, TImplementation>() where TImplementation : class, TInterface;
        public void RegisterTransient<TImplementation>() where TImplementation : class;
        public void RegisterSingleton<TInterface, TImplementation>() where TImplementation : class, TInterface;
        public void RegisterSingleton<TImplementation>() where TImplementation : class;
        public void RegisterFactory<T>(Func<IBaseProvider, T> factory, BaseLifetime lifetime = BaseLifetime.Transient);
        public bool IsItemRegistered<TImplementation>();
        public T? GetValue<T>(string name);
        public T GetItem<T>();
        public object GetItem(Type type);
    }
}
