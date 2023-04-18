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

        [SerializeField] private float HungerTick = 5.0f;
        [SerializeField] private float EnergyTick = 2.5f;

        [SerializeField]
        private Renderer mainRenderer;

        private void Awake()
        {
            stats = new Stats(this);
            inventory = new Inventory(this);
        }

        // Start is called before the first frame update
        IEnumerator Start()
        {
            mover = GetComponent<MoveController>();
            aiBrain = GetComponent<AIBrain>();

            while (GameManager.Instance == null) 
            {
                yield return null;
            }

            StartCoroutine(EnergyRoutine());
            StartCoroutine(HungerRoutine());

        }
        private void OnDestroy()
        {
            StopAllCoroutines();
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
        public void CancelCurrentAction() 
        { 
            //TODO: implement a function that cancel the current action
        }

        public void DoWork() 
        {
            StartCoroutine(WorkCoroutine());
        }
        public void DoSleep()
        {
            StartCoroutine(SleepCoroutine());
        }
        public void DoEat()
        {
            StartCoroutine(EatCoroutine());
        }
        public void DoBuyFood()
        {
            StartCoroutine(BuyFoodCoroutine());
        }

        public void DoSellWood() 
        {
            StartCoroutine(SellWoodCoroutine());
        }

        IEnumerator WorkCoroutine() 
        {
            if (!GameManager.Instance.Tree.ValidateRequirement(this))
            {
                Debug.Log(transform.name + " : I do not have enough energy to work!");
                OnFinishedAction();
                yield break;
            }

            if (GameManager.Instance.Tree.RequestInteraction(this))
            {
                mover.MoveTo(GameManager.Instance.Tree.InteractPosition.position);
                while (!mover.HasArrived())
                {
                    yield return null;
                }

                //something can happen while moving that defy the requirement for the action to be executed
                if (!GameManager.Instance.Tree.ValidateRequirement(this))
                {
                    Debug.Log(transform.name + " : I do not have enough energy to work!");
                    OnFinishedAction();
                    yield break;
                }

                GameManager.Instance.Tree.Interact(this);

                yield return new WaitForSeconds(GameManager.Instance.Tree.TimeToServe);
            }
            else 
            {
                mover.MoveTo(GameManager.Instance.Tree.WaitingArea.position);
                while (!mover.HasArrived())
                {
                    yield return null;
                }
            }

            // Decide our new best action after you finished this one
            OnFinishedAction();
        }

        IEnumerator SleepCoroutine()
        {

            if (GameManager.Instance.House.RequestInteraction(this))
            {
                mover.MoveTo(GameManager.Instance.House.InteractPosition.position);
                while (!mover.HasArrived())
                {
                    yield return null;
                }

                GameManager.Instance.House.Interact(this);

                yield return new WaitForSeconds(GameManager.Instance.House.TimeToServe);
            }
            else
            {
                mover.MoveTo(GameManager.Instance.House.WaitingArea.position);
                while (!mover.HasArrived())
                {
                    yield return null;
                }
            }

            // Decide our new best action after you finished this one
            OnFinishedAction();
        }

        IEnumerator EatCoroutine()
        {
            if (inventory.food == 0) 
            {
                Debug.Log(transform.name + " : I can't eat, I have no food !");
                OnFinishedAction();
                yield break;
            }

            //find closest wait area by sorting all wait positions and picking the first one
            List<Vector3> points = GameManager.Instance.WaitPositions;
            points.Sort(delegate (Vector3 a, Vector3 b)
            {
                return Vector3.Distance(transform.position, a).CompareTo(Vector3.Distance(transform.position, b));
            });

            mover.MoveTo(points[0]);
            while (!mover.HasArrived())
            {
                yield return null;
            }

            //something can happen while moving that defy the requirement for the action to be executed
            if (inventory.food == 0)
            {
                Debug.Log(transform.name + " : I can't eat, I have no food !");
                OnFinishedAction();
                yield break;
            }

            //eat - transform food into lowering hunger score
            yield return new WaitForSeconds(1f);

            inventory.food -= 1;
            stats.hunger -= 5;

            OnFinishedAction();
        }

        IEnumerator BuyFoodCoroutine()
        {
            if (!GameManager.Instance.Cafeteria.ValidateRequirement(this))
            {
                Debug.Log(transform.name + " : I do not have enough money to buy food!");
                OnFinishedAction();
                yield break;
            }

            if (GameManager.Instance.Cafeteria.RequestInteraction(this))
            {
                mover.MoveTo(GameManager.Instance.Cafeteria.InteractPosition.position);
                while (!mover.HasArrived())
                {
                    yield return null;
                }

                //something can happen while moving that defy the requirement for the action to be executed
                if (!GameManager.Instance.Cafeteria.ValidateRequirement(this))
                {
                    Debug.Log(transform.name + " : I do not have enough money to buy food!");
                    OnFinishedAction();
                    yield break;
                }

                GameManager.Instance.Cafeteria.Interact(this);

                yield return new WaitForSeconds(GameManager.Instance.Cafeteria.TimeToServe);
            }
            else
            {
                mover.MoveTo(GameManager.Instance.Cafeteria.WaitingArea.position);
                while (!mover.HasArrived())
                {
                    yield return null;
                }
            }

            // Decide our new best action after you finished this one
            OnFinishedAction();
        }

        IEnumerator SellWoodCoroutine()
        {
            if (!GameManager.Instance.WoodReserve.ValidateRequirement(this))
            {
                Debug.Log(transform.name + " : I do not have wood to sell !");
                OnFinishedAction();
                yield break;
            }

            if (GameManager.Instance.WoodReserve.RequestInteraction(this))
            {
                mover.MoveTo(GameManager.Instance.WoodReserve.InteractPosition.position);
                while (!mover.HasArrived())
                {
                    yield return null;
                }

                //something can happen while moving that defy the requirement for the action to be executed
                if (!GameManager.Instance.WoodReserve.ValidateRequirement(this))
                {
                    Debug.Log(transform.name + " : I do not have wood to sell !");
                    OnFinishedAction();
                    yield break;
                }

                GameManager.Instance.WoodReserve.Interact(this);

                yield return new WaitForSeconds(GameManager.Instance.WoodReserve.TimeToServe);
            }
            else
            {
                mover.MoveTo(GameManager.Instance.WoodReserve.WaitingArea.position);
                while (!mover.HasArrived())
                {
                    yield return null;
                }
            }

            // Decide our new best action after you finished this one
            OnFinishedAction();
        }

        IEnumerator EnergyRoutine() 
        {
            yield return new WaitForSeconds(EnergyTick);
            while (true)
            {
                stats.energy -= 1;
                yield return new WaitForSeconds(EnergyTick);
            }
        }
        IEnumerator HungerRoutine() 
        {
            yield return new WaitForSeconds(HungerTick);
            while (true) 
            {
                stats.hunger += 1;
                yield return new WaitForSeconds(HungerTick);
            }
        }

        #endregion

        #region AddProperties
        public void AddFood(int addedValue)
        {
            inventory.food += addedValue;
        }

        public void AddWood(int addedValue) 
        {
            inventory.wood += addedValue;
        }

        public void AddMoney(int addedValue)
        {
            inventory.money += addedValue;
        }

        public void AddEnergy(int addedValue)
        {
            stats.energy += addedValue;
        }

        public void AddHunger(int addedValue)
        {
            stats.hunger += addedValue;
        }
        #endregion
    }

    public class Inventory
    {
        private NPCController npc;
        public int food
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
        private int _food;

        public int wood
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
        private int _wood;

        public int money
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
        private int _money;


        public Inventory(NPCController _npc, int _food = 10, int _wood = 5, int _money = 10)
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
        public int energy
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
        private int _energy;

        public int hunger
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
        private int _hunger;

        public Stats(NPCController _npc, int _energy = 0, int _hunger = 10) 
        {
            npc = _npc;
            energy = _energy;
            hunger = _hunger;
        }
    }
}

