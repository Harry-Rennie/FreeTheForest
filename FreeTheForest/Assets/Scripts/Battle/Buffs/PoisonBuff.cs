using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonBuff : BuffBase
{
    public PoisonBuff()
    {
        isPermanent = true;
        canStack = true;
        eachTurn = true;
        buffName = "Poison";
    }

    public override void End()
    {
        return;
    }

    public override void Apply()
    {
        return;
    }

    public override void Tick()
    {
        //Damage target equal to stacks, then reduce stacks by half, or vanish if we're on one stack
        target.TakeDamage(stacks);

        if (stacks <= 1)
        {
            target.buffs.Remove(this);
        }
        else
        {
            stacks /= 2;
        }
    }
}

