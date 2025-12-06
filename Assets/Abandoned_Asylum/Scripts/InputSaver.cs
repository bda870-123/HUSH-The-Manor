using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputSaver : MonoBehaviour
{
    private const string REBINDS_KEY = "rebinds";

    public static void Save(InputActionAsset actions)
    {
        string rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString(REBINDS_KEY, rebinds);
        PlayerPrefs.Save();
    }

    public static void Load(InputActionAsset actions)
    {
        string rebinds = PlayerPrefs.GetString(REBINDS_KEY);
        if (!string.IsNullOrEmpty(rebinds))
        {
            actions.LoadBindingOverridesFromJson(rebinds);
        }
    }
}
