using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Keyboard Triggers 1.0.0, by Acid Bubbles
/// Executes triggers based on key presses
/// </summary>
public class KeyboardTriggers : MVRScript
{

    private JSONStorableStringChooser _actionKeyJSON;
    private KeyCode _actionKey;
    private JSONStorableStringChooser _atomJSON;
    private Atom _receivingAtom;
    private JSONStorableStringChooser _receiverJSON;
    private JSONStorable _receiver;
    private JSONStorableStringChooser _receiverTargetJSON;
    private string _missingReceiverStoreId = "";
    private string _receiverTargetName;
    private JSONStorableFloat _currentFloatJSON;
    private JSONStorableFloat _targetFloatJSON;
    private JSONStorableBool _currentBoolJSON;
    private JSONStorableFloat _receiverFloatTarget;
    private JSONStorableBool _receiverBoolTarget;

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

            _atomJSON = new JSONStorableStringChooser("atom", SuperController.singleton.GetAtomUIDs(), null, "Atom", SyncAtom);
            RegisterStringChooser(_atomJSON);
            SyncAtomChoices();
            UIDynamicPopup dp = CreateScrollablePopup(_atomJSON);
            dp.popupPanelHeight = 1100f;
            dp.popup.onOpenPopupHandlers += SyncAtomChoices;

            _receiverJSON = new JSONStorableStringChooser("receiver", null, null, "Receiver", SyncReceiver);
            RegisterStringChooser(_receiverJSON);
            dp = CreateScrollablePopup(_receiverJSON);
            dp.popupPanelHeight = 960f;

            _receiverTargetJSON = new JSONStorableStringChooser("receiverTarget", null, null, "Target", SyncReceiverTarget);
            RegisterStringChooser(_receiverTargetJSON);
            dp = CreateScrollablePopup(_receiverTargetJSON);
            dp.popupPanelHeight = 820f;

            _atomJSON.val = containingAtom.uid;

            _targetFloatJSON = new JSONStorableFloat("targetFloat", 0f, 0f, 1f, false);
            RegisterFloat(_targetFloatJSON);
            var targetFloatSlider = CreateSlider(_targetFloatJSON, true);
            targetFloatSlider.label = "Float Value (Target)";

            _currentFloatJSON = new JSONStorableFloat("currentFloat", 0f, 0f, 1f, false, false);
            var currentFloatSlider = CreateSlider(_currentFloatJSON, true);
            currentFloatSlider.label = "Float Value (Current)";
            currentFloatSlider.defaultButtonEnabled = false;
            currentFloatSlider.quickButtonsEnabled = false;

            _currentBoolJSON = new JSONStorableBool("currentBool", false);
            var currentBoolToggle = CreateToggle(_currentBoolJSON, true);
            currentBoolToggle.label = "Bool Value (Current)";
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
        CheckMissingReceiver();

        if (_receiverFloatTarget != null)
        {
            if (_actionKey != KeyCode.None && Input.GetKeyDown(_actionKey))
            {
                _receiverFloatTarget.val = _targetFloatJSON.val;
            }
            else
            {
                _currentFloatJSON.val = _receiverFloatTarget.val;
            }
        }
        else
        {
            _currentFloatJSON.val = 0f;
        }

        if (_receiverBoolTarget != null)
        {
            if (_actionKey != KeyCode.None && Input.GetKeyDown(_actionKey))
            {
                _receiverBoolTarget.val = !_receiverBoolTarget.val;
            }
            else
            {
                _currentBoolJSON.val = _receiverBoolTarget.val;
            }
        }
        else
        {
            _currentBoolJSON.val = false;
        }
    }

    protected void SyncAtomChoices()
    {
        var atomChoices = new List<string>();
        atomChoices.Add("None");
        foreach (string atomUID in SuperController.singleton.GetAtomUIDs())
        {
            atomChoices.Add(atomUID);
        }
        _atomJSON.choices = atomChoices;
    }

    protected void SyncAtom(string atomUID)
    {
        var receiverChoices = new List<string>();
        receiverChoices.Add("None");
        if (atomUID != null)
        {
            _receivingAtom = SuperController.singleton.GetAtomByUid(atomUID);
            if (_receivingAtom != null)
            {
                foreach (string receiverChoice in _receivingAtom.GetStorableIDs())
                {
                    receiverChoices.Add(receiverChoice);
                }
            }
        }
        else
        {
            _receivingAtom = null;
        }
        _receiverJSON.choices = receiverChoices;
        _receiverJSON.val = "None";
    }

    protected void CheckMissingReceiver()
    {
        if (_missingReceiverStoreId == "" || _receivingAtom == null)
            return;

        var missingReceiver = _receivingAtom.GetStorableByID(_missingReceiverStoreId);
        if (missingReceiver == null)
            return;

        var saveTargetName = _receiverTargetName;
        SyncReceiver(_missingReceiverStoreId);
        _missingReceiverStoreId = "";
        insideRestore = true;
        _receiverTargetJSON.val = saveTargetName;
        insideRestore = false;
    }

    protected void SyncReceiver(string receiverID)
    {
        var receiverTargetChoices = new List<string>();
        receiverTargetChoices.Add("None");
        if (_receivingAtom != null && receiverID != null)
        {
            _receiver = _receivingAtom.GetStorableByID(receiverID);
            if (_receiver != null)
            {
                foreach (string floatParam in _receiver.GetFloatParamNames())
                {
                    receiverTargetChoices.Add(floatParam);
                }
                foreach (string boolParam in _receiver.GetBoolParamNames())
                {
                    receiverTargetChoices.Add(boolParam);
                }
            }
            else if (receiverID != "None")
            {
                _missingReceiverStoreId = receiverID;
            }
        }
        else
        {
            _receiver = null;
        }
        _receiverTargetJSON.choices = receiverTargetChoices;
        _receiverTargetJSON.val = "None";
    }

    protected void SyncReceiverTarget(string receiverTargetName)
    {
        _receiverTargetName = receiverTargetName;
        _receiverFloatTarget = null;
        if (_receiver == null || receiverTargetName == null)
            return;

        _receiverFloatTarget = _receiver.GetFloatJSONParam(receiverTargetName);
        if (_receiverFloatTarget != null)
        {
            _targetFloatJSON.min = _receiverFloatTarget.min;
            _targetFloatJSON.max = _receiverFloatTarget.max;
            _currentFloatJSON.min = _receiverFloatTarget.min;
            _currentFloatJSON.max = _receiverFloatTarget.max;
            if (!insideRestore)
            {
                _targetFloatJSON.val = _receiverFloatTarget.val;
                _currentFloatJSON.val = _receiverFloatTarget.val;
            }
            return;
        }

        _receiverBoolTarget = _receiver.GetBoolJSONParam(receiverTargetName);
        if (_receiverBoolTarget != null)
        {
            if (!insideRestore)
            {
                _currentBoolJSON.val = _receiverBoolTarget.val;
            }
        }
    }
}

