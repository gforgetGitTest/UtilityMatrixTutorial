using System.Collections;
using System.Collections.Generic;
using TL.Core;
using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour
{
    public Transform InteractPosition;
    public Transform WaitingArea;
    public float TimeToServe;

    private NPCController usingNPC;
    public abstract bool ValidateRequirement(NPCController npc); 
    public bool RequestInteraction(NPCController npc) 
    {
        if (usingNPC == null || usingNPC == npc)
        {
            usingNPC = npc;
            npc.usingInteractiveObject = this;
            return true;
        }
        else 
        { 
            return false;
        }
    }

    public void Interact(NPCController npc) 
    {
        StartCoroutine(InteractingTimer(npc));
    }
    
    public void ReleaseInteractiveObject(NPCController npc) 
    {
        if (npc == usingNPC) 
        {
            usingNPC = null;
        }
    }
    public bool IsUsed(NPCController npc) 
    { 
        return usingNPC != null && usingNPC != npc;
    }

    protected IEnumerator InteractingTimer(NPCController npc) 
    {
        yield return new WaitForSeconds(TimeToServe);
        ResultAction(npc);
    }

    protected abstract void ResultAction(NPCController npc);
}
