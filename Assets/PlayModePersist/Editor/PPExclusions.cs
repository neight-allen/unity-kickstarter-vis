//AlmostLogical Software - http://www.almostlogical.com - support@almostlogical.com
using System;
using System.Collections.Generic;

public class PPExclusions
{
    private Dictionary<string, List<string>> exclusions = new Dictionary<string, List<string>>();

    public PPExclusions()
    {
        LoadExclusions();
    }

    private void LoadExclusions()
    {
        string exclusionInfo = PPExcludedPropertiesInfo.ExclusionInfo ?? "";

        string[] parts = exclusionInfo.Split(new string[] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);

        string currentType = "";

        foreach (string part in parts)
        {
            string trimmedPart = part.Trim();

            if (!trimmedPart.StartsWith("-"))
            {
                currentType = trimmedPart;
            }
            else
            {
                trimmedPart = trimmedPart.Remove(0, 1).Trim();

                if (!exclusions.ContainsKey(currentType))
                {
                    exclusions.Add(currentType, new List<string>());
                }

                exclusions[currentType].Add(trimmedPart);
            }
        }
    }

    public List<string> GetExclusionsForType(Type type)
    {
        List<string> exclusionList = new List<string>();

        List<string> classNames = GetClassNames(type);

        foreach (string className in classNames)
        {
            if (exclusions.ContainsKey(className))
            {
                exclusionList.AddRange(exclusions[className]);
            }
        }

        return exclusionList;
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