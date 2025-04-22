using System.Net;
using TMPro;
using UnityEngine;

public class SingleVectorVisualizer : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private TextMeshProUGUI startText, endText;
    private Camera mainCamera;

    private Vector3 startPosition, endPosition;

    private void Start()
    {
        mainCamera   = Camera.main;
    }

    public void SetRendererMaterial(Material material)
    {
        if (lineRenderer == null)
        {
            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth    = 0.25f;
            lineRenderer.useWorldSpace = true;
        }

        lineRenderer.material = material; 
    }

    public void SetUIText(Transform parentCanvas)
    {
        startText = new GameObject().AddComponent<TextMeshProUGUI>();
        endText   = new GameObject().AddComponent<TextMeshProUGUI>();

        if(parentCanvas != null)
        {
            if (startText != null)
            {
                startText.fontSize = 5;
                startText.alignment = TextAlignmentOptions.Center;
                startText.transform.SetParent(parentCanvas, false);
                startText.transform.eulerAngles = Vector3.up * -180f;
                startText.transform.localScale = new Vector3(-0.1f, 0.1f, 1);
            }

            if (endText != null)
            {
                endText.fontSize = 5;
                endText.alignment = TextAlignmentOptions.Center;
                endText.transform.SetParent(parentCanvas, false);
                endText.transform.eulerAngles = Vector3.up * -180f;
                endText.transform.localScale  = new Vector3(-0.1f, 0.1f, 1);
            }
        }
            
    }
    public void UpdateRenderer(Vector3 startingPoint, Vector3 endPoint)
    {

        startPosition = startingPoint;
        endPosition = endPoint;

        if (startText != null)
        {
            startText.text = $"({System.MathF.Round(startingPoint.x, 2)}, {System.MathF.Round(startingPoint.y, 2)}, {System.MathF.Round(startingPoint.z, 2)})\t\t\t\t";
            startText.transform.position = startingPoint;
        }

        if (endText != null)
        {
            endText.text = $"({System.MathF.Round(endPoint.x, 2)}, {System.MathF.Round(endPoint.y, 2)}, {System.MathF.Round(endPoint.z, 2)})\t\t\t\t";
            endText.transform.position = endPoint + Vector3.up;
        }
    }

    public void UpdateRenderer()
    {
        if (lineRenderer != null)
            lineRenderer.SetPositions(new Vector3[] { startPosition, endPosition });
    }
   
    private void OnDisable()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = false;

        if (startText != null)
            startText.enabled = false;

        if (endText != null)
            endText.enabled = false;
    }
    private void OnEnable()
    {
        if (lineRenderer != null)
            lineRenderer.enabled = true;

        if (startText != null)
            startText.enabled = true;

        if (endText != null)
            endText.enabled = true;
    }
}
