using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class CharacterMenu : MonoBehaviour
{
    [SerializeField] HPType hptyp;

    [SerializeField] Stats plStats;
    [SerializeField] TextMeshProUGUI Weakneses;
    [SerializeField] TextMeshProUGUI Resistances;
    [SerializeField] TextMeshProUGUI Hp;
    [SerializeField] TextMeshProUGUI Speed;
    public void OnEnable()
    {
        Weakneses.text = returnDmgTypes(hptyp.weaknesses);
        Resistances.text = returnDmgTypes(hptyp.resistances);
        Hp.text = plStats.hp.ToString();
        Speed.text = plStats.speed.ToString();
    }

    public string returnDmgTypes(List<DmgTypeHolder> list ) 
    {
        string a = "";
        for (int i = 0; i < list.Count; i++)
        {
            a += list[i].dmgType.name + " " + (list[i].value * 100) + "%" + "\n";
        }
        return a;
    }
}
