using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class WizardCreateAbility : ScriptableWizard
{
    [MenuItem("Tools/Create Ability")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<WizardCreateAbility>("Create Unit Ability", "Create", "Cancel");
    }

    public static WizardCreateAbility GetWizard()
    {
        return ScriptableWizard.DisplayWizard<WizardCreateAbility>("Create Unit Ability", "Create", "Cancel");
    }

    AbilityPrototype ability;
    Editor editor;

    public event System.Action<WizardCreateAbility, AbilityPrototype> AbilityCreatedEvent;
    public event System.Action<WizardCreateAbility> WizardClosedEvent;

    private void OnWizardCreate()
    {
        AbilityCreatedEvent?.Invoke(this, ability);
    }

    private void OnWizardOtherButton()
    {
        WizardClosedEvent?.Invoke(this);
    }

    private void OnWizardUpdate()
    {
        if(ability == null || ability.Name == null || ability.Name == "")
        {
            isValid = false;
        }
        else
        {
            isValid = true;
        }
    }

    protected override bool DrawWizardGUI()
    {
        bool returnValue = base.DrawWizardGUI();
        if (ability == null)
        {
            DrawAbilityTypeButtons();
            return returnValue;
        }

        editor.OnInspectorGUI();
        return returnValue;
    }

    void DrawAbilityTypeButtons()
    {
        List<string> types = AbilityPrototypeLookup.GetAbilityTypes();

        if(types == null || types.Count == 0)
        {
            errorString = "no ability types found.";
            return;
        }

        foreach(string type in types)
        {
            if(GUILayout.Button(type))
            {
                ability = AbilityPrototypeLookup.GetAbilityOfType(type);
                editor = Editor.CreateEditor(ability);
            }
        }
    }
}
