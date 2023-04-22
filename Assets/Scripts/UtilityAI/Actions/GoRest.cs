using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.UtilityAI;
using TL.Core;

[CreateAssetMenu(fileName = "GoRest", menuName = "UtilityAI/Actions/GoRest")]
public class GoRest : Action
{
    public override void Execute(NPCController npc)
    {
        npc.DoGoRest();
    }
}

