using UnityEngine;
using UnityEngine.UI;

public class ItemInspection : MonoBehaviour
{
    public Transform referenceTransform; // Reference to the transform you want to rotate
    public RawImage targetRawImage; // Reference to the RawImage you want to interact with
    public float rotationSpeed = 50f; // Speed of rotation
    public float inputSensitivity = 0.1f; // Sensitivity of input for rotation

    private Vector3 mouseOrigin;
    private bool isRotating;
    public Camera cam1;
    void Update()
    {
        // Check for mouse click within the target RawImage boundaries
        if (Input.GetMouseButtonDown(0))
        {
            // Check if mouse position is within the boundaries of the target RawImage
            RectTransform rectTransform = targetRawImage.rectTransform;
            Vector2 localMousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, Input.mousePosition, null, out localMousePosition);

            if (rectTransform.rect.Contains(localMousePosition))
            {
                isRotating = true;
                mouseOrigin = Input.mousePosition;
            }
        }

        // Stop rotating when mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            isRotating = false;
        }

        // Perform rotation if rotating
        if (isRotating)
        {
            Vector3 pos = cam1.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

            // Calculate rotation around the center of the object (reference transform)
            Quaternion rotation = Quaternion.Euler(0f, -pos.x * rotationSpeed * inputSensitivity, pos.y * rotationSpeed * inputSensitivity);

            // Apply rotation to the reference transform
            referenceTransform.rotation *= rotation;

            mouseOrigin = Input.mousePosition;
        }
    }
}
