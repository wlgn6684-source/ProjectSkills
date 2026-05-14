using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingManager : ManagerBase
{
    protected override IEnumerator OnConnected(GameManager newManager)
    {   
        Screen.autorotateToLandscapeLeft        = true;
        Screen.autorotateToLandscapeRight       = true;
        Screen.autorotateToPortrait             = false;
        Screen.autorotateToPortraitUpsideDown   = false;

        //Screen.orientation = ScreenOrientation.LandscapeLeft; //화면 방향 고정
        //Screen.sleepTimeout.SystemSetting; //시스템 설정을 따르도록 설정
        Screen.sleepTimeout = SleepTimeout.SystemSetting;   
                                         //NeverSleep; //화면이 꺼지지 않도록 설정

        yield return null;
    }
    protected override void OnDisconnected()
    {

    }

   
}
