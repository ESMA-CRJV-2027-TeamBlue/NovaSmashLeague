using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "ScriptableObjects/Character", order = 0)]
public class SO_Character : ScriptableObject
{
    [Tooltip("Nom du personnage.")]
    public string characterName;
    [Tooltip("Prefab Comportant les informations visuels du personnage.")]
    public GameObject visualCharacter;
}
