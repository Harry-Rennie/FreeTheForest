using UnityEngine;
using UnityEngine.Events;

public class TargetSlot : MonoBehaviour
{
    public UnityEvent OnChildChanged = new UnityEvent();
    private void OnTransformChildrenChanged()
    {
        bool hasChildren;
         OnChildChanged.Invoke();
    }
}
