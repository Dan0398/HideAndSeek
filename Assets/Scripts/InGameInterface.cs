using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class InGameInterface : MonoBehaviour
{
    public static InGameInterface instance;
    [SerializeField] RectTransform Pause, Gameplay, Endgame;
    [SerializeField] Text EndText;
    [SerializeField] Image NoiseLVL, NoiseImage, FadeImage;
    [SerializeField] Gradient NoiseGradient;

    private void Awake()
    {
        instance = this;
        HideFade();
    }

    public static void ApplyNoiseLevel(float NoizeNormalized)
    {
        if (instance == null) return;
        NoizeNormalized = Mathf.Clamp(NoizeNormalized, 0, 1);
        instance.NoiseLVL.fillAmount = NoizeNormalized;
        instance.NoiseImage.color = instance.NoiseGradient.Evaluate(NoizeNormalized);
    }

    public async void Restart()
    {
        SetPause();
        await ShowFade();
        HideEndGameUI();
        MapMaker.BuildNewScene();
        ContinuePlaying();
        HideFade();
    }

    public async void GoToMenu()
    {
        ContinuePlaying();
        await ShowFade();
        ContinuePlaying();
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public static void SetPause()
    {
         Time.timeScale = 0;
    }

    public static void ContinuePlaying()
    {
        Time.timeScale = 1;
    }

    public static void GameOver()
    {
        if (instance == null) return;
        SetPause();
        instance.EndText.text = "Bad luck";
        ShowEndGameUI();
        //MapMaker.Instance.StopMusic();
    }

    static void ShowEndGameUI()
    {
        instance.Endgame.gameObject.SetActive(true);
        instance.Pause.gameObject.SetActive(false);
        instance.Gameplay.gameObject.SetActive(false);
    }

    static void HideEndGameUI()
    {
        instance.Endgame.gameObject.SetActive(false);
        instance.Pause.gameObject.SetActive(false);
        instance.Gameplay.gameObject.SetActive(true);
    }

    public static void LevelCompleted()
    {
         if (instance == null) return;
        SetPause();
        instance.EndText.text = "Handsome!";
        ShowEndGameUI();
        //MapMaker.Instance.StopMusic();
    }

    async Task ShowFade() 
    {
        await AnimateFade(true);
    }

    async void HideFade()
    {
        await AnimateFade(false);
    }

    async Task AnimateFade(bool isInvoking)
    {
        if (isInvoking) FadeImage.gameObject.SetActive(true);
        float Lerp = 0;
        for (int i = 0; i <= 90; i+=3)
        {
            Lerp = Mathf.Sin(i * Mathf.Deg2Rad);
            if (!isInvoking) Lerp = 1 - Lerp;
            FadeImage.color = Color.black * Lerp;
            await Task.Delay(30);
        }
        if (!isInvoking) FadeImage.gameObject.SetActive(false);
    }
}