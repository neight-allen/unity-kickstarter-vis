//AlmostLogical Software - http://www.almostlogical.com - support@almostlogical.com
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;

public static class PPLocalStorageManager
{
    private const string KeyExclusions = "PMPersist_Excludes";
    private const string KeyDefaults = "PMPersist_Defaults";
    private const string SeperatorProperty = ",";
    private const string SeperatorClass = "|";
    private const string SeperatorPropertiesList = ":";

    private static Dictionary<string, List<string>> systemExclusions;
    private static Dictionary<string, List<string>> exclusions;
    private static List<string> defaults;

    private static HashSet<string> cachedTypesDefaultYes = new HashSet<string>();
    private static HashSet<string> cachedTypesDefaultNo = new HashSet<string>();

    static PPLocalStorageManager()
    {
        LoadSystemExclusions();
        LoadExclusions();
        LoadDefaults();
    }

    public static List<string> Defaults
    {
        get { return PPLocalStorageManager.defaults; }
    }


    public static List<string> GetExclusionsForType(Type type)
    {
        List<string> exclusionList = new List<string>();

        List<string> classNames = GetClassNames(type);

        foreach (string className in classNames)
        {
            if (exclusions.ContainsKey(className))
            {
                exclusionList.AddRange(exclusions[className]);
            }

            if (systemExclusions.ContainsKey(className))
            {
                exclusionList.AddRange(systemExclusions[className]);
            }
        }

        return exclusionList;
    }

    public static bool IsTypeDefaulted(Type type)
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
            isDefaulted = classNames.Intersect(defaults).Count() > 0;

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

    public static void AddExclusion(string className, string propertyName)
    {
        List<string> existingExclusions;

        bool exclusionFound = exclusions.TryGetValue(className, out existingExclusions);

        if (exclusionFound)
        {
            exclusionFound = existingExclusions.Contains(propertyName);
        }
        else
        {
            existingExclusions = new List<string>();
            exclusions.Add(className, existingExclusions);
        }

        if (!exclusionFound)
        {
            existingExclusions.Add(propertyName);
            exclusions[className] = existingExclusions;

            SaveExclusions();
        }
    }

    public static void RemoveExclusion(string className, string propertyName)
    {
        List<string> existingExclusions;

        bool exclusionFound = exclusions.TryGetValue(className, out existingExclusions);

        if (exclusionFound)
        {
            if (existingExclusions.Remove(propertyName))
            {
                exclusions[className] = existingExclusions;
                SaveExclusions();
            }
        }
    }

    public static void AddDefault(string className)
    {
        if (!defaults.Contains(className))
        {
            defaults.Add(className);
            SaveDefaults();
        }
    }

    public static void RemoveDefault(string className)
    {
        if (defaults.Contains(className))
        {
            defaults.Remove(className);
            SaveDefaults();
        }
    }

    private static void SaveExclusions()
    {
        string exclusionString = string.Join("|", exclusions.Select(e => string.Format("{0}:{1}", e.Key, string.Join(",", e.Value.ToArray()))).ToArray());
        EditorPrefs.SetString(KeyExclusions, exclusionString);
    }

    private static void SaveDefaults()
    {
        string defaultsString = string.Join("|", defaults.ToArray());
        EditorPrefs.SetString(KeyDefaults, defaultsString);

        cachedTypesDefaultYes.Clear();
        cachedTypesDefaultNo.Clear();
    }

    private static void LoadExclusions()
    {
        exclusions = new Dictionary<string, List<string>>();

        string rawExclusions = EditorPrefs.GetString(KeyExclusions);

        string[] allClassParts = rawExclusions.Split(new string[] { SeperatorClass }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string allClassPart in allClassParts)
        {
            string[] classParts = allClassPart.Split(new string[] { SeperatorPropertiesList }, StringSplitOptions.RemoveEmptyEntries);

            if (classParts.Length.Equals(2))
            {
                string className = classParts[0].Trim();

                if (!exclusions.ContainsKey(className))
                {
                    exclusions.Add(className, new List<string>());
                }

                string[] propertyParts = classParts[1].Split(new string[] { SeperatorProperty }, StringSplitOptions.RemoveEmptyEntries);

                foreach (string property in propertyParts)
                {
                    exclusions[className].Add(property.Trim());
                }
            }
        }
    }

    private static void LoadDefaults()
    {
        defaults = new List<string>();

        string rawDefaults = EditorPrefs.GetString(KeyDefaults);

        string[] classParts = rawDefaults.Split(new string[] { SeperatorClass }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string classPart in classParts)
        {
            defaults.Add(classPart.Trim());
        }
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

    private static void LoadSystemExclusions()
    {
        systemExclusions = new Dictionary<string, List<string>>();

        systemExclusions.Add(
            "UnityEngine.Transform",
            new List<string>() { 
                "position"
                ,"eulerAngles"
                ,"localEulerAngles"
                ,"right"
                ,"up"
                ,"forward"
                ,"rotation"
                ,"parent"
            });
        systemExclusions.Add(
            "UnityEngine.Renderer",
            new List<string>() {
                "lightmapIndex"
                ,"lightmapTilingOffset"
                ,"material"
                ,"materials"
            });
        systemExclusions.Add(
            "UnityEngine.MeshFilter",
            new List<string>() {
                "mesh"
            });
        systemExclusions.Add(
            "UnityEngine.Collider",
            new List<string>() {
                "material"
            });
        systemExclusions.Add(
            "UnityEngine.MonoBehaviour",
            new List<string>() {
                "useGUILayout"
            });
        systemExclusions.Add(
            "UnityEngine.ParticleEmitter",
            new List<string>() {
                "particles"
            });
        systemExclusions.Add(
            "UnityEngine.ParticleRenderer",
            new List<string>() {
                "animatedTextureCount"
                ,"uvTiles"
                ,"widthCurve"
                ,"heightCurve"
                ,"rotationCurve"
            });
        systemExclusions.Add(
            "UnityEngine.Rigidbody",
            new List<string>() {
                "velocity"
                ,"angularVelocity"
                ,"centerOfMass"
                ,"inertiaTensorRotation"
                ,"inertiaTensor"
                ,"useConeFriction"
                ,"position"
                ,"rotation"
                ,"solverIterationCount"
                ,"sleepVelocity"
                ,"sleepAngularVelocity"
                ,"maxAngularVelocity"
            });
        systemExclusions.Add(
            "UnityEngine.CharacterController",
            new List<string>() {
                "detectCollisions"
                ,"isTrigger"
                ,"sharedMaterial"
            });
        systemExclusions.Add(
            "UnityEngine.BoxCollider",
            new List<string>() {
                "extents"
            });
        systemExclusions.Add(
            "UnityEngine.MeshCollider",
            new List<string>() {
                "mesh"
            });
        systemExclusions.Add(
            "UnityEngine.WheelCollider",
            new List<string>() {
                "motorTorque"
                ,"brakeTorque"
                ,"steerAngle"
                ,"isTrigger"
                ,"sharedMaterial"
            });
        systemExclusions.Add(
            "UnityEngine.FixedJoint",
            new List<string>() {
                "axis"
                ,"anchor"
            });
        systemExclusions.Add(
            "UnityEngine.SpringJoint",
            new List<string>() {
                "axis"
            });
        systemExclusions.Add(
            "UnityEngine.CharacterJoint",
            new List<string>() {
                "targetRotation"
                ,"targetAngularVelocity"
                ,"rotationDrive"
            });
        systemExclusions.Add(
            "UnityEngine.AudioListener",
            new List<string>() {
                "volume"
                ,"pause"
                ,"velocityUpdateMode"
            });
        systemExclusions.Add(
            "UnityEngine.AudioSource",
            new List<string>() {
                "time"
                ,"timeSamples"
                ,"ignoreListenerVolume"
                ,"velocityUpdateMode"
                ,"minVolume"
                ,"maxVolume"
                ,"rolloffFactor"
            });
        systemExclusions.Add(
            "UnityEngine.AudioReverbFilter",
            new List<string>() {
                "roomRolloff"
            });
        systemExclusions.Add(
            "UnityEngine.Camera",
            new List<string>() {
                "fov"
                ,"near"
                ,"far"
                ,"aspect"
                ,"orthograhic"
                ,"pixelRect"
                ,"aspect"
                ,"worldToCameraMatrix"
                ,"projectionMatrix"
                ,"layerCullDistances"
                ,"depthTextureMode"
            });
        systemExclusions.Add(
            "UnityEngine.Projector",
            new List<string>() {
                "isOrthoGraphic"
                ,"orthoGraphicSize"
            });
        systemExclusions.Add(
            "UnityEngine.GUIText",
            new List<string>() {
                "material"
            });
        systemExclusions.Add(
            "UnityEngine.Animation",
            new List<string>() {
                "wrapMode"
            });
        systemExclusions.Add(
            "UnityEngine.Tree",
            new List<string>() {
                "data"
            });
    }
}