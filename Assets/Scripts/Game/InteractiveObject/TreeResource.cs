using System.Collections;
using System.Collections.Generic;
using TL.Core;
using UnityEngine;

public class TreeResource : InteractiveObject
{
    public int takenEnergy = 10;
    public int addedTree = 5;

    public override bool ValidateRequirement(NPCController npc)
    {
        return npc.stats.energy >= takenEnergy;
    }

    protected override void ResultAction(NPCController npc)
    {
        npc.AddEnergy(takenEnergy * -1);
        npc.AddWood(addedTree);
    }
}
