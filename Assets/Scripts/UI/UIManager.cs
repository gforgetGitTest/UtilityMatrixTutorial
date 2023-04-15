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
        while (GameManager.Instance == null) 
        { 
            yield return null;
        }

        SelectNPC(GameManager.Instance.NPCs[0]);
    }

    public void SelectNPC(NPCController npc) 
    {
        for (int i=0; i< GameManager.Instance.NPCs.Length; i++) 
        {
            if (GameManager.Instance.NPCs[i] == npc)
            {
                GameManager.Instance.NPCs[i].ChangeNPCColor(selectedMaterial);
            }
            else 
            {
                GameManager.Instance.NPCs[i].ChangeNPCColor(unselectedMaterial);
            }
        }

        if (selectedNPC != null) selectedNPC.OnPropertyChange -= UpdateUI;
        selectedNPC = npc;
        selectedNPC.OnPropertyChange += UpdateUI;
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

    private void OnDestroy()
    {
        if (selectedNPC != null) selectedNPC.OnPropertyChange -= UpdateUI;
    }
}
