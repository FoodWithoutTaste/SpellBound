/* 
    ------------------- Code Monkey -------------------

    Thank you for downloading this package
    I hope you find it useful in your projects
    If you have any questions let me know
    Cheers!

               unitycodemonkey.com
    --------------------------------------------------
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CinemachineShake : MonoBehaviour {

    public static CinemachineShake Instance { get; private set; }

    public CinemachineVirtualCamera[] cinemachineVirtualCamera;
    public float shakeTimer;
    public float shakeTimerTotal;
    public float startingIntensity;
    
    private void Awake() {
        Instance = this;
  
    }

    public void ShakeCamera(float intensity, float time) {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = GetVCam().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = intensity;

        startingIntensity = intensity;
        shakeTimerTotal = time;
        shakeTimer = time;
    }
    public CinemachineVirtualCamera GetVCam()
    {
        int a = 0;
        int priorityy = 0;
        for (int i = 0; i < cinemachineVirtualCamera.Length; i++)
        {
            if(cinemachineVirtualCamera[i].Priority > priorityy)
            {
                priorityy = cinemachineVirtualCamera[i].Priority;
                a = i;
            }
        }
        return cinemachineVirtualCamera[a];
    }

    private void Update() {
        if (shakeTimer > 0) {
            shakeTimer -= Time.deltaTime;
            CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin =
                GetVCam().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            cinemachineBasicMultiChannelPerlin.m_AmplitudeGain = 
                Mathf.Lerp(startingIntensity, 0f, 1 - shakeTimer / shakeTimerTotal);
        }
    }

}
