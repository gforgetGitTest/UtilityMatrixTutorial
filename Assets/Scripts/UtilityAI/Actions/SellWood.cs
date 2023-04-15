using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.UtilityAI;
using TL.Core;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "SellWood", menuName = "UtilityAI/Actions/SellWood")]
    public class SellWood : Action
    {
        public override void Execute(NPCController npc)
        {
            npc.DoSellWood();
        }
    }
}
