using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Ddd.Taxi.Domain;

namespace Ddd.Infrastructure
{
    public class ValueType<T>
    {
        private List<PropertyInfo> PropertiesOfOrder { get; }

        public ValueType()
        {
            PropertiesOfOrder = 
                    GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .OrderBy(property => property.Name)
                    .ToList();
        }

        public override bool Equals(object objToCheck)
        {
            if (objToCheck is null)
                return false;
            else if (objToCheck.GetType() != GetType())
                return false;
            else if (ReferenceEquals(this, objToCheck))
                return true;

            foreach (var propertyOfOrder in PropertiesOfOrder)
            {
                var thisObjValue = propertyOfOrder.GetValue(this, null);
                var otherObjValue = propertyOfOrder.GetValue(objToCheck, null);
                if (thisObjValue == null & otherObjValue == null)
                    continue;
                else if (thisObjValue == null || otherObjValue == null || !thisObjValue.Equals(otherObjValue))
                    return false;
            }
            return true;
        }

        public bool Equals(PersonName name)
        {
            return Equals((object)name);
        }

        public override int GetHashCode()
        {
            int hashCode = 0;

            unchecked
            {
                foreach (var property in PropertiesOfOrder)
                {
                    var propertyHashCode = property.GetValue(this, null).GetHashCode();
                    hashCode = (hashCode * 5036324) ^ propertyHashCode;
                }
            }

            return hashCode;
        }

        public override string ToString()
        {
            var properties = new StringBuilder(GetType().Name);
            int index = 0;
            properties.AppendFormat("(");
            foreach (var property in PropertiesOfOrder)
            {
                if (index != PropertiesOfOrder.Count - 1)
                    properties.AppendFormat("{0}: {1}; ", property.Name, property.GetValue(this, null));
                else
                    properties.AppendFormat("{0}: {1}", property.Name, property.GetValue(this, null));
                index++;
            }
            properties.AppendFormat(")");

            return properties.ToString();
        }
    }
}