using UnityEngine;

public class OpenableUIBase : UIBase, IOpenable
{
    public bool IsOpen => gameObject.activeSelf;
    public void Open() => gameObject.SetActive(true);
    public void Close() => gameObject.SetActive(false);
    public void Toggle() => gameObject.SetActive(!IsOpen);
}
