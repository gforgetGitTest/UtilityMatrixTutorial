using System.Collections;
using System.Collections.Generic;
using TL.Core;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    public static NPCManager Instance { get; private set; }
    public NPCController[] npcs;

    private void Awake()
    {
        Instance = this;
    }
}
