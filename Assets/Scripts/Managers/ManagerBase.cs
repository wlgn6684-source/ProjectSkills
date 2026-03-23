using System.Collections;
using UnityEngine;

//class     : 변수 o /함수 내용 o /객체생성 o

//abstract  : 변수 o /함수 내용 세모 / 객체생성 x
//추상적인
//interface : 변수 x /함수 내용 x /객체생성 x
//instance : 실제 객체


public abstract class ManagerBase : MonoBehaviour
{
    GameManager _connectedManager;

    public virtual int LoadCount => 1;

    //Connect를 자유롭게 하기 위해서 Virtual을 사용
    //virtual을 쓰려면 생각해야 하는것
    //OCP => 개방폐쇄원칙 확장에는 열려있으나 수정에는 닫혀있게끔
    public IEnumerator Connect(GameManager newManager)
    { 

        _connectedManager = newManager;
        yield return OnConnected(newManager);
    }

    protected abstract IEnumerator OnConnected(GameManager newManager);
    protected abstract void OnDisconnected();
    public void Disconnected()
    {
        _connectedManager = null;
        OnDisconnected();
    }
}
