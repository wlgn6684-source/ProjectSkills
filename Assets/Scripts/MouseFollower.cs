using UnityEngine;

public class MouseFollower : MonoBehaviour
{

    void Start()
    {
        InputManager.OnMouseLeftUp += CreateToMouse;
        InputManager.OnMouseRightDown += DestroyToMouse;
        
    }

    void CreateToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        Instantiate(DataManager.LoadDataFile<GameObject>("Square 13"), worldPosition, Quaternion.identity);
    }

    void MoveToMouse(Vector2 screenPosition, Vector3 worldPosition)
    { 
        transform.position = worldPosition;
    }
    void DestroyToMouse(Vector2 screenPosition, Vector3 worldPosition)
    {
        Debug.Log( GameManager.Instance.Input.GetGameObjectUnderCursor());
    }

}
