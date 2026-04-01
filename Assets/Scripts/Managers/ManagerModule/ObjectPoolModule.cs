using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;
using UnityEngine.UI;

public class ObjectPoolModule
{
    PoolSetting _setting;
    public PoolSetting Setting => _setting;

    Transform rootTransform;

    Queue<GameObject> prepareQueue = new();

    List<GameObject> inProgressList = new();

    public ObjectPoolModule(PoolSetting newSetting)
    { 
        _setting = newSetting;
        
    }
    public void Initialize()
    {
        rootTransform = new GameObject(Setting.poolName).transform;
        Setting.target?.TryAddComponent<PooledObject>();
        PrepareObjects(Setting.countInitial);

    }

    GameObject PrepareObject()
    {
        if (!Setting.target) return null;
        GameObject result = ObjectManager.CreateObject(Setting.target, rootTransform);
        EnqueueObject(result);
        return result;
    }
    
    //uint 마이너스가 존재하면 안됨
    //unsigned 부호없는
    void PrepareObjects(uint count)
    {
        if (!Setting.target) return;
        for (int i = 0; i < count; i++)
        {
            GameObject result = CreateFromPrefab();
            EnqueueObject(result);

        }
    }

    void PrepareObjects(uint count, out GameObject activeObject)
    {
        if (!Setting.target) 
        {   
            activeObject = null;
            return;
        }

        activeObject = CreateFromPrefab();

        for (int i = 1; i < count; i++)
        {
            GameObject result = CreateFromPrefab();
            EnqueueObject(result);
        }
    }

    public GameObject CreateFromPrefab()
    {
        GameObject result = ObjectManager.CreateObject(Setting.target, rootTransform);
        if (result)
        { 
            result.name = Setting.poolName;
            if (result.TryGetComponent(out PooledObject pool ))
            {
                pool.OnEnqueueEvent -= DestroyObject;
                pool.OnEnqueueEvent += DestroyObject;
            }
        }
        return result;
    }
    public GameObject CreateObject(Transform parent = null)
    {
        GameObject result;
        if (!prepareQueue.TryDequeue(out result))
        {
            PrepareObjects(Setting.countAdditional, out result);
        }
        result.SetActive(true);

        if (result)
        {
            if (result.TryGetComponent(out PooledObject pool))
            {
                pool.OnDequeue();
            }
            Transform currentTransform = result.transform;
            Transform originTracsform = Setting.target.transform;
            currentTransform.SetParent(parent);
            if(currentTransform is RectTransform asRectTransform 
                && originTracsform is RectTransform originRectTransform)
            { 
                asRectTransform.anchorMin = originRectTransform.anchorMin;
                asRectTransform.anchorMax = originRectTransform.anchorMax;


                asRectTransform.pivot = originRectTransform.pivot;

                if (parent)
                {
                    LayoutRebuilder.ForceRebuildLayoutImmediate(parent.transform as RectTransform);
                }
                bool strechX = asRectTransform.anchorMin.x != asRectTransform.anchorMax.x;
                bool strechY = asRectTransform.anchorMin.y != asRectTransform.anchorMax.y;
                if (strechX || strechY)
                {   
                    asRectTransform.offsetMax = originRectTransform.offsetMax;
                    asRectTransform.offsetMin = originRectTransform.offsetMin;
                    //if (strechX)
                    //{
                        //asRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, originRectTransform.offsetMin.x, 0);
                        //asRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, -originRectTransform.offsetMax.x, 0);
                    //}
                    //if (strechY)
                    //{
                        //asRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, originRectTransform.offsetMin.y, 0);
                        //asRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, -originRectTransform.offsetMax.y, 0);
                    //}
                }
                else
                {
                    asRectTransform.anchoredPosition = originRectTransform.anchoredPosition;
                    asRectTransform.sizeDelta = originRectTransform.sizeDelta;
                }
                
            }
            else 
            {
                currentTransform.localPosition = originTracsform.localPosition;
            }
            currentTransform.localPosition = originTracsform.localPosition;
            currentTransform.localScale = originTracsform.localScale;
                        
        }

        return result;
    }

    public void DestroyObject(GameObject target)
    {
        EnqueueObject(target);
        if (target)
        { 
            target.transform.SetParent(rootTransform);
        }
    }

    public void EnqueueObject(GameObject target)
    {
        if (!target) return;
        {
            target.SetActive(false);

            target.name = Setting.poolName;
            prepareQueue.Enqueue(target);

        }
    }
}
 