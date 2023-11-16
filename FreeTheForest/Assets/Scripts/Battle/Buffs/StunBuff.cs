using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//For enemies only. If an enemy has a stack of stun, they will skip their next turn.
public class StunBuff : BuffBase
{
    public StunBuff()
    {
        isPermanent = false;
        canStack = true;
        eachTurn = false;
        buffName = "Stun";
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
        return;
    }
}
