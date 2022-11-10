using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point : MonoBehaviour
{
    public Transform[] NextPoints;
    public bool IsJump = false;
    public bool IsEnd = false;
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (NextPoints.Length == 0)
            return;
        for (int i = 0; i < NextPoints.Length; i++)
            Gizmos.DrawLine(transform.position, NextPoints[i].position);
    }
}
