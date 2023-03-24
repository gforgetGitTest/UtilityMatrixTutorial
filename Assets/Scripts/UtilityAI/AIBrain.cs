using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TL.Core
{
    public class AIBrain : MonoBehaviour
    {
        public Action bestAction { get; set; }
        private NPCController npc;
        
        // Start is called before the first frame update
        void Start()
        {
            npc = GetComponent<NPCController>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        //Loop through all the available actions
        // Give me the highest scoring action
        public void DecideBestAction(Action[] actionsAvailable) 
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
        }

        //Loop through all the considerations of the action
        //Score all theconsiderations
        //Average the consderation score ==> overall action score
        public float ScoreAction(Action action) 
        {
            float score = 1f;
            
            for (int i = 0; i < action.considerations.Length; i++) 
            { 
                float considerationScore = action.considerations[i].ScoreConsideration();
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