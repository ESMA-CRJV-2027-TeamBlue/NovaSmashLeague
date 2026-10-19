using System;
using UnityEngine;

[Serializable]
public class ActionPhaseFrames
{
    [Min(1)] public int startup = 10;
    [Min(1)] public int active = 1;

    [Min(0)] public int recovery = 20;

    public int Total => startup + active + recovery;
}