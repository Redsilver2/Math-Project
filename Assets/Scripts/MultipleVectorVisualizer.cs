using Game.Managers;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class MultipleVectorVisualizer : MonoBehaviour
{
    [SerializeField] private float offset = 2f;
    private float lineOffsetMultiplier = 1f;

    private VectorVisualizerType visualizerType;
    public VectorVisualizerType VisualizerType => visualizerType;

    private LineRenderer rootRenderer;
    private Vector3   direction;

    private void Start()
    {
        Initialize();
    }


    private void Initialize()
    {
        var root = new GameObject();

        rootRenderer   = root.AddComponent<LineRenderer>();  
        root.transform.SetParent(transform, false);

        root.name  = "Linerender";
        direction = GetDirection();

        root.transform.localPosition = Vector3.zero;
        rootRenderer.startWidth = 0.25f;

        rootRenderer.material = GameManager.GetMaterial((int)visualizerType);
    }

    public void SetOffsetMultiplier(float offset)
    {
        lineOffsetMultiplier = offset;  
    }

    private Vector3 GetDirection()
    {
        if (visualizerType == VectorVisualizerType.X)
            return transform.right;
        else if (visualizerType == VectorVisualizerType.Y)
            return transform.up;

        return transform.forward;
    }

    public void UpdateVisuals(int direction)
    {
        Transform rootTransform = rootRenderer.transform;

        if (rootRenderer != null)
            rootRenderer.SetPositions(new Vector3[] { rootTransform.position, rootTransform.position + GetDirection() * offset * lineOffsetMultiplier });
    }
    

    private void SetVisualizerType(VectorVisualizerType visualizerType)
    {
        this.visualizerType = visualizerType;
    }

    private void SetOffSet(float offset)
    {
        this.offset = offset;
    }

    public static MultipleVectorVisualizer[] SetAndGetVisualizers(Transform transform, float offset)
    {
        MultipleVectorVisualizer[] visualizers = SetAndGetVisualizers(transform);
      
        foreach (MultipleVectorVisualizer visualizer in visualizers)
        {
            visualizer.SetOffSet(offset);
        }

        return visualizers;
    }

    public static MultipleVectorVisualizer[] SetAndGetVisualizers(Transform transform)
    {
        if (transform == null) return new MultipleVectorVisualizer[0];

        List<MultipleVectorVisualizer> results = new List<MultipleVectorVisualizer>();
        StringBuilder builder = new StringBuilder();
        VectorVisualizerType[] visualizerTypes = (VectorVisualizerType[])Enum.GetValues(typeof(VectorVisualizerType));

        for (int i = 0; i < visualizerTypes.Length; i++)
        {
            GameObject visualizerObject = new GameObject();
            MultipleVectorVisualizer visualizer;

            builder.Clear();
            visualizerObject.transform.SetParent(transform);

            visualizerObject.name = builder.Append("Visualizer #").Append(i).ToString();
            visualizer = visualizerObject.AddComponent<MultipleVectorVisualizer>();

            visualizer.transform.localPosition = Vector3.zero;

            visualizer.SetVisualizerType(visualizerTypes[i]);
            results.Add(visualizer);
        }

        return results.ToArray();
    }
}

public enum VectorVisualizerType
{
    X, Y, Z
}
