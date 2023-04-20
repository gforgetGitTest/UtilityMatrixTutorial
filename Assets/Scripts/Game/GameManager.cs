using System.Collections;
using System.Collections.Generic;
using TL.Core;
using UnityEngine;
using static GlobalEnum;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public NPCController[] NPCs;

    [SerializeField]private Transform[] WaitPositionsTransform;

    [SerializeField] private TreeResource Tree;
    [SerializeField] private Cafeteria Cafeteria;
    [SerializeField] private WoodReserve WoodReserve;
    [SerializeField] private House House;

    public Dictionary<GlobalEnum.InteractiveObject, InteractiveObject> InteractiveObjects = new Dictionary<GlobalEnum.InteractiveObject, InteractiveObject>();

    [HideInInspector]public List<Vector3>WaitPositions = new List<Vector3>();
    private void Awake()
    {
        for (int i = 0; i < WaitPositionsTransform.Length; i++)
        {
            WaitPositions.Add(WaitPositionsTransform[i].position);
        }

        InteractiveObjects.Add(GlobalEnum.InteractiveObject.Tree, Tree);
        InteractiveObjects.Add(GlobalEnum.InteractiveObject.Cafeteria, Cafeteria);
        InteractiveObjects.Add(GlobalEnum.InteractiveObject.WoodReserve, WoodReserve);
        InteractiveObjects.Add(GlobalEnum.InteractiveObject.House, House);

        Instance = this;
    }
}
