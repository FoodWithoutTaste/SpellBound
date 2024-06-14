using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class PL_Stats : Stats
{
    [Header("Properties")]
    public TextMeshProUGUI skillPointsText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelText2;
    public GameObject skillPoint1;
    public GameObject skillPoint2;
    public GameObject skillPoint3;

    [SerializeField] UiBar uiBarXP;
    [SerializeField] UiBar uiBarMana;
    public override void Start()
    {
        base.Start();
        uiBarXP.Setup(() => xp, xp, maxXp);
        uiBarMana.Setup(() => mana, mana, maxMana);
    }
   
    public override void AddLevel()
    {

        base.AddLevel();
        uiBarXP.Setup(() => xp, xp, maxXp);
        skillPoint1.SetActive(true); 
        skillPoint2.SetActive(true);
        skillPoint3.SetActive(true);
        skillPointsText.text = "Skill Points : " + skillPoint;
        levelText.text = "LVL:" + level;
        levelText2.text = "LVL:" + level;
        
    }

    public void DisableSkillButtons()
    {
        skillPointsText.text = "Skill Points : " + skillPoint;
        skillPoint1.SetActive(false);
        skillPoint2.SetActive(false);
        skillPoint3.SetActive(false);
    }
}
