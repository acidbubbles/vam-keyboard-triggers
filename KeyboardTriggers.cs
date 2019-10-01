using System;
using UnityEngine;

/// <summary>
/// Keyboard Triggers 1.0.0, by Acid Bubbles
/// Executes triggers based on key presses
/// </summary>
public class KeyboardTriggers : MVRScript
{
    private JSONStorableStringChooser _actionKeyJSON;
    private KeyCode _actionKey;


    public override void Init()
    {
        try
        {
            InitControls();
        }
        catch (Exception e)
        {
            SuperController.LogError("Failed to init: " + e);
            DestroyImmediate(this);
        }
    }

    private void InitControls()
    {
        try
        {
            var keys = Enum.GetNames(typeof(KeyCode)).ToList();
            _actionKeyJSON = new JSONStorableStringChooser("Key", keys, "None", "Key", new JSONStorableStringChooser.SetStringCallback(v => _actionKey = ApplyToggleKey(v)));
            RegisterStringChooser(_actionKeyJSON);
            var toggleKeyPopup = CreateScrollablePopup(_actionKeyJSON);
            toggleKeyPopup.popupPanelHeight = 600f;
            _actionKey = ApplyToggleKey(_actionKeyJSON.val);

        }
        catch (Exception e)
        {
            SuperController.LogError("Failed to init controls: " + e);
        }
    }


    private KeyCode ApplyToggleKey(string val)
    {
        return string.IsNullOrEmpty(val) ? KeyCode.None : (KeyCode)Enum.Parse(typeof(KeyCode), val);
    }

    public void Update()
    {
        if (_actionKey != KeyCode.None && Input.GetKeyDown(_actionKey))
        {
            return;
        }
    }
}

