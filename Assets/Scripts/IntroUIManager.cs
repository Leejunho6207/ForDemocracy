using UnityEngine;
using System.Collections;

public class IntroUIManager : MonoBehaviour
{
    [Header("Canvas Groups")]
    public CanvasGroup loadingGroup; // 로딩 문구의 Canvas Group
    public CanvasGroup skipGroup;    // 스킵 문구의 Canvas Group

    [Header("Time Settings")]
    public float loadingDuration = 3.0f; // 로딩 문구가 유지되는 시간 (3초)
    public float fadeDuration = 1.0f;    // 페이드에 걸리는 시간 (1초)
    public float skipAppearanceTime = 5.0f; // 스킵 문구가 나타나기 시작할 시점 (5초)

    void Start()
    {
        // 시작할 때 초기 상태 설정
        if (loadingGroup != null) loadingGroup.alpha = 1f;
        if (skipGroup != null) skipGroup.alpha = 0f;

        // 순차적 타임라인 코루틴 시작
        StartCoroutine(TimelineSequence());
    }

    IEnumerator TimelineSequence()
    {
        // ==========================================
        // 1단계: 로딩 문구 처리 (0초 ~ 4초 구간)
        // ==========================================
        // 3초 동안 로딩 문구 유지
        yield return new WaitForSeconds(loadingDuration);

        // 1초 동안 로딩 문구 서서히 사라짐 (3초 ~ 4초)
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (loadingGroup != null)
            {
                loadingGroup.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            }
            yield return null;
        }

        if (loadingGroup != null)
        {
            loadingGroup.alpha = 0f;
            loadingGroup.gameObject.SetActive(false); // 확실하게 꺼주기
        }

        // ==========================================
        // 2단계: 빈 화면 여백 대기 (4초 ~ 5초 구간)
        // ==========================================
        // 스킵이 나타나야 하는 5초까지 남은 시간만큼 대기합니다.
        // (5초 - 3초(유지) - 1초(페이드)) = 1초 동안 여백 발생
        float remainingWait = skipAppearanceTime - (loadingDuration + fadeDuration);
        if (remainingWait > 0)
        {
            yield return new WaitForSeconds(remainingWait);
        }

        // ==========================================
        // 3단계: 스킵 문구 처리 (5초 ~ 6초 구간)
        // ==========================================
        // 1초 동안 스킵 문구 서서히 나타남 (5초 ~ 6초)
        timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (skipGroup != null)
            {
                skipGroup.alpha = Mathf.Lerp(0f, 1f, timer / fadeDuration);
            }
            yield return null;
        }

        if (skipGroup != null)
        {
            skipGroup.alpha = 1f;
        }

        Debug.Log("5초 대기 후 스킵 문구 등장 완료!");
    }
}