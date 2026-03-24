using UnityEngine;

public class MouseFollower : MonoBehaviour
{

    void Start()
    {
        InputManager.OnMouseMove += MoveToMouse;
    }

    void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    { 
        transform.position = worldPosition;
    }

}
