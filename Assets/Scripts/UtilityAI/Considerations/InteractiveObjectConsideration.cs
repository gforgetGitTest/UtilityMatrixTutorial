using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.Core;
using TL.UtilityAI;

namespace TL.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "InteractiveObjectConsideration", menuName = "UtilityAI/Considerations/Interactive Object Consideration")]
    public class InteractiveObjectConsideration : Consideration
    {
       [SerializeField] private GlobalEnum.InteractiveObject InteractiveObject;
        public override float ScoreConsideration(NPCController npc)
        {
            score = GameManager.Instance.InteractiveObjects[InteractiveObject].IsUsed(npc) ? 0 : 1;
            return score;
        }
    }
}
