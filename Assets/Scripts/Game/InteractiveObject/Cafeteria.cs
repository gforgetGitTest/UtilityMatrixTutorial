using System.Collections;
using System.Collections.Generic;
using TL.Core;
using UnityEngine;

public class Cafeteria : InteractiveObject
{
    public int takenMoney = 2;
    public int addedFood = 1;
    public override bool ValidateRequirement(NPCController npc)
    {
        return npc.inventory.money >= takenMoney;
    }

    protected override void ResultAction(NPCController npc)
    {
        npc.AddMoney(takenMoney*-1);
        npc.AddFood(addedFood);
    }
}
