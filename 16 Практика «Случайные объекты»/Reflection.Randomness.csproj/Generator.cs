using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Reflection.Randomness
{
    public class FromDistribution : Attribute
    {
        public IContinuousDistribution Distribution { get; }

        public FromDistribution(Type type, params object[] args)
        {
            try
            {
                Distribution = (IContinuousDistribution)Activator.CreateInstance(type, args);
            }
            catch (Exception)
            {
                throw new ArgumentException(type.Name);
            }
        }
    }

    public class Generator<T>
    {
        private readonly Dictionary<string, IContinuousDistribution> distributions = 
            new Dictionary<string, IContinuousDistribution>();
        private Func<Dictionary<string, IContinuousDistribution>, Random, T> compiledFunc;
        private bool isNeedToRecompileFunc;
        public class PropertiesKeeper
        {
            private string PropertyName { get; }
            private Generator<T> Parent { get; }

            public PropertiesKeeper(string propertyName, Generator<T> parent)
            {
                PropertyName = propertyName;
                Parent = parent;
            }

            public Generator<T> Set(IContinuousDistribution distribution)
            {
                Parent.distributions[PropertyName] = distribution;
                Parent.isNeedToRecompileFunc = true;
                return Parent;
            }
        }

        private void CreateDictionaryOfProperties()
        {
            foreach (var propertyWithCustomAttribute in typeof(T)
                .GetProperties()
                .Where(p => p.GetCustomAttribute<FromDistribution>() != null)
                .Select(p => new { p.Name, Attribute = p.GetCustomAttribute<FromDistribution>() })
                    )
            {
                distributions[propertyWithCustomAttribute.Name] =
                    propertyWithCustomAttribute.Attribute.Distribution;
            }
        }

        private void CompileFunc()
        {
            // (rnd, dict) => new T{P = dict["P"].Generate(rnd), ...}

            var rnd = Expression.Parameter(typeof(Random), "rnd");
            var dict = Expression.Parameter(typeof(Dictionary<string, IContinuousDistribution>), "dict");
            var bindings = CreateBindings(rnd, dict);

            var body = Expression.MemberInit(
                Expression.New(typeof(T).GetConstructor(new Type[0])),
                bindings
            );

            var lambda =
                Expression.Lambda<Func<Dictionary<string, IContinuousDistribution>, Random, T>>(
                    body,
                    dict,
                    rnd
                );

            compiledFunc = lambda.Compile();
            isNeedToRecompileFunc = false;
        }

        private List<MemberBinding> CreateBindings(ParameterExpression rnd, ParameterExpression dict)
        {
            var bindings = new List<MemberBinding>();
            var properties = typeof(T).GetProperties()
                .Where(p => distributions.ContainsKey(p.Name))
                .Select(p => new { p.Name, Info = p });
            foreach (var property in properties)
            {
                var dictValue = Expression.Parameter(typeof(IContinuousDistribution));
                var dictAccess = Expression.Block( new[] { dictValue }, Expression
                    .Assign(dictValue, Expression.Property(dict, "Item", Expression.Constant(property.Name))));
                var methodCall = Expression.Call(dictAccess, "Generate", null, rnd );
                var binding = Expression.Bind(property.Info, methodCall);
                bindings.Add(binding);
            }
            return bindings;
        }

        public Generator()
        {
            CreateDictionaryOfProperties();
            CompileFunc();
        }

        public T Generate(Random rnd)
        {
            if (isNeedToRecompileFunc)
                CompileFunc();
            return compiledFunc(distributions, rnd);
        }
    }
}