using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSkillBar : MonoBehaviour
{
    [SerializeField] List<GameObject> BarPoints;
    [SerializeField] PL_Stats playerStats;
    int max = 0;
    private void Start()
    {
        

       
    }

    public void AddPoint(int index)
    {
        max++;
        for (int i = 0; i < BarPoints.Count; i++)
        {
            if(i <= max)
            {
                BarPoints[i].SetActive(true);
            }
            else
            {
                BarPoints[i].SetActive(false);
            }
           
        }

        switch (index)
        {
            case 0:
                playerStats.skillHpModifier += 0.1f;
                break;
            case 1:
                playerStats.skillManaModifier += 0.1f;
                break;
            case 2:
                playerStats.skillSpeedModifier += 0.1f;
                break;
            case 3:
                playerStats.skillDmgModifier += 0.1f;
                break;
            default:
                Debug.LogWarning("Invalid skill index.");
                break;
        }

        // Reduce skill points
        playerStats.skillPoint--;
        playerStats.DisableSkillButtons();

    }
}
