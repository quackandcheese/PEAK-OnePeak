using System;
using System.Collections.Generic;
using System.Text;

namespace OnePeak;

public class Action_ApplyDevilFruitStatus : ItemAction
{
    public CharacterAfflictions.STATUSTYPE devilFruitStatus;

    public override void RunAction()
    {
        character.refs.afflictions.AddStatus(devilFruitStatus, 0.1f);
    }
}
