using BaseClass.Base.Interface;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseClass.Model
{
    /*
     * This Section is for describing what is being registered into the Container.
     * ServiceType: Interface or abstract class we’re registering
     * ImplementationType: Concrete class that implements the service
     * Lifetime: Determines instance management strategy
     * Instance: Pre-created objects (for singletons)
     * Factory: Custom creation logic when needed
     */

    public class BaseDescriptor
    {
        public Type? BaseType { get; set; }
        public Type? ImplementationType { get; set; }
        public BaseLifetime Lifetime { get; set; }
        public object? Instance { get; set; }
        public Func<IBaseProvider, object>? Factory { get; set; }
    }

    public enum BaseLifetime
    {
        Transient,
        Scoped,
        Singleton
    }
}
