using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuCanvas : MonoBehaviour
{
    [SerializeField] Image FadeImage; 
    void Start()
    {
        StartCoroutine(FadeOpen(false));
    }
    public void StartTheGame() {
        StartCoroutine(GoNextLVL());
    }
    public void Quit()
    {
        StartCoroutine(AppQuit());
    }
    IEnumerator AppQuit()
    {
        yield return StartCoroutine(FadeOpen(true));
        Application.Quit();

    }
    IEnumerator FadeOpen(bool GoingDark)
    {
        if (GoingDark)
        {
            FadeImage.gameObject.SetActive(true);
            for (int i = 0; i <= 30; i++)
            {
                FadeImage.color = Color.black * Mathf.Sin(i * 3 * Mathf.Deg2Rad);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            for (int i = 0; i <= 30; i++)
            {
                FadeImage.color = Color.black * Mathf.Cos(i * 3 * Mathf.Deg2Rad);
                yield return new WaitForEndOfFrame();
            }
            FadeImage.gameObject.SetActive(false);
        }
        yield return null;
    }
    IEnumerator GoNextLVL()
    {
        yield return FadeOpen(true);
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
}
