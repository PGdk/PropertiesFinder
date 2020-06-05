using System;
using System.Reflection;

namespace Extensions.Reflection
{
    public static class PropertyApplier
    {
        public static void ApplyNewValues<T>(this T baseObject, T newObject) where T : class
        {
            baseObject.ApplyNewValues(newObject, 0);
        }

        internal static void ApplyNewValues(this object baseObject, object newObject, int indent)
        {
            if (baseObject == null)
            {
                return;
            }

            Type objType = baseObject.GetType();
            PropertyInfo[] properties = objType.GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object basePropValue = property.GetValue(baseObject, null);
                object newPropValue = property.GetValue(newObject, null);

                if (basePropValue?.Equals(newPropValue) ?? false) continue;

                if (basePropValue == null)
                {
                    property.SetValue(baseObject, newPropValue);
                }
                else if (newPropValue != null && newObject != null)
                {
                    if (property.PropertyType.Assembly == objType.Assembly)
                    {
                        ApplyNewValues(basePropValue, newPropValue, indent + 2);
                    }
                    else
                    {
                        property.SetValue(baseObject, newPropValue);
                    }
                }
            }
        }

    }
}