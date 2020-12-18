using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class WizardCreateUnit : ScriptableWizard
{
    [MenuItem("Tools/Create Unit")]
    static void CreateWizard()
    {
        ScriptableWizard.DisplayWizard<WizardCreateUnit>("Create Unit Prototype", "Create", "Cancel");
    }
    
    public enum UnitType { Player, Enemy, Boss }
    public UnitType unitType;

    string GetTypeString()
    {
        switch (unitType)
        {
            case UnitType.Player:
                return "Player";

            case UnitType.Enemy:
                return "Enemy";

            case UnitType.Boss:
                return "Boss";

        }

        return "error";
    }

    public string Type;
    public Sprite Portrait;
    public int RecruitCost;
    public int MaxHealth;
    public int MaxMP;
    public float TimeBetweenActions;
    public int Difficulty;
    public bool Flying;
    public float MoveSpeed;
    public GameObject Prefab;

    [Header("Abilities")]
    public List<AbilityPrototype> Abilities;


    private void OnWizardCreate()
    {
        UnitPrototype unit = ScriptableObject.CreateInstance<UnitPrototype>();
        string prototypePath = Path.Combine("Assets", "Game", "Units", "_Prototypes", GetTypeString());
        AssetDatabase.CreateAsset(unit, Path.Combine(prototypePath, Type + ".asset"));
        UnitPrototypeDB.AddPrototype(unit);

        unit.Type = Type;
        string unitDirectory = Path.Combine("Assets", "Game", "Units", "Data", GetTypeString(), Type);
        
        Directory.CreateDirectory(unitDirectory);
        string abilityPath = Path.Combine(unitDirectory, "Abilities"); 
        Directory.CreateDirectory(abilityPath);
        string vfxPath = Path.Combine(unitDirectory, "VFX");
        Directory.CreateDirectory(vfxPath);
        string animationPath = Path.Combine(unitDirectory, "Animation");
        Directory.CreateDirectory(animationPath);
        string conditionsPath = Path.Combine(unitDirectory, "Conditions");
        Directory.CreateDirectory(conditionsPath);

        unit.Portrait = Portrait;
        if(Portrait != null)
        {
            AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(Portrait), unitDirectory);
        }

           
        unit.RecruitCost = RecruitCost;
        unit.MaxHealth = MaxHealth;
        unit.MaxMP = MaxMP;
        unit.TimeBetweenActions = TimeBetweenActions;
        unit.Difficulty = Difficulty;
        unit.Flying = Flying;
        unit.MoveSpeed = MoveSpeed;

        unit.Prefab = Prefab;
        if(Prefab != null)
        {
            AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(Prefab), unitDirectory);
        }


        unit.BaseAbilities = Abilities;
        foreach(AbilityPrototype ability in Abilities)
        {
            AssetDatabase.CreateAsset(ability, Path.Combine(abilityPath, ability.Name + ".asset"));
            if(ability.VFX != null)
            {
                foreach(GameObject vfx in ability.VFX)
                {
                    AssetDatabase.MoveAsset(AssetDatabase.GetAssetPath(vfx), vfxPath);
                }
            }
        }


        AssetDatabase.Refresh();
    }

    private void OnWizardOtherButton()
    {
        
    }

    private void OnWizardUpdate()
    {
        if(Type == null || 
            Type == "" || 
            Directory.Exists(Path.Combine(Application.dataPath, "Game", "Units", "Data", GetTypeString(), Type)))
        {
            isValid = false;
            return;
        }

        isValid = true;
    }

    protected override bool DrawWizardGUI()
    {
        bool returnValue = base.DrawWizardGUI();

        if(GUILayout.Button("Add Ability"))
        {
            WizardCreateAbility wizard = WizardCreateAbility.GetWizard();
            SubscribeToAbilityWizard(wizard);
        }

        return returnValue;
    }

    void OnAbilityAdded(WizardCreateAbility wizard, AbilityPrototype ability)
    {
        UnsubscribeFromWizard(wizard);
        Abilities.Add(ability);
    }

    void OnAbilityWizardClosed(WizardCreateAbility wizard)
    {
        UnsubscribeFromWizard(wizard);
    }

    void SubscribeToAbilityWizard(WizardCreateAbility wizard)
    {
        wizard.AbilityCreatedEvent += OnAbilityAdded;
        wizard.WizardClosedEvent += OnAbilityWizardClosed;
    }

    void UnsubscribeFromWizard(WizardCreateAbility wizard)
    {
        wizard.AbilityCreatedEvent -= OnAbilityAdded;
        wizard.WizardClosedEvent -= OnAbilityWizardClosed;
    }
}
