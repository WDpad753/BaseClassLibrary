using BaseClass.Base.Interface;
using BaseClass.Helper;
using BaseClass.Model;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.Concurrent;

namespace BaseClass.Base
{
    public class BaseProvider : IBaseProvider, IDisposable
    {
        private readonly ConcurrentDictionary<Type, BaseDescriptor> _items = new();
        private readonly ConcurrentDictionary<Type, object> _singletonItems = new();
        private readonly ThreadLocal<HashSet<Type>> _resolutionStack = new(() => new HashSet<Type>());
        private bool _disposed;

        /// <summary>
        /// Registers a pre-existing instance of the specified type in the container.
        /// </summary>
        /// <typeparam name="T">The type of the instance being registered.</typeparam>
        /// <param name="instance">The concrete instance to register.</param>
        /// <exception cref="InvalidOperationException">Thrown if the type <typeparamref name="T"/> has already been registered.
        /// </exception>
        public void RegisterInstance<T>(T instance) => Register<T>(BaseLifetime.Singleton, instance: instance, implementationType: typeof(T));
        //{
        //    if (_items.ContainsKey(typeof(T)))
        //        throw new InvalidOperationException($"{typeof(T)} is already registered.");

        //    _items[typeof(T)] = new BaseDescriptor
        //    {
        //        BaseType = typeof(T),
        //        Lifetime = BaseLifetime.Singleton,
        //        Instance = instance
        //    };

        //    _singletonItems[typeof(T)] = instance!;
        //}

        /// <summary>
        /// Registers a type as Transient: a new instance is created every time it is requested.
        /// Ideal for lightweight, stateless items.
        /// </summary>
        /// <typeparam name="TInterface">The interface or base type to register (if applicable).</typeparam>
        /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
        //public void RegisterTransient<TInterface, TImplementation>() where TImplementation : class, TInterface => Register<TInterface, TImplementation>(BaseLifetime.Transient);
        public void RegisterTransient<TInterface, TImplementation>() where TImplementation : class, TInterface => Register<TInterface>(BaseLifetime.Transient, implementationType: typeof(TImplementation));

        /// <summary>
        /// Registers a type as Transient: a new instance is created every time it is requested.
        /// Ideal for lightweight, stateless items.
        /// </summary>
        /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
        public void RegisterTransient<TImplementation>() where TImplementation : class => Register<TImplementation>(BaseLifetime.Transient, implementationType: typeof(TImplementation));


        /// <summary>
        /// Registers a type as Singleton: a single instance is created and reused throughout the application lifetime.
        /// </summary>
        /// <typeparam name="TInterface">The interface or base type to register (if applicable).</typeparam>
        /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
        //public void RegisterSingleton<TInterface, TImplementation>() where TImplementation : class, TInterface => Register<TInterface, TImplementation>(BaseLifetime.Singleton);
        public void RegisterSingleton<TInterface, TImplementation>() where TImplementation : class, TInterface => Register<TInterface>(BaseLifetime.Singleton, implementationType: typeof(TImplementation));

        /// <summary>
        /// Registers a type as Singleton: a single instance is created and reused throughout the application lifetime.
        /// </summary>
        /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
        public void RegisterSingleton<TImplementation>() where TImplementation : class => Register<TImplementation>(BaseLifetime.Singleton, implementationType: typeof(TImplementation));

        /// <summary>
        /// Registers a factory method for creating instances of a type with the specified lifetime.
        /// </summary>
        /// <typeparam name="T">The type to register.</typeparam>
        /// <param name="factory">The factory function used to create instances.</param>
        /// <param name="lifetime">The lifetime of the service (Transient or Singleton).</param>
        //public void RegisterFactory<T>(Func<IBaseContainer, T> factory, BaseLifetime lifetime = BaseLifetime.Transient)
        //{
        //    _items[typeof(T)] = new BaseDescriptor
        //    {
        //        BaseType = typeof(T),
        //        Lifetime = lifetime,
        //        Factory = c => factory(c)
        //    };
        //}
        public void RegisterFactory<T>(Func<IBaseProvider, T> factory, BaseLifetime lifetime = BaseLifetime.Transient) => Register<T>(lifetime, factory: c => factory(c)!);

        /// <summary>
        /// Internal registration logic shared by Singleton and Transient registrations.
        /// </summary>
        /// <typeparam name="TInterface">The interface or base type to register (if applicable).</typeparam>
        /// <typeparam name="TImplementation">The concrete implementation type.</typeparam>
        /// <param name="lifetime">Type of LifeTime</param>
        /// <exception cref="InvalidOperationException">If the item exists</exception>
        //private void Register<TInterface, TImplementation>(BaseLifetime lifetime) where TImplementation : class, TInterface
        //{
        //    if (_items.ContainsKey(typeof(TInterface)))
        //        throw new InvalidOperationException($"{typeof(TInterface)} is already registered.");

        //    _items[typeof(TInterface)] = new BaseDescriptor
        //    {
        //        BaseType = typeof(TInterface),
        //        ImplementationType = typeof(TImplementation),
        //        Lifetime = lifetime
        //    };
        //}
        private void Register<TInterface>(BaseLifetime lifetime, TInterface? instance = default, Type? implementationType = null, Func<IBaseProvider, object>? factory = null)
        {
            //if (_items.ContainsKey(typeof(TInterface)))
            //    throw new InvalidOperationException($"{typeof(TInterface)} is already registered.");

            _items[typeof(TInterface)] = new BaseDescriptor
            {
                BaseType = typeof(TInterface),
                ImplementationType = implementationType,
                Lifetime = lifetime,
                Instance = instance != null ? instance : null,
                Factory = factory != null ? c => factory(c) : null,
            };
        }

        /// <summary>
        /// Checks if a type is already registered in the container.
        /// </summary>
        /// <typeparam name="TImplementation">The type to check.</typeparam>
        /// <returns>True if the type exists in the registration dictionary; otherwise false.</returns>
        public bool IsItemRegistered<TImplementation>()
        {
            return _items.ContainsKey(typeof(TImplementation));
        }

        /// <summary>
        /// Resolves an instance of the specified type from the container
        /// </summary>
        /// <typeparam name="T">The type to resolve.</typeparam>
        /// <returns>An instance of the requested type.</returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown if the type is not registered or if a circular dependency is detected.
        /// </exception>
        public T GetItem<T>() => (T)GetItem(typeof(T));

        public object GetItem(Type type)
        {
            if (!_items.TryGetValue(type, out var descriptor))
                throw new InvalidOperationException($"Type {type.Name} is not registered.");

            if (_resolutionStack.Value.Contains(type))
                throw new InvalidOperationException($"Circular dependency detected: {type.Name}");

            _resolutionStack.Value.Add(type);

            try
            {
                return Resolve(descriptor);
            }
            finally
            {
                _resolutionStack.Value.Remove(type);
            }
        }

        /// <summary>
        /// Registers a concrete instance of a type in the container.
        /// </summary>
        /// <typeparam name="T">The type of the instance to register.</typeparam>
        /// <param name="name">Key name of the registered instance</param>
        /// <returns>Returns the instance</returns>
        public T? GetValue<T>(string name)
        {
            var valCollector = GetItem<ValueCollector<T>>();
            
            if (valCollector != null)
            {
                if (valCollector.ValName == name)
                    return valCollector.Value;
            }

            return default;
        }

        /// <summary>
        /// Resolves a service based on its descriptor and lifetime strategy.
        /// </summary>
        /// <param name="descriptor">The service descriptor.</param>
        /// <returns>The resolved service instance.</returns>
        private object Resolve(BaseDescriptor descriptor)
        {
            if (descriptor.Instance != null)
                return descriptor.Instance;

            if (descriptor.Factory != null)
                return descriptor.Factory(this);

            return descriptor.Lifetime switch
            {
                BaseLifetime.Singleton => _singletonItems.GetOrAdd(descriptor.BaseType, _ => CreateInstance(descriptor.ImplementationType)),
                BaseLifetime.Transient => CreateInstance(descriptor.ImplementationType),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        /// <summary>
        /// Creates a new instance of the specified implementation type by resolving its constructor dependencies.
        /// </summary>
        /// <param name="implementationType">The type to instantiate.</param>
        /// <returns>A new instance of the specified type.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no public constructor is found.</exception>
        private object? CreateInstance(Type implementationType)
        {
            var ctor = implementationType.GetConstructors().OrderByDescending(c => c.GetParameters().Length).FirstOrDefault();

            if (ctor == null)
                throw new InvalidOperationException($"No public constructor for {implementationType.Name}");

            var parameters = ctor.GetParameters().Select(p => GetItem(p.ParameterType)).ToArray();

            return Activator.CreateInstance(implementationType, parameters)!;
        }

        /// <summary>
        /// Disposing of all created instances. 
        /// </summary>
        /// <param name="disposing">Indicates if the managed resources should be disposed.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    foreach (var instance in _singletonItems.Values)
                    {
                        if (instance is IDisposable disposable)
                            disposable.Dispose();
                    }

                    _singletonItems.Clear();
                    _items.Clear();
                    _resolutionStack.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
