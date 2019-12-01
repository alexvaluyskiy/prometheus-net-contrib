using System;
using System.Linq;
using System.Reflection;

namespace Prometheus.Contrib.Core
{
    internal class PropertyFetcher
    {
        private readonly string propertyName;
        private PropertyFetch innerFetcher;

        public PropertyFetcher(string propertyName)
        {
            this.propertyName = propertyName;
        }

        public object Fetch(object obj)
        {
            if (innerFetcher == null)
            {
                var type = obj.GetType().GetTypeInfo();
                var property = type.DeclaredProperties.FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.InvariantCultureIgnoreCase));
                if (property == null)
                    property = type.GetProperty(propertyName);

                innerFetcher = PropertyFetch.FetcherForProperty(property);
            }

            return innerFetcher?.Fetch(obj);
        }

        private class PropertyFetch
        {
            /// <summary>
            /// Create a property fetcher from a .NET Reflection PropertyInfo class that
            /// represents a property of a particular type.
            /// </summary>
            public static PropertyFetch FetcherForProperty(PropertyInfo propertyInfo)
            {
                if (propertyInfo == null)
                    // returns null on any fetch.
                    return new PropertyFetch();

                var typedPropertyFetcher = typeof(TypedFetchProperty<,>);
                var instantiatedTypedPropertyFetcher = typedPropertyFetcher.GetTypeInfo().MakeGenericType(
                    propertyInfo.DeclaringType, propertyInfo.PropertyType);
                return (PropertyFetch)Activator.CreateInstance(instantiatedTypedPropertyFetcher, propertyInfo);
            }

            /// <summary>
            /// Given an object, fetch the property that this propertyFetch represents.
            /// </summary>
            public virtual object Fetch(object obj)
            {
                return null;
            }

            private class TypedFetchProperty<TObject, TProperty> : PropertyFetch
            {
                private readonly Func<TObject, TProperty> propertyFetch;

                public TypedFetchProperty(PropertyInfo property)
                {
                    propertyFetch = (Func<TObject, TProperty>)property.GetMethod.CreateDelegate(typeof(Func<TObject, TProperty>));
                }

                public override object Fetch(object obj)
                {
                    if (obj is TObject o)
                        return propertyFetch(o);

                    return null;
                }
            }
        }
    }
}
