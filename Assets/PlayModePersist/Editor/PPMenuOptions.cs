using UnityEditor;
using UnityEngine;
class PPMenuOptions : MonoBehaviour
{
    [MenuItem("Edit/PlayModePersist - Persist Selected GameObject(s) &#p")]
    static void PersistSelectedGameObject()
    {
        if (PPEditorWindow.currentWindow!=null)
        {
            PPEditorWindow.PersistSelectedGameObject();
            PPEditorWindow.currentWindow.Repaint();
        }
    }

    [MenuItem("Edit/PlayModePersist - Persist Selected GameObject(s) &#p",true)]
    static bool ValidatePersistSelectedGameObject()
    {
        return (EditorApplication.isPlaying || EditorApplication.isPaused) && Selection.activeTransform != null && PPEditorWindow.currentWindow!=null;
    }

    /*
     * Experimenting
    [MenuItem("Edit/PlayModePersist - Persist All Rigidbody Positions On Stop &#r")]
    static void PersistAllRigidbodyPositions()
    {
        if (Selection.objects.Length == 0)
        {
            Object obj = ((Transform)GameObject.FindObjectOfType(typeof(Transform))).gameObject;

            if (obj != null)
            {
                Object[] objs = new Object[1];
                objs[0] = obj;
                Selection.objects = objs;
            }
        }
        //NEED TO FIX
        //PPInspector.PersistAllRigidbodyPositions();
    }

    // Add menu named "Do Something" to the main menu
    [MenuItem("Edit/PlayModePersist - Persist All Rigidbody Positions On Stop &#r", true)]
    static bool ValidatePersistAllRigidbodyPositions()
    {
        return (EditorApplication.isPlaying || EditorApplication.isPaused);
    }
     * */
}