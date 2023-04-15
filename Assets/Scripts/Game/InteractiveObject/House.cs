using System.Collections;
using System.Collections.Generic;
using TL.Core;
using UnityEngine;

public class House : InteractiveObject
{
    public int addedEnergy = 5;
    public override bool ValidateRequirement(NPCController npc)
    {
        return true;
    }

    protected override void ResultAction(NPCController npc)
    {
        npc.AddEnergy(addedEnergy);
    }
}
