using System;
using UnityEngine;

public class ViewBase : MonoBehaviour
{
    protected Events Events;
    
    private void Awake()
    {
        Events = Events.Get();
    }
    private void OnDestroy()
    {
        Events.OnUnAllSubscribe();
        ((IDisposable)Events).Dispose();
    }

    private void OnEnable()
    {
        OnSubscribe();
    }
    protected virtual void OnSubscribe()
    {
        
    }

    private void OnDisable()
    {
        Events.OnUnAllSubscribe();
    }
}
