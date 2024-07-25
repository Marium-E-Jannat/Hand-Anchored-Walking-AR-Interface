using UnityEngine;
using UnityEngine.UI;

public class CanvasPointerHandler : MonoBehaviour
{
    public RectTransform pointerPanel; // Reference to the PointerPanel RectTransform
    public Canvas canvas; // Reference to the Canvas

    private RectTransform canvasRectTransform;

    private void Start()
    {
        canvasRectTransform = canvas.GetComponent<RectTransform>();
    }

    private void Update()
    {
        // Get the mouse or touch position
        Vector2 inputPosition = Input.mousePosition;

        // Convert the screen point to the canvas local point
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRectTransform, inputPosition, canvas.worldCamera, out localPoint);

        // Set the pointer position
        pointerPanel.anchoredPosition = localPoint;

        // Ensure the pointer is visible
        pointerPanel.gameObject.SetActive(true);
    }
}
