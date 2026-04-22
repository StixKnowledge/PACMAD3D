using UnityEngine;

public class MiniMapSystem : MonoBehaviour
{
    [SerializeField] private Transform playerTransform;

    private void Update()
    {
        Vector3 newPosition = playerTransform.position;
        newPosition.y = transform.position.y; // Keep the minimap at a fixed height
        transform.position = newPosition;

        transform.rotation = Quaternion.Euler(90f, playerTransform.eulerAngles.y, 0f); // Rotate minimap to match player orientation
    }
}
