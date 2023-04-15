using System.Collections;
using System.Collections.Generic;
using TL.Core;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    public NPCController[] NPCs;

    [SerializeField]private Transform[] WaitPositionsTransform;

    public TreeResource Tree;
    public Cafeteria Cafeteria;
    public WoodReserve WoodReserve;
    public House House;

    [HideInInspector]public List<Vector3>WaitPositions = new List<Vector3>();
    private void Awake()
    {
        for (int i = 0; i < WaitPositionsTransform.Length; i++)
        {
            WaitPositions.Add(WaitPositionsTransform[i].position);
        }

        Instance = this;
    }
}
