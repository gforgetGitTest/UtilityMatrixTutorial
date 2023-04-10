using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TL.UtilityAI;
using static UnityEngine.EventSystems.EventTrigger;

namespace TL.Core
{
    public class NPCController : MonoBehaviour
    {
        public delegate void OnPropertyChangeEvent(NPCController npc);
        public event OnPropertyChangeEvent OnPropertyChange;
        public MoveController mover { get; set; }

        public AIBrain aiBrain { get; set; }
        public TL.UtilityAI.Action[] actionsAvailable;

        public Stats stats;
        public Inventory inventory;

        [SerializeField]
        private Renderer mainRenderer;

        private void Awake()
        {
            stats = new Stats(this);
            inventory = new Inventory(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            mover = GetComponent<MoveController>();
            aiBrain = GetComponent<AIBrain>();
        }

        public void ChangeNPCColor(Material material) 
        {
            mainRenderer.material = material;
        }

        // Update is called once per frame
        void Update()
        {
            if (aiBrain.finishedDeciding) 
            {
                aiBrain.finishedDeciding = false;
                aiBrain.bestAction.Execute(this);
            }
        }

        public void CallOnPropertyChange() 
        {
            if (OnPropertyChange != null) OnPropertyChange(this);
        }

        public void OnFinishedAction() 
        {
            aiBrain.DecideBestAction(actionsAvailable);
        }

        #region Coroutine

        public void DoWork(int time) 
        {
            StartCoroutine(WorkCoroutine(time));
        }
        public void DoSleep(int time)
        {
            StartCoroutine(SleepCoroutine(time));
        }

        IEnumerator WorkCoroutine(int time) 
        {
            int counter = time;
            while (counter > 0) 
            {
                yield return new WaitForSeconds(1);
                counter--;
            }

            Debug.Log("I just harvested 1 resource!");
            // Logic to update things involved with work

            // Decide our new best action after you finished this one
            OnFinishedAction();
        }

        IEnumerator SleepCoroutine(int time)
        {
            int counter = time;
            while (counter > 0)
            {
                yield return new WaitForSeconds(1);
                counter--;
            }
            Debug.Log("I slept and gain 1 energy!");
            //Logic to update energy

            // Decide our new best action after you finished this one
            OnFinishedAction();
        }
        #endregion
    }

    public class Inventory
    {
        private NPCController npc;
        public float food
        {
            get
            {
                return _food;
            }

            set
            {
                _food = Mathf.Clamp(value, 0, 100);
                npc.CallOnPropertyChange();
            }
        }
        private float _food;

        public float wood
        {
            get
            {
                return _wood;
            }

            set
            {
                _wood = Mathf.Clamp(value, 0, 100);
                npc.CallOnPropertyChange();
            }
        }
        private float _wood;

        public float money
        {
            get
            {
                return _money;
            }

            set
            {
                _money = Mathf.Clamp(value, 0, 1000);
                npc.CallOnPropertyChange();
            }
        }
        private float _money;


        public Inventory(NPCController _npc, float _food = 10.0f, float _wood = 0.0f, float _money = 0.0f)
        {
            npc = _npc;
            food = _food;
            wood = _wood;
            money = _money;
        }
    }

    public class Stats 
    {
        private NPCController npc;
        public float energy
        {
            get 
            {
                return _energy;
            }

            set 
            {
                _energy = Mathf.Clamp(value, 0, 100);
                npc.CallOnPropertyChange();
            }
        }
        private float _energy;

        public float hunger
        {
            get
            {
                return _hunger;
            }

            set
            {
                _hunger = Mathf.Clamp(value, 0,100);
                npc.CallOnPropertyChange();
            }
        }
        private float _hunger;


        public Stats(NPCController _npc, float _energy = 100.0f, float _hunger = 0.0f) 
        {
            npc = _npc;
            energy = _energy;
            hunger = _hunger;
        }
    }
}

