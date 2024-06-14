using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimLayerController : MonoBehaviour
{
    public Animator animRefrence;
    private int x;
    private int y;
    public float time;
    public void ChangeLayers(int x,int y)
    {
        this.x = x;
        this.y = y;
    }
    void Update()
    {
        animRefrence.SetLayerWeight(x, Mathf.Lerp(animRefrence.GetLayerWeight(x), y, time));
    }
}
