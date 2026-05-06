using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraManager : ManagerBase
{
    public Camera MainCamera { get; private set; }

    protected override IEnumerator OnConnected(GameManager newManager)
    {
        SetMainCamera(Camera.main);
        yield return null;
    }
    protected override void OnDisconnected()
    {

    }

    public void SetMainCamera(Camera wantCamera)
    { 
        MainCamera = wantCamera;
       
    }

    
    public void GetRaycastResult(Vector3 screenPosition, List<RaycastResult> outResult)
    {
        EventSystem currentEvent = EventSystem.current;
        if (!currentEvent) return;
        PointerEventData eventData = new(EventSystem.current);
        eventData.position = screenPosition;
        currentEvent.RaycastAll(eventData, outResult);
        
    }
}
