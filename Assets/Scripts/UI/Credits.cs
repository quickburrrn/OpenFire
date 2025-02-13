using System.Collections;
using UnityEngine;


public class Credits : MonoBehaviour
{
    public GameObject[] pages;
    public int currentPage = 0;
    [Tooltip("time for each page")]public float time = 1f;

    float alpha;

    private void Start()
    {
        StartCoroutine(countdown(time+1f));
    }

    IEnumerator countdown(float time_)
    {
        //in
        InvokeRepeating("FadeIn", 0f, 0.01f);
        yield return new WaitForSeconds(time_ / 3f);
        //stopin
        CancelInvoke("FadeIn");
        yield return new WaitForSeconds(time_/3f);
        InvokeRepeating("FadeOut", 0f, 0.01f);
        yield return new WaitForSeconds(time_/3f);
        SwichPage();
        //stopout
        CancelInvoke("FadeOut");
        StartCoroutine(countdown(time));
    }

    void SwichPage()
    {
        for (int i = 0; i < pages.Length; i++)
        {
            pages[i].gameObject.SetActive(false);
        }

        currentPage++;

        if (currentPage == pages.Length-1)
        {
            Destroy(this,time/3);
        }

        pages[currentPage].gameObject.SetActive(true);
    }

    void FadeOut()
    {
        CanvasGroup canvasGroup = pages[currentPage].GetComponent<CanvasGroup>();
        canvasGroup.alpha -= .01f;
    }

    void FadeIn()
    {
        CanvasGroup canvasGroup = pages[currentPage].GetComponent<CanvasGroup>();
        canvasGroup.alpha += .01f;
    }
}
