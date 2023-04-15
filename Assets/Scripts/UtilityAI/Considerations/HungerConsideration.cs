using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.Core;
using TL.UtilityAI;

namespace TL.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName = "HungerConsideration", menuName = "UtilityAI/Considerations/Hunger Consideration")]
    public class HungerConsideration : Consideration
    {
        [SerializeField] private AnimationCurve responseCurve;

        public override float ScoreConsideration(NPCController npc)
        {
            //score = responseCurve.Evaluate(Mathf.Clamp01(npc.stats.hunger/100.0f));
            //score = responseCurve.Evaluate(Mathf.Clamp01(50f/100f));
            score = 0.5f;
            return score;
        }
    }
}
