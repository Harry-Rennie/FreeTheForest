using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthBuff : BuffBase
{
    public GrowthBuff()
    {
        isPermanent = true;
        canStack= true;
        buffName = "Growth";
    }

    public override void End()
    {
        target.strength -= stacks;
        target.defence -= stacks;
        target.battleManager.energyGain-= stacks;
    }

    public override void Apply()
    {
        target.strength++;
        target.defence++;
        target.battleManager.energyGain++;
    }

    public override void Tick()
    {
        //For growth, nothing.
    }
}
