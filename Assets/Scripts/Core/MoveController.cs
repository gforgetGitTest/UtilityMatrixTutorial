using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace TL.Core
{
    public class MoveController : MonoBehaviour
    {
        private NavMeshAgent Agent;
        private Vector3 Destination;

        // Start is called before the first frame update
        void Start()
        {
            Agent = GetComponent<NavMeshAgent>();
        }

        public void MoveTo(Vector3 position) 
        {
            Destination = position;
            Agent.SetDestination(position);
        }

        public bool HasArrived() 
        {
            //Debug.Log(Vector3.SqrMagnitude(Destination - transform.position));
            return Vector3.SqrMagnitude(Destination - transform.position) < 0.01f;
        }
    }
}