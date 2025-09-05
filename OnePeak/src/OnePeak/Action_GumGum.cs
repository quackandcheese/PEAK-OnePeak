using System;
using System.Collections.Generic;
using System.Text;

namespace OnePeak;

public class Action_GumGum : ItemAction
{
    public override void RunAction()
    {
        character.refs.afflictions.AddStatus(Plugin.GumGumStatus, 0.1f);
    }
}
