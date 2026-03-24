using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.InputSystem;

//          
//       대리자 => 기능을 전달 위임
// 플레이어가 할 일 대리 뛰어주고, 열려있는 창이 있다면 그 친구의 기능도 수행하고 간다!
public delegate void MouseDownEvent(Vector3 position);
public delegate void MouseUpEvent(Vector3 position);
public delegate void MouseMoveEvent(Vector2 screenPosition, Vector3 worldPosition);


//대상 변수나 클래스 위쪽에 []에 내용을 넣는 것을 Attribute : 속성이라고 한다.
[RequireComponent(typeof(PlayerInput))]

public class InputManager : ManagerBase
{
    public static event MouseDownEvent OnLeftMouseDown;
    public static event MouseDownEvent OnRightMouseDown;
    public static event MouseUpEvent OnLeftMouseUp;    
    public static event MouseUpEvent OnRightMouseUp;        
    public static event MouseMoveEvent OnMouseMove;    
    
    
    PlayerInput targetInput;
    Dictionary<string, InputAction> actionDictionary = new();

    public bool is2D = true;

    protected override IEnumerator OnConnected(GameManager newManager)
    {   
        targetInput = GetComponent<PlayerInput>();
        LoadAllActions();
        InitializeAllActions();
        
        yield return null;
    }
    protected override void OnDisconnected()
    {

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
        if (actionDictionary.TryGetValue("CursorPositionChanged", out InputAction cursorPositionChange))
        {
            cursorPositionChange.performed += CursorPositionChanged;
        }

    }
    void CursorPositionChanged(InputAction.CallbackContext context)
    {
        //마우스의 화면상 실제 픽셀위치
        Vector2 screenPosition = context.ReadValue<Vector2>();
        Debug.Log(screenPosition);
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
        
        //대리자는 모든 스킬을 한번에 사용 할 수 있지만 가르쳐주지 않으면 아무것도 할 수 없다
        OnMouseMove?.Invoke(screenPosition, worldPosition); 
    }
}
