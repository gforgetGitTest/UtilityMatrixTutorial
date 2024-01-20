using System.Collections;
using System.Collections.Generic;
using TL.Core;
using UnityEngine;

public class WaitingArea : MonoBehaviour
{
    [SerializeField] private List<Transform> AllPossiblePositionsReferences;
    [SerializeField] private List<(bool, Vector3)> AllPossiblePositions;

    void OnDrawGizmos()
    {
        for (int i = 0; i < AllPossiblePositionsReferences.Count; i++) 
        {
            DrawGizmoDisk(AllPossiblePositionsReferences[i].transform.position);
        }
    }

    private const float GIZMO_DISK_THICKNESS = 0.01f;
    public void DrawGizmoDisk(Vector3 position)
    {
        Matrix4x4 oldMatrix = Gizmos.matrix;
        Gizmos.color = new Color(0f, 1f, 1f, 0.5f); 
        Gizmos.matrix = Matrix4x4.TRS(position, Quaternion.identity, new Vector3(1, GIZMO_DISK_THICKNESS, 1));
        Gizmos.DrawSphere(Vector3.zero, 1.0f);
        Gizmos.matrix = oldMatrix;
    }
}
