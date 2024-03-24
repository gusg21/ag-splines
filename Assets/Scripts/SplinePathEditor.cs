using TreeEditor;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

[CustomEditor(typeof(SplinePath))]
class SplinePathEditor : Editor
{
    public static int SelectedNodeIndex = 0;

    protected virtual void OnSceneGUI()
    {
        SplinePath path = (SplinePath)target;

        for (int i = 0; i < path.Nodes.Count; i++)
        {
            SplineNode node = path.Nodes[i];
            
            if (i == SelectedNodeIndex)
            {
                EditorGUI.BeginChangeCheck();
                Vector3 newPos = node.Position;
                Quaternion newRot = node.Rotation;
                float newScale = node.Scale;
                Handles.TransformHandle(ref newPos, ref newRot, ref newScale);
                if (EditorGUI.EndChangeCheck())
                {
                    Undo.RecordObject(path, "Change Node Transform");

                    node.Position = newPos;
                    node.Rotation = newRot;
                    node.Scale = Mathf.Clamp(newScale, 0.1f, float.MaxValue);
                }
            }
            else
            {
                if (Handles.Button(node.Position, Quaternion.LookRotation(
                (Camera.current.transform.position - node.Position).normalized,
                Camera.current.transform.up), 0f, SplinePath.NODE_VISUAL_SIZE * 3f, Handles.SphereHandleCap))
                {
                    SelectedNodeIndex = i;
                }
            }
            
        }
    }
}