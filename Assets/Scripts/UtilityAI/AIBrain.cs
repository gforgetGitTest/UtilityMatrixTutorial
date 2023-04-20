using System.Collections;
using System.Collections.Generic;
using TL.UtilityAI;
using UnityEngine;

namespace TL.Core
{
    public class AIBrain : MonoBehaviour
    {
        public bool finishedDeciding { get; set; }
        public Action bestAction { get; set; }
        [SerializeField]private NPCController npc;

        //Loop through all the available actions
        // Give me the highest scoring action
        public Action DecideBestAction(Action[] actionsAvailable) 
        {
            float score = 0f;
            int nextBestActionIndex = 0;
            
            for (int i = 0; i < actionsAvailable.Length; i++) 
            {
                if (ScoreAction(actionsAvailable[i]) > score) 
                {
                    nextBestActionIndex = i;
                    score = actionsAvailable[i].score;
                }
            }

            bestAction = actionsAvailable[nextBestActionIndex];
            finishedDeciding = true;

            return bestAction; 
        }

        // Return the best action without actually picking it
        public Action LookUpBestAction(Action[] actionsAvailable) 
        {
            float score = 0f;
            int nextBestActionIndex = 0;

            for (int i = 0; i < actionsAvailable.Length; i++)
            {
                if (ScoreAction(actionsAvailable[i]) > score)
                {
                    nextBestActionIndex = i;
                    score = actionsAvailable[i].score;
                }
            }

            return actionsAvailable[nextBestActionIndex]; ;
        }

        //Loop through all the considerations of the action
        //Score all theconsiderations
        //Average the consderation score ==> overall action score
        public float ScoreAction(Action action) 
        {
            float score = 1f;
            
            for (int i = 0; i < action.considerations.Length; i++) 
            { 
                float considerationScore = action.considerations[i].ScoreConsideration(npc);
                score *= considerationScore;

                if (score == 0) 
                {
                    action.score = 0;
                    return action.score; //no point computing further
                }
            }

            // averaging scheme of overall score
            // in other word : create a more usefull value due to fact that score is aggregated by multiplying decimal, which make the value lower and lower

            float originalScore = score;
            float modFactor = 1 - (1 / action.considerations.Length);
            float makeupValue = (1 - originalScore) * modFactor;
            action.score = originalScore + (makeupValue * originalScore);

            return action.score;
        }
    }
}