using System.Collections;
using System.Collections.Generic;
using TL.Core;
using UnityEngine;

public abstract class InteractiveObject : MonoBehaviour
{
    public Transform InteractPosition;
    public Transform WaitingArea;
    public float TimeToServe;

    public Queue<NPCController> NPCQueue = new Queue<NPCController>();

    public abstract bool ValidateRequirement(NPCController npc); 
    public bool RequestInteraction(NPCController npc) 
    {
        if (!NPCQueue.Contains(npc)) 
        {
            NPCQueue.Enqueue(npc);
        }
        
        if (NPCQueue.Peek() == npc)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void Interact(NPCController npc) 
    {
        NPCQueue.Dequeue();
        StartCoroutine(InteractingTimer(npc));
    }

    protected IEnumerator InteractingTimer(NPCController npc) 
    {
        yield return new WaitForSeconds(TimeToServe);
        ResultAction(npc);
    }

    protected abstract void ResultAction(NPCController npc);
}
