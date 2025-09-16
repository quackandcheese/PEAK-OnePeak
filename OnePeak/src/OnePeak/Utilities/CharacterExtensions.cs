using System;
using System.Collections.Generic;
using System.Text;

namespace OnePeak.Utilities;

public static class CharacterExtensions
{
    public static bool HasEatenDevilFruit(this Character character)
    {
        foreach (var fruit in DevilFruit.AllFruits)
        {
            if (character.refs.afflictions.GetCurrentStatus(fruit.Status) > 0.0)
                return true;
        }
        return false;
    }
    public static bool HasEatenDevilFruit<T>(this Character character) where T : DevilFruit<T>
    {
        return (character.refs.afflictions.GetCurrentStatus(DevilFruit<T>.Instance.Status) > 0.0);
    }
    public static bool HasEatenDevilFruit(this Character character, DevilFruit fruit)
    {
        return (character.refs.afflictions.GetCurrentStatus(fruit.Status) > 0.0);
    }
}
