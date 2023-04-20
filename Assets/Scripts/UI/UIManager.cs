using System.Collections;
using System.Collections.Generic;
using TL.Core;
using TMPro;
using UnityEngine;
using UnityEngine.Networking.Types;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField]
    private Camera mainCamera;

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

    private void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            RaycastHit hit;
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);

            //Get the first object using the character layer (3)
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, 1<<3))
            {
                SelectNPC(hit.transform.parent.GetComponent<NPCController>());
            }
        }
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
        selectedNPC.OnNPCDeath += OnNPCDeath;
        selectedNPC.OnPropertyChange += UpdateUI;

        UpdateUI(npc);
    }

    private void OnNPCDeath(NPCController npc) 
    {
        if (selectedNPC == npc)
        {
            NPCId.text = "NPC Id : ";
            foodText.text = "Food -/100";
            woodText.text = "Wood -/100";
            moneyText.text = "Money -/1000";

            energyText.text = "Energy -/100";
            hungerText.text = "Hunger -/100";

            selectedNPC.OnPropertyChange -= UpdateUI;
            selectedNPC.OnNPCDeath -= OnNPCDeath;
            selectedNPC = null;
        }
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
        if (selectedNPC != null) 
        {
            selectedNPC.OnPropertyChange -= UpdateUI;
        }
    }
}
