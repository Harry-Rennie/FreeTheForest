using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BuffBase
{
    public int stacks;
    public string buffName; //Name for buff. Used for comparing when re-applying buffs
    public bool canStack; //Can we have more than 1 stack of this buff?
    public bool isPermanent; //Should we reduce stacks by 1 each round for this buff? Set to false for custom stack drain behaviour.
    public Entity target; //What entity does this buff act on?
    public bool eachTurn; //Does this buff do something each turn?

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

    public abstract void Tick();
}
