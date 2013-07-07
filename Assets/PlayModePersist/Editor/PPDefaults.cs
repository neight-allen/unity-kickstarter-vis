//AlmostLogical Software - http://www.almostlogical.com - support@almostlogical.com
using System;
using System.Collections.Generic;
using System.Linq;

public class PPDefaults
{
    private List<string> typesToDefault = new List<string>();

    private HashSet<string> cachedTypesDefaultYes = new HashSet<string>();
    private HashSet<string> cachedTypesDefaultNo = new HashSet<string>();

    public PPDefaults()
    {
        LoadDefaults();
    }

    private void LoadDefaults()
    {
        string defaultInfo = PPDefaultPropertiesInfo.DefaultInfo ?? "";

        string[] components = defaultInfo.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);

        typesToDefault.AddRange(components.Distinct());
    }

    public bool IsTypeDefaulted(Type type)
    {
        string typeName = type.ToString();

        bool isDefaulted = false;

        if (cachedTypesDefaultYes.Contains(typeName))
        {
            isDefaulted = true;
        }
        else if (cachedTypesDefaultNo.Contains(typeName))
        {
            isDefaulted = false;
        }
        else
        {
            List<string> classNames = GetClassNames(type);
            isDefaulted = classNames.Intersect(typesToDefault).Count() > 0;

            if (isDefaulted)
            {
                cachedTypesDefaultYes.Add(typeName);
            }
            else
            {
                cachedTypesDefaultNo.Add(typeName);
            }
        }

        return isDefaulted;
    }

    private static List<string> GetClassNames(Type type)
    {
        List<string> classNames = new List<string>();

        classNames.Add(type.ToString());

        while (null != type.BaseType)
        {
            classNames.Add(type.BaseType.ToString());
            type = type.BaseType;
        }

        return classNames;
    }
}