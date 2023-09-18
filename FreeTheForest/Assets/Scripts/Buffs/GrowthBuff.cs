using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowthBuff : BuffBase
{
    public GrowthBuff()
    {
        isPermanent = true;
        canStack= true;
    }

    public override void End()
    {
        target.offense -= stacks;
        target.defense -= stacks;
        target.battleManager.energyGain-= stacks;
    }

    public override void Apply()
    {
        target.offense++;
        target.defense++;
        target.battleManager.energyGain++;
    }
}
