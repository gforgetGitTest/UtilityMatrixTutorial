using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.Core;
using TL.UtilityAI;

namespace TL.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName ="WoodConsideration", menuName = "UtilityAI/Considerations/Wood Consideration")]
    public class WoodConsideration : Consideration
    {
        [SerializeField] private AnimationCurve responseCurve;
        public override float ScoreConsideration(NPCController npc)
        {
            score = responseCurve.Evaluate(Mathf.Clamp01((float)npc.inventory.wood / 100.0f));
            return score;
        }
    }
}

