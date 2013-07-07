// AlmostLogical Software - http://www.almostlogical.com - support@almostlogical.com (auto opens, delete file if you want to disable auto open)
using UnityEngine;
using UnityEditor;

//comment out InitializeOnLoad will disable PlayModePersist auto open at start
[InitializeOnLoad]
public class PPAutoOpen
{
    static PPAutoOpen()
    {
        #if (UNITY_2_6 || UNITY_2_6_1 || UNITY_3_0 || UNITY_3_0_0 || UNITY_3_1 || UNITY_3_2 || UNITY_3_3 || UNITY_3_4)
        #else
            if (PPEditorWindow.currentWindow == null)
            {
                EditorApplication.update += AutoInit;
            }
        #endif
    }

    static void AutoInit()
    {
        EditorApplication.update -= AutoInit;
        PPEditorWindow.AutoInit();
    }
}