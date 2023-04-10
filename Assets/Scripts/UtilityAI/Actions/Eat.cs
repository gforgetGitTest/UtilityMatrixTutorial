using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.UtilityAI;
using TL.Core;

namespace TL.UtilityAI.Actions
{
    [CreateAssetMenu(fileName = "Eat", menuName = "UtilityAI/Actions/Eat")]
    public class Eat : Action
    {
        public override void Execute(NPCController npc)
        {
            Debug.Log("I ate food!");
            // Logic for updating everything involve with eating

            // Decide our new best action after you finished this one
            npc.OnFinishedAction();
        }
    }
}
