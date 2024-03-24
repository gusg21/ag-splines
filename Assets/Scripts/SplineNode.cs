using System;
using UnityEngine;

[Serializable]
public class SplineNode
{
    public Vector3 Position;
    public Quaternion Rotation = Quaternion.identity;
    public float Scale = 1f;

    public Vector3 ControlPointA => Rotation * Vector3.right * Scale;
    public Vector3 ControlPointB => -ControlPointA;
}