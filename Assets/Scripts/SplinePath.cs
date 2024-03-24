using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SplinePath : MonoBehaviour
{
    public const float NODE_VISUAL_SIZE = 0.1f;
    public readonly Color[] NODE_SEGMENT_COLORS =
    {
        Color.red,
        Color.blue,
        Color.green,
        Color.yellow,
        Color.magenta,
        Color.cyan
    };

    public List<SplineNode> Nodes = new();
    public int NodeCount => Nodes.Count;
    public Vector3 Sample(float u) {
        int index = (int)Mathf.Clamp(Mathf.FloorToInt(u), 0f, NodeCount - 1f - 0.001f);
        float t = u - index;

        SplineNode from = Nodes[index];
        SplineNode to = Nodes[index + 1];

        // Lerp method
        Vector3[] points =
        {
            from.Position,
            from.Position + from.ControlPointA,
            to.Position + to.ControlPointB,
            to.Position
        };

        return Vector3.Lerp(
                Vector3.Lerp(
                    Vector3.Lerp(points[0], points[1], t),
                    Vector3.Lerp(points[1], points[2], t),
                t),
                Vector3.Lerp(
                    Vector3.Lerp(points[1], points[2], t),
                    Vector3.Lerp(points[2], points[3], t),
                t),
            t
            );
    }

    public float SampleSpeed(float u)
    {
        const float epsilon = 0.01f;

        return Vector3.Distance(Sample(u), Sample(u + epsilon));
    }
    public Quaternion SampleDirection(float u)
    {
        const float epsilon = 0.01f;

        return Quaternion.FromToRotation(Vector3.right, (Sample(u + epsilon) - Sample(u)).normalized);
    }
    public void RenderCurve(out List<Vector3> curve, int samples)
    {
        RenderCurveFrom(out curve, samples, 0f, NodeCount - 1);
    }
    public void RenderCurveFrom(out List<Vector3> curve, int samples, float initialU, float endU)
    {
        curve = new();

        for (int sample = 0; sample < samples; sample++)
        {
            float u = Mathf.Lerp(initialU, endU, ((float)sample / (samples - 1))); // -1 to make sure we get the end of the curve

            Vector3 pos = Sample(u);
            curve.Add(pos);
        }
    }

    private void Start()
    {
        for (int i = 0; i < NodeCount - 1; i++)
        {
            GameObject lineSegmentObj = new();
            lineSegmentObj.transform.parent = transform;
            LineRenderer lines = lineSegmentObj.AddComponent<LineRenderer>();
            lines.widthMultiplier = 0.1f;
            lines.material = new Material(Shader.Find("Sprites/Default"));
            lines.startColor = NODE_SEGMENT_COLORS[i % NODE_SEGMENT_COLORS.Length];
            lines.endColor = NODE_SEGMENT_COLORS[i % NODE_SEGMENT_COLORS.Length];

            List<Vector3> curve;
            RenderCurveFrom(out curve, 50, i, i + 1);
            lines.positionCount = curve.Count;
            lines.SetPositions(curve.ToArray());
        }
        
    }

    private void OnDrawGizmos()
    {
        for (int i = 0; i < Nodes.Count; i++)
        {
            SplineNode node = Nodes[i];
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(node.Position, NODE_VISUAL_SIZE);

            if (i < Nodes.Count - 1)
            {
                Gizmos.color = Color.white;
                SplineNode nextNode = Nodes[i + 1];

                Gizmos.DrawLine(node.Position, nextNode.Position);
            }

            Gizmos.color = Color.white;
            Gizmos.DrawLine(node.Position, node.Position + node.ControlPointA);
            Gizmos.DrawLine(node.Position, node.Position + node.ControlPointB);

            Gizmos.color = Color.red;
            Gizmos.DrawSphere(node.Position + node.ControlPointA, NODE_VISUAL_SIZE);
            Gizmos.DrawSphere(node.Position + node.ControlPointB, NODE_VISUAL_SIZE);
        }

        Gizmos.color = Color.blue;

        List<Vector3> curve;
        RenderCurve(out curve, 50);
        for (int i = 0; i < curve.Count - 1; i++)
        {
            Vector3 curvePoint = curve[i];
            Vector3 nextCurvePoint = curve[i + 1];

            Gizmos.DrawLine(curvePoint, nextCurvePoint);
        }
    }
}
