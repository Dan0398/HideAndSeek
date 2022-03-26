using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Interface : MonoBehaviour
{
    public static Interface instance;
    [SerializeField] RectTransform Pause, Gameplay, Endgame;
    [SerializeField] Text EndText;
    [SerializeField] Image NoiseLVL, NoiseImage, FadeImage;
    [SerializeField] Gradient NoiseGradient;
    private void Awake()
    {
        instance = this;
        StartCoroutine(FadeOpen(false));
    }
    public void AnimateNoise(float ValueNorml)
    {
        ValueNorml = Mathf.Clamp(ValueNorml, 0, 1);
        NoiseLVL.fillAmount = ValueNorml;
        NoiseImage.color = NoiseGradient.Evaluate(ValueNorml);
    }
    public void Restart()
    {
        StartCoroutine(ChangeScene(1));
    }
    public void GoToMenu()
    {
        StartCoroutine(ChangeScene(0));
    }
    IEnumerator ChangeScene(int SceneNumber)
    {
        yield return StartCoroutine(FadeOpen(true));
        SetTimeScale(false);
        UnityEngine.SceneManagement.SceneManager.LoadScene(SceneNumber);
    }
    public void SetTimeScale(bool isPaused)
    {
        Time.timeScale = isPaused?0 : 1;
    }
    public void End(bool AreYouWinningSon)
    {
        SetTimeScale(true);
        Endgame.gameObject.SetActive(true);
        Pause.gameObject.SetActive(false);
        Gameplay.gameObject.SetActive(false);
        if (AreYouWinningSon) EndText.text = "Handsome!";
        else EndText.text = "Bad luck";
        MapMaker.Instance.StopMusic();
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
}
