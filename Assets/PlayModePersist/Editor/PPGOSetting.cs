//AlmostLogical Software - http://www.almostlogical.com - support@almostlogical.com
using System.Collections.Generic;
using UnityEngine;

public class PPGOSetting
{
    private List<PPComponentSetting> componentSettings;

    public PPGOSetting(GameObject gameObj)
    {
        this.GameObj = gameObj;

        CreateComponentSettings();
    }

    public GameObject GameObj { get; set; }

    public bool Expanded { get; set; }

    public bool SaveAll { get; set; }

    public List<PPComponentSetting> ComponentSettings
    {
        get { return componentSettings; }
    }

    public void CreateComponentSettings()
    {
        componentSettings = new List<PPComponentSetting>();

        Component[] components = GameObj.GetComponents(typeof(Component));

        foreach (Component c in components)
        {
            PPComponentSetting setting = new PPComponentSetting(c);

            if (c != null)
            {
                if (PPLocalStorageManager.IsTypeDefaulted(c.GetType()))
                {
                    setting.IsSavingSettings = true;
                }
            }

            componentSettings.Add(setting);
        }
    }

    public void StoreAllSelectedSettings()
    {
        componentSettings.ForEach(setting => setting.StoreSettings());
    }
    public void RestoreAllSelectedSettings()
    {
        componentSettings.ForEach(setting => setting.RestoreSettings());
    }
}