using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrenShakeInstatitor : MonoBehaviour
{
    public void StartScrenShake(float a)
    {
        CinemachineShake.Instance.ShakeCamera(a, 0.4f);
    }

}
