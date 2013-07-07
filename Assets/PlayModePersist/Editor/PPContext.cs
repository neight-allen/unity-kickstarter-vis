//AlmostLogical Software - http://www.almostlogical.com - support@almostlogical.com
using System.Collections.Generic;
using UnityEngine;

public class PPCurrentContext
{
    private static Dictionary<GameObject, PPGOSetting> persistentSettings = new Dictionary<GameObject, PPGOSetting>();

    public GameObject GameObj { get; private set; }

    public PPGOSetting GameObjectSetting { get; private set; }

    private PPGOSetting GetStoredGameObjectSetting()
    {
        PPGOSetting setting = null;

        if (persistentSettings.ContainsKey(GameObj))
        {
            setting = persistentSettings[GameObj];
        }
        else
        {
            setting = new PPGOSetting(GameObj);
            persistentSettings.Add(GameObj, setting);
        }

        return setting;
    }

    public void SetContext(Object target)
    {
        GameObj = ((Transform)target).gameObject;

        if (null == GameObjectSetting)
        {
            GameObjectSetting = GetStoredGameObjectSetting();
        }
    }
}