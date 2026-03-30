using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

//확장 메소드들을 가지고 있을 친구들
//영토 확장
//확장판
//원본에 없던 개념을 추가
//내가 원하는 클래스에 내가 원하는 기능을 추가해줄 수 있다.

// ex) Normalize는 본인을 1로 바꾸는것
//     normalized는 크기가 1인 값을 돌려주는 것
//     float를 정규화하면 어떤 모양이 될까?
//      13.0f => 1.0f
//      -2.0f => -1.0f
//      0.0f => 0.0f
//      float => float
public static class Extensions
{
    public static float normalized(this float target)
    {
        if (target > 0)      return 1.0f;
        else if (target < 0) return -1.0f;
        else                 return 0.0f;
    }

    // Try Add Component => 추가를 시도 => 있는지 확인 => 없으면 추가
    public static T TryAddComponent<T>(this GameObject target) where T : Component
    { 
        T result = null;

        if (target == null) return result; //RVO

        result = target.GetComponent<T>() ?? target.AddComponent<T>();
        //if (result is null) result = target.AddComponent<T>();
        //result ??= target.AddComponent<T>();

        return result;
    }

    public static T TryAddComponent<T>(this Component target) where T : Component
    {
        if (target == null) return null;
        else                return target.gameObject.TryAddComponent<T>(); //NRVO
    }

    public static IEnumerator WaitforTask(this Task targetTask)
    {
        yield return new WaitUntil(() => targetTask.IsCompleted);
        targetTask.Dispose();
    }
}
