using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public bool canChangeSettings;

    public float transitiontime = 1f;
    public Animator transition;
    public static UIManager instance;

    public GameObject[] menues;

    public Dropdown resolutionDropdown;

    [Header("Audio")]
    public AudioSource uiSource;
    public AudioClip ClickClip;

    Resolution[] resolutions;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.LogWarning("UIManager instance already exist, destroing object");
            Destroy(this);
        }
    }

    private void Start()
    {
        if (canChangeSettings)
        {
            resolutions = Screen.resolutions;

            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();

            int currentResolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + "x" + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    currentResolutionIndex = i;
                }
            }

            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }
        transition = GetComponentInChildren<Animator>();

        if (transition == null)
            Debug.LogWarning(gameObject.name + "you need to assign me an animator so that i can fade between scenes");

        //select a button for the controller
        if (menues.Length != 0)
        {
            menues[0].GetComponentInChildren<Button>().Select();
        }
    }

    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
        PlayClick();
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayClick();
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        PlayClick();
    }

    public void ChangeScene(int index)
    {
        StartCoroutine(LoadLevel(index));
    }

    IEnumerator LoadLevel(int level)
    {
        if (transition != null)
            transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitiontime);
        SceneManager.LoadScene(level);
    }

    //Changes the menu
    public void ChangeMenu(GameObject menu)
    {

        for (int i = 0; i < menues.Length; i++)
        {
            menues[i].SetActive(false);
        }
        
        menu.SetActive(true);
        menu.GetComponentInChildren<Button>().Select();
        PlayClick();
    }

    void PlayClick()
    {
        uiSource.clip = ClickClip;
        uiSource.Play();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
