using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSwapper : MonoBehaviour
{
    [SerializeField] List<MeshRenderer> m_targets = default;
    [SerializeField] List<FactionMaterial> m_factionMaterials = default;
    Dictionary<Faction, Material> m_materialsMap;
    [SerializeField] Material m_defaultMaterial = default;

    private void Awake()
    {
        m_materialsMap = new Dictionary<Faction, Material>();
        if (m_targets == null || m_targets.Count == 0)
        {
            Debug.LogWarning("No targets set.");
            return;
        }

        if(m_factionMaterials == null || m_factionMaterials.Count == 0)
        {
            Debug.LogWarning("No factionMaterials set.");
            return;
        }

        if(m_defaultMaterial == null)
        {
            Debug.LogWarning("Default material not set.");
        }
        

        foreach(FactionMaterial pair in m_factionMaterials)
        {
            if(m_materialsMap.ContainsKey(pair.Faction))
            {
                Debug.LogWarning($"Duplicate factionMaterial for faction {pair.Faction}");
                continue;
            }

            if(pair.Material == null)
            {
                Debug.LogError($"Material for faction {pair.Faction} is null.");
                continue;
            }

            m_materialsMap[pair.Faction] = pair.Material;
        }
    }

    public void SwapMaterial(Faction faction)
    {
        if(!m_materialsMap.ContainsKey(faction))
        {
            Debug.LogWarning($"No material set for faction {faction}. Using default material.");
            SetMaterials(m_defaultMaterial);
            return;
        }

        SetMaterials(m_materialsMap[faction]);
    }
    
    void SetMaterials(Material material)
    {
        foreach(MeshRenderer mr in m_targets)
        {
            mr.material = material;
        }
    }
}

[CreateAssetMenu(fileName = "new Faction Material", menuName = "Units/FactionMaterial", order = 102)]
public class FactionMaterial : ScriptableObject
{
    public Faction Faction;
    public Material Material;
}
