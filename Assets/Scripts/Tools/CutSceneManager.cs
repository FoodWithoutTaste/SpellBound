using UnityEngine;
using System.Collections.Generic;
using Cinemachine;

[System.Serializable]
public class Cutscene
{
    public string name;
    public CinemachineVirtualCamera cam;
    public float cutSceneDuration;
}

public class CutSceneManager : MonoBehaviour
{
    public static CutSceneManager Instance { get; private set; }

    public List<Cutscene> cutscenes = new List<Cutscene>();

    private Dictionary<string, Cutscene> cutsceneDictionary = new Dictionary<string, Cutscene>();
    private int currentCameraIndex = 0;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        
        foreach (var cutscene in cutscenes)
        {
            cutsceneDictionary[cutscene.name] = cutscene;
        }
    }

    private void Start()
    {
       
        foreach (var cutscene in cutscenes)
        {
            cutscene.cam.gameObject.SetActive(false);
        }
    }

    public void PlayCutscene(string cutsceneName)
    {
        if (cutsceneDictionary.ContainsKey(cutsceneName))
        {
            var cutscene = cutsceneDictionary[cutsceneName];
          
            cutscene.cam.gameObject.SetActive(true);
            var anim = cutscene.cam.GetComponent<Animator>();
            if (anim != null)
            {
                anim.Play("First");
            }
            cutscene.cam.Priority = 20; 
            StartCoroutine(DeactivateCameraAfterDuration(cutscene.cam, cutscene.cutSceneDuration));
        }
        else
        {
            Debug.LogWarning("Cutscene with name " + cutsceneName + " not found!");
        }
    }

    private System.Collections.IEnumerator DeactivateCameraAfterDuration(CinemachineVirtualCamera cam, float duration)
    {
        yield return new WaitForSeconds(duration);
        cam.gameObject.SetActive(false);
    }
}
