using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneFader : MonoBehaviour
{
    [Header("UI Reference")]
    public CanvasGroup fadeCanvasGroup; // 방금 만든 FadeOverlay의 Canvas Group

    [Header("Settings")]
    public float fadeDuration = 1.0f;   // 화면이 완전히 어두워지는 데 걸리는 시간 (1초)
    public string nextSceneName = "InGameScene"; // 이동할 다음 씬 이름

    private bool isFading = false; // 중복 클릭 방지용 변수

    // '게임 시작' 버튼에 연결할 함수입니다.
    public void OnClickGameStart()
    {
        // 이미 페이드가 진행 중이라면 버튼 입력을 무시합니다.
        if (isFading) return;

        StartCoroutine(FadeAndLoadScene());
    }

    IEnumerator FadeAndLoadScene()
    {
        isFading = true;

        // 페이드 아웃이 시작되면 마우스 클릭이 뒤에 있는 버튼들에 닿지 않도록 막아줍니다.
        fadeCanvasGroup.blocksRaycasts = true;

        float timer = 0f;

        // 1초 동안 검은 암막을 서서히 불투명(Alpha: 1)하게 만듭니다.
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            yield return null;
        }

        // 혹시 모를 오차를 위해 확실하게 1로 세팅
        fadeCanvasGroup.alpha = 1f;

        // 화면이 완전히 깜깜해진 이 순간, 다음 씬을 로드합니다!
        SceneManager.LoadScene(nextSceneName);
    }
}