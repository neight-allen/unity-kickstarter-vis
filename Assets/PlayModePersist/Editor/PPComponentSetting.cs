//AlmostLogical Software - http://www.almostlogical.com - support@almostlogical.com
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

public class PPComponentSetting
{
    private Dictionary<string, object> values = null;

    public PPComponentSetting(Component component)
    {
        this.ComponentObject = component;
    }

    private Component ComponentObject { get; set; }

    public bool IsExpanded { get; set; }

    public bool IsSavingSettings { get; set; }

    public string ComponentName
    {
        get
        {
            string[] parts = ComponentObject.GetType().ToString().Split(new string[] { "." }, StringSplitOptions.RemoveEmptyEntries);
            return parts[parts.Length - 1];
        }
    }

    public List<string> PropertiesToIgnore
    {
        get
        {
            List<string> propertiesToIgnore = new List<string>();

            List<string> propertiesManuallyExcluded = PPLocalStorageManager.GetExclusionsForType(ComponentObject.GetType());

            if (null != propertiesManuallyExcluded)
            {
                propertiesToIgnore.AddRange(propertiesManuallyExcluded);
            }
           
            foreach (PropertyInfo propertyInfo in typeof(Component).GetProperties())
            {
                propertiesToIgnore.Add(propertyInfo.Name);
            }

            return propertiesToIgnore;
        }
    }

    public void ClearSettings()
    {
        values = null;
    }

    public void StoreSettings()
    {
        if (IsSavingSettings && null != ComponentObject)
        {
            values = new Dictionary<string, object>();

            List<string> propertiesToIgnore = PropertiesToIgnore;

            List<PropertyInfo> properties = GetProperties(propertiesToIgnore);
            List<FieldInfo> fields = GetFields(propertiesToIgnore);

            foreach (PropertyInfo property in properties)
            {
                values.Add(property.Name, property.GetValue(ComponentObject, null));
            }
            foreach (FieldInfo field in fields)
            {
                values.Add(field.Name, field.GetValue(ComponentObject));
            }
        }
    }

    private List<FieldInfo> GetFields(List<string> propertiesToIgnore)
    {
        List<FieldInfo> fields = new List<FieldInfo>();

        foreach (FieldInfo fieldInfo in ComponentObject.GetType().GetFields())
        {
            if (fieldInfo.IsPublic && !propertiesToIgnore.Contains(fieldInfo.Name))
            {
                if (!Attribute.IsDefined(fieldInfo, typeof(HideInInspector)))
                {
                    fields.Add(fieldInfo);
                }
            }
        }

        return fields;
    }

    private List<PropertyInfo> GetProperties(List<string> propertiesToIgnore)
    {
        List<PropertyInfo> properties = new List<PropertyInfo>();

        foreach (PropertyInfo propertyInfo in ComponentObject.GetType().GetProperties())
        {
            if (!propertiesToIgnore.Contains(propertyInfo.Name))
            {
                if (!Attribute.IsDefined(propertyInfo, typeof(HideInInspector)))
                {
                    MethodInfo setMethod = propertyInfo.GetSetMethod();
                    if (null != setMethod && setMethod.IsPublic)
                    {
                        properties.Add(propertyInfo);
                    }
                }
            }
        }

        return properties;
    }

    public void RestoreSettings()
    {
        if (ComponentObject != null)
        {
            ComponentObject = EditorUtility.InstanceIDToObject(ComponentObject.GetInstanceID()) as Component;
        }

        if (IsSavingSettings && null != ComponentObject && null != values)
        {
            foreach (string name in values.Keys)
            {
                object newValue = values[name];

                PropertyInfo property = ComponentObject.GetType().GetProperty(name);

                if (null != property)
                {
                    object currentValue = property.GetValue(ComponentObject, null);

                    if (HasValueChanged(newValue, currentValue))
                    {
                        property.SetValue(ComponentObject, newValue, null);
                    }
                }
                else
                {
                    FieldInfo field = ComponentObject.GetType().GetField(name);
                    object currentValue = field.GetValue(ComponentObject);

                    if (HasValueChanged(newValue, currentValue))
                    {
                        field.SetValue(ComponentObject, newValue);
                    }
                }
            }
        }

        values = null;
    }

    private bool HasValueChanged(object newValue, object oldValue)
    {
        bool valuesChanged = true;

        if (null != newValue && null != oldValue)
        {
            IComparable valueToCompare = newValue as IComparable;

            if (null == valueToCompare)
            {
                try
                {
                    XmlSerializer serializer = new XmlSerializer(newValue.GetType());

                    using (MemoryStream streamNew = new MemoryStream())
                    {
                        serializer.Serialize(streamNew, newValue);

                        UTF8Encoding encoding = new UTF8Encoding();

                        string oldValueSerialized = encoding.GetString(streamNew.ToArray());

                        using (MemoryStream streamOld = new MemoryStream())
                        {
                            serializer.Serialize(streamOld, oldValue);

                            string newValueSerialized = encoding.GetString(streamOld.ToArray());

                            valuesChanged = !string.Equals(newValueSerialized, oldValueSerialized);
                        }
                    }
                }
                catch
                {
                    valuesChanged = true;
                }
            }
            else
            {
                valuesChanged = valueToCompare.CompareTo(oldValue) != 0;
            }
        }
        else if (null == oldValue && null == newValue)
        {
            valuesChanged = false;
        }

        return valuesChanged;
    }
}