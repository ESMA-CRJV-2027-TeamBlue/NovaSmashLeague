using System;
using UnityEngine;

public interface IDeathSource
{
  bool IsDead { get; }
    event Action Died;
}
