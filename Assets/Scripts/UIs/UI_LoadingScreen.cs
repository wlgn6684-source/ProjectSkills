using UnityEngine;

public class UI_LoadingScreen : UIBase, IOpenable
{
    //public bool IsOpen { get; protected set; }
    public bool IsOpen => gameObject.activeSelf;

    public void Close() => gameObject.SetActive(false);
    public void Open() => gameObject.SetActive(true);
    public void Toggle() =>  gameObject.SetActive(!IsOpen);

}
