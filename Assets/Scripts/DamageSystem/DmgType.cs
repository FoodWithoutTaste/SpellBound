using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDmgType", menuName = "Dmg/DmgType", order = 1)]
public class DmgType : ScriptableObject
{
    public string typeName;
    public Sprite dmgTexture;
}