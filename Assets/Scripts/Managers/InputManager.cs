using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;


//          
//       대리자 => 기능을 전달 위임
// 플레이어가 할 일 대리 뛰어주고, 열려있는 창이 있다면 그 친구의 기능도 수행하고 간다!
public delegate void MouseButtonEvent(bool value, Vector2 position, Vector3 worldPosition);
public delegate void MouseMoveEvent(Vector2 screenPosition, Vector3 worldPosition);
public delegate void ButtonEvent(bool value);
public delegate void VectorEvent(Vector2 value);
public delegate void AxisEvent(Vector2 value);


//대상 변수나 클래스 위쪽에 []에 내용을 넣는 것을 Attribute : 속성이라고 한다.
[RequireComponent(typeof(PlayerInput))]

public class InputManager : ManagerBase
{
    public static event MouseButtonEvent OnMouseLeftButton;
    public static event MouseButtonEvent OnMouseRightButton;
       
    public static event MouseMoveEvent   OnMouseMove;    
    public static event ButtonEvent      OnMovementButton;    
    public static event ButtonEvent      OnInteractionButton;    
    public static event ButtonEvent      OnRunningButton;    
    public static event ButtonEvent      OnInsensiblyButton;    
    public static event ButtonEvent      OnTiltButton;    
    public static event ButtonEvent      OnMapButton;    
    public static event ButtonEvent      OnInventoryButton;    
    public static event ButtonEvent      OnCancelButton;
    public static event Action           OnAnyKey;
    
    public static event VectorEvent      OnMove;
    
    
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
        InitializeAction ("CursorPositionChanged",     (context) => CursorPositionChanged(GetVector2Value(context)));
        InitializeAction ("Movement",        (context) => OnMove                  ?.Invoke(GetVector2Value(context))
                                            ,(context) => OnMove                  ?.Invoke(Vector2.zero));

        InitializeAction ("MouseRightButton",(context) => OnMouseRightButton      ?.Invoke(true, cursorScreenPosition, cursorWorldPosition)
        ,                                    (context) => OnMouseRightButton      ?.Invoke(false, cursorScreenPosition, cursorWorldPosition));
        InitializeAction ("MouseLeftButton", (context) => OnMouseLeftButton       ?.Invoke(true, cursorScreenPosition, cursorWorldPosition)
        ,                                    (context) => OnMouseLeftButton       ?.Invoke(false, cursorScreenPosition, cursorWorldPosition));

        InitializeAction("MovementDown",     (context) => OnMovementButton        ?.Invoke(true));
        InitializeAction("MovementUp",       (context) => OnMovementButton        ?.Invoke(false));
        InitializeAction("InteractionDown",  (context) => OnInteractionButton     ?.Invoke(true));
        InitializeAction("InteractionUp",    (context) => OnInteractionButton     ?.Invoke(false));
        InitializeAction("RunningDown",      (context) => OnRunningButton         ?.Invoke(true));
        InitializeAction("RunningUp",        (context) => OnRunningButton         ?.Invoke(false));
        InitializeAction("InsensiblyDown",   (context) => OnInsensiblyButton      ?.Invoke(true));
        InitializeAction("InsensiblyUp",     (context) => OnInsensiblyButton      ?.Invoke(false));
        InitializeAction("TiltDown",         (context) => OnTiltButton            ?.Invoke(true));
        InitializeAction("TiltUp",           (context) => OnTiltButton            ?.Invoke(false));
        InitializeAction("MapDown",          (context) => OnMapButton             ?.Invoke(true));
        InitializeAction("MapUp",            (context) => OnMapButton             ?.Invoke(false));
        InitializeAction("TapDown",          (context) => OnInventoryButton       ?.Invoke(true));
        InitializeAction("TapUp",            (context) => OnInventoryButton       ?.Invoke(false));
        InitializeAction("Cancel",           (context) => OnCancelButton          ?.Invoke(true));
        InitializeAction("AnyKey",           (context) => OnAnyKey                ?.Invoke());
        

    }

    

    void InitializeAction(string actionName, Action<InputAction.CallbackContext>actionMethod, Action<InputAction.CallbackContext> cancelMethod = null)
    {
        if (actionDictionary == null) return;
        if (actionDictionary.TryGetValue(actionName, out InputAction currentInput))
        {
            if(actionMethod is not null) currentInput.performed += actionMethod;
            if(cancelMethod is not null) currentInput.canceled += actionMethod;
        }
    }

    T GetInputValue<T>(InputAction.CallbackContext context) where T : struct
    {
        if (context.valueType != typeof(T)) return default;
        return context.ReadValue<T>();
    }
    Vector2 GetVector2Value(InputAction.CallbackContext context) => GetInputValue<Vector2>(context);

    void CursorPositionChanged(Vector2 screenPosition)
    //
    {
        Camera mainCamera = Camera.main;
        Physics2DRaycaster raycaster2D = mainCamera.GetComponent<Physics2DRaycaster>();
        PhysicsRaycaster raycaster = mainCamera.GetComponent<PhysicsRaycaster>();

       

        //마우스의 화면상 실제 픽셀위치
        //Vector2 screenPosition = context.ReadValue<Vector2>();
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
