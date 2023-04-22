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

        public delegate void OnNPCDeathEvent(NPCController npc);
        public event OnNPCDeathEvent OnNPCDeath;

        public Transform ActionTextAnchor;

        public MoveController mover;

        public AIBrain aiBrain;
        public TL.UtilityAI.Action[] actionsAvailable;

        public float MaxSpeed = 7.0f;

        [SerializeField] private Renderer mainRenderer;

        public Stats stats;
        public Inventory inventory;

        private float HungerTick = 3.5f;
        private float EnergyTick = 2.5f;

        private TL.UtilityAI.Action oldAction;
        private TL.UtilityAI.Action currentAction;
        private bool CanCancelAction = true;

        [HideInInspector]public InteractiveObject usingInteractiveObject;
        [HideInInspector]public string CurrentActionName = "Walking";

        private void Awake()
        {
            stats = new Stats(this);
            inventory = new Inventory(this);
        }

        // Start is called before the first frame update
        IEnumerator Start()
        {
            while (GameManager.Instance == null) 
            {
                yield return null;
            }

            StartCoroutine(EnergyRoutine());
            StartCoroutine(HungerRoutine());

            currentAction = aiBrain.DecideBestAction(actionsAvailable);
            oldAction = currentAction;
        }

        // Update is called once per frame
        void Update()
        {
            if (aiBrain.LookUpBestAction(actionsAvailable) != currentAction) 
            {
                //Stop Current Action
                CancelCurrentAction();
            }

            if (aiBrain.finishedDeciding)
            {
                aiBrain.finishedDeciding = false;
                aiBrain.bestAction.Execute(this);
            }
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
        }

        public void ChangeNPCColor(Material material) 
        {
            mainRenderer.material = material;
        }

        public void CallOnPropertyChange() 
        {
            if (OnPropertyChange != null) OnPropertyChange(this);
        }

        public void OnFinishedAction() 
        {
            CanCancelAction = true;
            currentAction = aiBrain.DecideBestAction(actionsAvailable);
            if (oldAction != currentAction && usingInteractiveObject != null) 
            {
                usingInteractiveObject.ReleaseInteractiveObject(this);
                usingInteractiveObject = null;
            }
            oldAction = currentAction;
        }

        #region Coroutine
        public void CancelCurrentAction() 
        {
            if (CanCancelAction) 
            {
                //Stop the actual action
                StopCoroutine("WorkCoroutine");
                StopCoroutine("SleepCoroutine");
                StopCoroutine("EatCoroutine");
                StopCoroutine("BuyFoodCoroutine");
                StopCoroutine("SellWoodCoroutine");
                StopCoroutine("GoRestCoroutine");
                StopCoroutine("RestCoroutine");
                //Change action by calling the OnFinishedAction event
                OnFinishedAction();
            }
        }

        public void DoWork() 
        {
            StartCoroutine("WorkCoroutine");
        }
        public void DoSleep()
        {
            StartCoroutine("SleepCoroutine");
        }

        public void DoGoRest() 
        {
            StartCoroutine("GoRestCoroutine");
        }

        public void DoEat()
        {
            StartCoroutine("EatCoroutine");
        }
        public void DoBuyFood()
        {
            StartCoroutine("BuyFoodCoroutine");
        }

        public void DoSellWood() 
        {
            StartCoroutine("SellWoodCoroutine");
        }

        IEnumerator WorkCoroutine() 
        {
            CurrentActionName = "Walking";

            if (!GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.Tree].ValidateRequirement(this))
            {
                Debug.Log(transform.name + " : I do not have enough energy to work!");
                OnFinishedAction();
                yield break;
            }

            if (GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.Tree].RequestInteraction(this))
            {
                CanCancelAction = false;

                mover.MoveTo(GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.Tree].InteractPosition.position);
                while (!mover.HasArrived())
                {
                    yield return null;
                }

                //something can happen while moving that defy the requirement for the action to be executed
                if (!GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.Tree].ValidateRequirement(this))
                {
                    Debug.Log(transform.name + " : I do not have enough energy to work!");
                    OnFinishedAction();
                    yield break;
                }

                CurrentActionName = "Working";
                GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.Tree].Interact(this);

                yield return new WaitForSeconds(GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.Tree].TimeToServe);

                // Decide our new best action after you finished this one
                OnFinishedAction();
            }
            else 
            {
                mover.MoveTo(GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.Tree].WaitingArea.position);
                while (!mover.HasArrived())
                {
                    yield return null;
                }

                StartCoroutine("RestCoroutine");
            }

        }

        IEnumerator SleepCoroutine()
        {
            CurrentActionName = "Walking";
            if (GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.House].RequestInteraction(this))
            {
                CanCancelAction = false;
                mover.MoveTo(GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.House].InteractPosition.position);
                while (!mover.HasArrived())
                {
                    yield return null;
                }

                CurrentActionName = "Sleeping";
                GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.House].Interact(this);

                yield return new WaitForSeconds(GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.House].TimeToServe);
                
                // Decide our new best action after you finished this one
                OnFinishedAction();
            }
            else
            {
                mover.MoveTo(GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.House].WaitingArea.position);
                while (!mover.HasArrived())
                {
                    yield return null;
                }

                StartCoroutine("RestCoroutine");

            }
        }

        IEnumerator EatCoroutine()
        {
            CurrentActionName = "Walking";
            CanCancelAction = false;

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

            CurrentActionName = "Eating";
            //eat - transform food into lowering hunger score
            yield return new WaitForSeconds(1f);

            inventory.food -= 1;
            stats.hunger -= 5;
            stats.energy += 2;

            OnFinishedAction();
        }

        IEnumerator BuyFoodCoroutine()
        {
            CurrentActionName = "Walking";
            if (!GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.Cafeteria].ValidateRequirement(this))
            {
                Debug.Log(transform.name + " : I do not have enough money to buy food!");
                OnFinishedAction();
                yield break;
            }

            if (GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.Cafeteria].RequestInteraction(this))
            {
                CanCancelAction = false;
                mover.MoveTo(GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.Cafeteria].InteractPosition.position);
                while (!mover.HasArrived())
                {
                    yield return null;
                }

                //something can happen while moving that defy the requirement for the action to be executed
                if (!GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.Cafeteria].ValidateRequirement(this))
                {
                    Debug.Log(transform.name + " : I do not have enough money to buy food!");
                    OnFinishedAction();
                    yield break;
                }

                CurrentActionName = "Buying Food";
                GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.Cafeteria].Interact(this);

                yield return new WaitForSeconds(GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.Cafeteria].TimeToServe);
                
                // Decide our new best action after you finished this one
                OnFinishedAction();
            }
            else
            {
                mover.MoveTo(GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.Cafeteria].WaitingArea.position);
                while (!mover.HasArrived())
                {
                    yield return null;
                }
                StartCoroutine("RestCoroutine");
            }
        }

        IEnumerator SellWoodCoroutine()
        {
            CurrentActionName = "Walking";
            if (!GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.WoodReserve].ValidateRequirement(this))
            {
                Debug.Log(transform.name + " : I do not have wood to sell !");
                OnFinishedAction();
                yield break;
            }

            if (GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.WoodReserve].RequestInteraction(this))
            {
                CanCancelAction = false;
                mover.MoveTo(GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.WoodReserve].InteractPosition.position);
                while (!mover.HasArrived())
                {
                    yield return null;
                }

                //something can happen while moving that defy the requirement for the action to be executed
                if (!GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.WoodReserve].ValidateRequirement(this))
                {
                    Debug.Log(transform.name + " : I do not have wood to sell !");
                    OnFinishedAction();
                    yield break;
                }

                CurrentActionName = "Selling Wood";
                GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.WoodReserve].Interact(this);

                yield return new WaitForSeconds(GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.WoodReserve].TimeToServe);

                // Decide our new best action after you finished this one
                OnFinishedAction();
            }
            else
            {
                mover.MoveTo(GameManager.Instance.InteractiveObjects[GlobalEnum.InteractiveObject.WoodReserve].WaitingArea.position);
                while (!mover.HasArrived())
                {
                    yield return null;
                }
                StartCoroutine("RestCoroutine");
            }
        }

        IEnumerator GoRestCoroutine()
        {
            CurrentActionName = "Walking";

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

            StartCoroutine("RestCoroutine");
        }

        IEnumerator RestCoroutine()
        {
            CurrentActionName = "Resting";
            yield return new WaitForSeconds(1f);

            stats.energy += 2;
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
                if (stats.hunger == 100) 
                {
                    NPCDeath();
                }
                yield return new WaitForSeconds(HungerTick);
            }


        }

        private void NPCDeath() 
        {
            if (OnNPCDeath != null) OnNPCDeath(this);
            StopAllCoroutines();
            Destroy(gameObject);
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


        public Inventory(NPCController _npc, int _food = 0, int _wood = 0, int _money = 0)
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
                npc.mover.SetMovementSpeed(npc.MaxSpeed*Mathf.Clamp((float)_energy/100, 0.25f, 1.0f));
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

        public Stats(NPCController _npc, int _energy = 100, int _hunger = 0) 
        {
            npc = _npc;
            energy = _energy;
            hunger = _hunger;
        }
    }
}

