using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BuffBase
{
    public int stacks;
    public bool canStack;
    public bool isPermanent;
    public Entity target;

    public void Activate()
    {
        if (canStack)
        {
            Apply();
            stacks++;
        }
        else
        {
            Apply();
        }
    }

    public abstract void Apply();

    public abstract void End();
}
