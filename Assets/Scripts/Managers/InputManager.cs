using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

//          
//       대리자 => 기능을 전달 위임
// 플레이어가 할 일 대리 뛰어주고, 열려있는 창이 있다면 그 친구의 기능도 수행하고 간다!
public delegate void MouseDownEvent(Vector2 position, Vector3 worldPosition);
public delegate void MouseUpEvent(Vector2 position, Vector3 worldPosition);
public delegate void MouseMoveEvent(Vector2 screenPosition, Vector3 worldPosition);


//대상 변수나 클래스 위쪽에 []에 내용을 넣는 것을 Attribute : 속성이라고 한다.
[RequireComponent(typeof(PlayerInput))]

public class InputManager : ManagerBase
{
    public static event MouseDownEvent OnMouseLeftDown;
    public static event MouseDownEvent OnMouseRightDown;
    public static event MouseUpEvent OnMouseLeftUp;    
    public static event MouseUpEvent OnMouseRightUp;        
    public static event MouseMoveEvent OnMouseMove;    
    
    
    PlayerInput targetInput;
    Dictionary<string, InputAction> actionDictionary = new();
    List<RaycastResult> cursorHitList = new();

    Vector2 cursorScreenPosition;
    Vector3 cursorWorldPosition; 

    public bool is2D = true;

    protected override IEnumerator OnConnected(GameManager newManager)
    {   
        
        targetInput = GetComponent<PlayerInput>();

        LoadAllActions();
        InitializeAllActions();

        GameManager.OnUpdateManager -= UpdateEvent;//뺄건데 없으면 말고
        GameManager.OnUpdateManager += UpdateEvent;

        yield return null;
    }
    protected override void OnDisconnected()
    {
        GameManager.OnUpdateManager -= UpdateEvent;
    }

    public void UpdateEvent(float deltaTime) 
    {
        RefreshGameObjectUnderCursor();
    }
    
    void RefreshGameObjectUnderCursor()
    {
        cursorHitList.Clear();
        if (is2D)
        {
            GameManager.Instance.Camera.GetRaycastResult2D(cursorScreenPosition, cursorHitList);
        }
        else
        {
            GameManager.Instance.Camera.GetRaycastResult3D(cursorScreenPosition, cursorHitList);
        }
    }

    public GameObject GetGameObjectUnderCursor()
    { 
        if (cursorHitList.Count == 0) return null;
        return cursorHitList[0].gameObject;
    }

    void LoadAllActions()
    {
        foreach (InputAction currentAction in targetInput.actions)
        {
            actionDictionary.TryAdd(currentAction.name, currentAction);
            //currentAction.performed += (InputAction.CallbackContext context) =>{ Debug.Log(currentAction.name);};
        }
    }

    void InitializeAllActions()
    {
        if(actionDictionary == null || actionDictionary.Count == 0) return;
        InitializeAction ("CursorPositionChanged", CursorPositionChanged);
        InitializeAction ("MouseRightButtonUp",   (context) => OnMouseRightUp?.Invoke(cursorScreenPosition, cursorWorldPosition));
        InitializeAction ("MouseLeftButtonUp",    (context) => OnMouseLeftUp?.Invoke(cursorScreenPosition, cursorWorldPosition));
        InitializeAction ("MouseRightButtonDown", (context) => OnMouseRightDown?.Invoke(cursorScreenPosition, cursorWorldPosition));
        InitializeAction ("MouseLeftButtonDown",  (context) => OnMouseLeftDown?.Invoke(cursorScreenPosition, cursorWorldPosition));
    }

    

    void InitializeAction(string actionName, Action<InputAction.CallbackContext>actionMethod)
    {
        if (actionDictionary == null) return;
        if (actionDictionary.TryGetValue(actionName, out InputAction cursorPositionChange))
        {
            cursorPositionChange.performed += actionMethod;
        }
    }

    void CursorPositionChanged(InputAction.CallbackContext context)
    {
        Camera mainCamera = Camera.main;
        Physics2DRaycaster raycaster2D = mainCamera.GetComponent<Physics2DRaycaster>();
        PhysicsRaycaster raycaster = mainCamera.GetComponent<PhysicsRaycaster>();

       

        //마우스의 화면상 실제 픽셀위치
        Vector2 screenPosition = context.ReadValue<Vector2>();
        //화면상 x축으로 1픽셀
        //유니티에서 1칸은 1m
        //카메라를 기준으로 세상을 본다
        Vector3 worldPosition;

        if (is2D)
        {
            worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
            worldPosition.z = 0;
        }

        else 
        {
            worldPosition = Vector3.zero;
        }
        
        cursorScreenPosition = screenPosition;
        cursorWorldPosition = worldPosition;

        //대리자는 모든 스킬을 한번에 사용 할 수 있지만 가르쳐주지 않으면 아무것도 할 수 없다
        OnMouseMove?.Invoke(screenPosition, worldPosition); 
    }

   
}
