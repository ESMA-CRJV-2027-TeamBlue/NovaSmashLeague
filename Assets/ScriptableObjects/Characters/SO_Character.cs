using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Character", order = 0)]
public class SO_Character : ScriptableObject
{
    public string characterName;
    public Mesh _meshCharacter;
}
