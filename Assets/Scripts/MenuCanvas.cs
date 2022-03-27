using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MenuCanvas : MonoBehaviour
{
    [SerializeField] Image FadeImage; 

    async void Start()
    {
        await HideFade();
    }

    public async void StartTheGame() 
    {
        await ShowFade();
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    public async void Quit()
    {
        await ShowFade();
        Application.Quit();
    }

    async Task ShowFade() 
    {
        await AnimateFade(true);
        
    }

    async Task HideFade()
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