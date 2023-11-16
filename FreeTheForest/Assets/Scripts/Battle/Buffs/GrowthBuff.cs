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
        PlayerInfoController.instance.ModifyStrength(-stacks);
        PlayerInfoController.instance.ModifyDefence(-stacks);
    }

    public override void Apply()
    {
        target.strength++;
        target.defence++;
        target.battleManager.energyGain++;
        PlayerInfoController.instance.ModifyStrength(1);
        PlayerInfoController.instance.ModifyDefence(1);
    }

    public override void Tick()
    {
        //For growth, nothing.
    }
}
