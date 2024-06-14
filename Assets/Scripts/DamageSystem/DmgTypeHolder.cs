
using UnityEditor;
using UnityEngine;
[System.Serializable]
public class DmgTypeHolder
{
    public DmgType dmgType;
    [Range(0.0f, 1.0f)]
    public float value;


    public DmgTypeHolder(DmgType dmgType, int value)
    {
        this.dmgType = dmgType;
        this.value = value;
    }
}
