using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.UtilityAI;
using TL.Core;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "BuyFood", menuName = "UtilityAI/Actions/BuyFood")]
    public class BuyFood : Action
    {
        public override void Execute(NPCController npc)
        {
            npc.DoBuyFood();
        }
    }
}
