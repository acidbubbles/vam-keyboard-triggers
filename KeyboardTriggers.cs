using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keyboard Triggers 1.0.0, by Acid Bubbles
/// Executes triggers based on key presses
/// </summary>
public class KeyboardTriggers : MVRScript
{
    private JSONStorableStringChooser _actionKey1JSON;
    private KeyCode _actionKey1;
    private JSONStorableAction _action1JSON;

    private JSONStorableStringChooser _actionKey2JSON;
    private KeyCode _actionKey2;
    private JSONStorableAction _action2JSON;

    private JSONStorableStringChooser _actionKey3JSON;
    private KeyCode _actionKey3;
    private JSONStorableAction _action3JSON;

    private JSONStorableStringChooser _actionKey4JSON;
    private KeyCode _actionKey4;
    private JSONStorableAction _action4JSON;

    private JSONStorableStringChooser _actionKey5JSON;
    private KeyCode _actionKey5;
    private JSONStorableAction _action5JSON;

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
            RegisterKey(keys, ref _actionKey2JSON, v => _actionKey1 = v, ref _action1JSON, "1");
            RegisterKey(keys, ref _actionKey2JSON, v => _actionKey2 = v, ref _action2JSON, "2");
            RegisterKey(keys, ref _actionKey3JSON, v => _actionKey3 = v, ref _action3JSON, "3");
            RegisterKey(keys, ref _actionKey4JSON, v => _actionKey4 = v, ref _action4JSON, "4");
            RegisterKey(keys, ref _actionKey5JSON, v => _actionKey5 = v, ref _action5JSON, "5");
        }
        catch (Exception e)
        {
            SuperController.LogError("Failed to init controls: " + e);
        }
    }

    private void RegisterKey(List<string> keys, ref JSONStorableStringChooser actionKeyJSON, Action<KeyCode> actionKey, ref JSONStorableAction actionJSON, string index)
    {
        actionKeyJSON = new JSONStorableStringChooser($"Action Key {index}", keys, "None", $"Action Key {index}", new JSONStorableStringChooser.SetStringCallback(v => actionKey(ApplyToggleKey(v))));
        RegisterStringChooser(actionKeyJSON);
        var toggleKeyPopup = CreateScrollablePopup(actionKeyJSON);
        toggleKeyPopup.popupPanelHeight = 600f;

        actionJSON = new JSONStorableAction($"Action {index}", new JSONStorableAction.ActionCallback(() => SuperController.LogMessage($"Action {index} triggered")));
        RegisterAction(actionJSON);
    }

    private KeyCode ApplyToggleKey(string val)
    {
        return (KeyCode)Enum.Parse(typeof(KeyCode), val);
    }

    public void Update()
    {
        Handle(_actionKey1, _action1JSON);
        Handle(_actionKey2, _action2JSON);
        Handle(_actionKey3, _action3JSON);
        Handle(_actionKey4, _action4JSON);
        Handle(_actionKey5, _action5JSON);
    }

    private void Handle(KeyCode actionKey, JSONStorableAction actionJSON)
    {
        if (actionKey != KeyCode.None && Input.GetKeyDown(actionKey))
        {
            actionJSON.actionCallback();
            return;
        }
    }
}

