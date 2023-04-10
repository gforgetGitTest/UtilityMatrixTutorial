using System.Collections;
using System.Collections.Generic;
using TL.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Networking.Types;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [HideInInspector]
    public NPCController selectedNPC;

    [Header("General")]
    public TextMeshProUGUI NPCId;

    [Header("Inventory")]
    public TextMeshProUGUI foodText;
    public TextMeshProUGUI woodText;
    public TextMeshProUGUI moneyText;

    [Header("Stats")]
    public TextMeshProUGUI energyText;
    public TextMeshProUGUI hungerText;

    [Header("Material")]
    public Material unselectedMaterial;
    public Material selectedMaterial;

    private void Awake()
    {
        Instance = this;
    }

    IEnumerator Start() 
    {
        while (NPCManager.Instance == null) 
        { 
            yield return null;
        }

        SelectNPC(NPCManager.Instance.npcs[0]);
    }

    public void SelectNPC(NPCController npc) 
    {
        for (int i=0; i< NPCManager.Instance.npcs.Length; i++) 
        {
            if (NPCManager.Instance.npcs[i] == npc)
            {
                NPCManager.Instance.npcs[i].ChangeNPCColor(selectedMaterial);
            }
            else 
            {
                NPCManager.Instance.npcs[i].ChangeNPCColor(unselectedMaterial);
            }
        }

        selectedNPC = npc;
        UpdateUI(npc);
    }

    public void UpdateUI(NPCController npc) 
    {
        if (selectedNPC == npc) 
        {
            NPCId.text = "NPC Id : " + selectedNPC.transform.name;
            foodText.text = "Food " + selectedNPC.inventory.food + "/100";
            woodText.text = "Wood " + selectedNPC.inventory.wood + "/100";
            moneyText.text = "Money " + selectedNPC.inventory.money + "/1000";

            energyText.text = "Energy " + selectedNPC.stats.energy + "/100";
            hungerText.text = "Hunger " + selectedNPC.stats.hunger + "/100";
        }
    }
}
