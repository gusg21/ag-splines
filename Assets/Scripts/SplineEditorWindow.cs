using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

class SplineEditorWindow : EditorWindow
{
    private SplinePath selectedPath;
    private static VisualTreeAsset splineNodeUiAsset;

    private void OnEnable()
    {
        

        selectedPath = GetSelectedPath();

        ConstructGUI();

        rootVisualElement.Q<Button>("add-button").clicked += AddNode;

        Repaint();
        UpdateGUI();
    }

    private void OnDisable()
    {
        selectedPath = GetSelectedPath();
        Repaint();
        UpdateGUI();
    }

    private void OnSelectionChange()
    {
        selectedPath = GetSelectedPath();
        Repaint();
        UpdateGUI();
    }

    private static void ConstructGUI()
    {
        SplineEditorWindow editorWindow = GetWindow<SplineEditorWindow>();
        editorWindow.rootVisualElement.Clear();

        editorWindow.titleContent = new GUIContent("Spline Editor Window");

        VisualTreeAsset uiAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/EditorUIs/SplineEditorUI.uxml");
        VisualElement ui = uiAsset.Instantiate();

        if (splineNodeUiAsset == null)
            splineNodeUiAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/EditorUIs/SplineNodeInfoUI.uxml");

        editorWindow.rootVisualElement.Add(ui);
    }

    private void UpdateGUI()
    {
        if (selectedPath == null)
        {
            rootVisualElement.Q("editor").SetEnabled(false);
            return;
        }

        rootVisualElement.Q("editor").SetEnabled(true);

        VisualElement nodeList = rootVisualElement.Q<VisualElement>("node-list");
        nodeList.Clear();

        for (int i = 0; i < selectedPath.Nodes.Count; i++)
        {
            SplineNode node = selectedPath.Nodes[i];
            VisualElement nodeUI = splineNodeUiAsset.Instantiate();
            nodeUI.name = $"node-{i}";
            nodeUI.Q<Label>("title").text = $"Spline Node {i}";

            nodeUI.Q<Button>("delete-button").clicked += () => RemoveNode(i);

            nodeUI.Q<Vector3Field>("position").SetEnabled(false);
            nodeUI.Q<Vector3Field>("position").value = node.Position;

            nodeList.Add(nodeUI);
        }
    }

    [MenuItem("Window/Spline Editor")]
    public static void ShowEditor()
    {
        ConstructGUI();
    }

    private SplinePath GetSelectedPath()
    {
        if (Selection.activeGameObject != null)
        {
            SplinePath target = Selection.activeGameObject.GetComponentInChildren<SplinePath>();
            if (target != null)
                return target;
        }
        return null;
    }

    private void AddNode()
    {
        SplineNode lastNode = selectedPath.Nodes[selectedPath.NodeCount - 1];

        SplineNode newNode = new();
        newNode.Position = lastNode.Position + UnityEngine.Random.onUnitSphere;

        SplinePathEditor.SelectedNodeIndex = selectedPath.NodeCount - 1;

        selectedPath.Nodes.Add(newNode);
    }

    private void RemoveNode(int index)
    {
        selectedPath.Nodes.RemoveAt(index);
    }
}