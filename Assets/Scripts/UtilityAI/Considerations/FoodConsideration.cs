using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.Core;
using TL.UtilityAI;

namespace TL.UtilityAI.Considerations
{
    [CreateAssetMenu(fileName ="FoodConsideration", menuName = "UtilityAI/Considerations/Food Consideration")]
    public class FoodConsideration : Consideration
    {
        [SerializeField] private AnimationCurve responseCurve;
        public override float ScoreConsideration(NPCController npc)
        {
            score = responseCurve.Evaluate(Mathf.Clamp01((float)npc.inventory.food / 100.0f));
            return score;
        }
    }
}

