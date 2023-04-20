using System.Collections;
using System.Collections.Generic;
using TL.Core;
using UnityEngine;

public class WoodReserve : InteractiveObject
{
    public int takenWood = 1;
    public int addedMoney = 2;
    public override bool ValidateRequirement(NPCController npc)
    {
        return npc.inventory.wood >= takenWood && npc.stats.energy > 0;
    }

    protected override void ResultAction(NPCController npc)
    {
        npc.AddWood(takenWood * -1);
        npc.AddMoney(addedMoney);
    }
}
