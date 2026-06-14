using UnityEngine;
using UnityEngine.SceneManagement; // 씬 전환을 위해 필요
using System.Collections; // 코루틴을 위해 필요

public class CinematicTextScroll : MonoBehaviour
{
    [Header("Scroll Settings")]
    [Tooltip("초당 스크롤 속도 (높을수록 빠르게 올라감)")]
    public float scrollSpeed = 60f;

    [Tooltip("텍스트가 완전히 화면 밖으로 나가서 사라졌다고 판단할 Y축 위치")]
    public float endYPosition = 1200f;

    [Header("Scene Transition")]
    [Tooltip("스토리가 끝난 후 로드할 다음 인게임 씬 이름")]
    public string nextSceneName = "SingleLane"; // 요청하신 대로 SingleLane으로 기본값 설정

    [Header("UI Reference")]
    [Tooltip("스킵 안내 문구의 Canvas Group")]
    public CanvasGroup skipTextCanvasGroup;

    [Header("Time Settings")]
    [Tooltip("스킵 문구가 나타나기 시작할 시점 (초)")]
    public float skipAppearanceTime = 5.0f;
    [Tooltip("스킵 문구가 서서히 나타나는 시간 (초)")]
    public float fadeDuration = 1.0f;

    private RectTransform rectTransform;
    private bool canSkip = false; // 실제로 스킵이 가능한 상태인지 체크하는 유령 스위치

    void Start()
    {
        rectTransform = GetComponent<Transform>() as RectTransform;

        // 혹시 다른 씬에서 시간 조절을 하다가 일시 정지 상태로 넘어올 경우를 대비해 1로 고정
        Time.timeScale = 1f;

        // 게임 시작 시 스킵 문구는 안 보이게 초기화
        if (skipTextCanvasGroup != null)
        {
            skipTextCanvasGroup.alpha = 0f;
        }

        // 5초 뒤에 스킵 버튼을 활성화하는 타임라인 코루틴 시작
        StartCoroutine(SkipActivationSequence());
    }

    void Update()
    {
        // 1. 매 프레임 일정한 속도로 자막 텍스트를 위로 이동시킴
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
        }

        // 2. 자막 텍스트 위치가 완전히 위로 벗어나면 자동으로 다음 씬으로 전환
        if (rectTransform != null && rectTransform.anchoredPosition.y >= endYPosition)
        {
            LoadNextScene();
        }

        // 3. 수정된 스킵 기능: New Input System 방식
        if (canSkip && UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            LoadNextScene();
        }
    }

    // 5초 뒤에 스킵 문구를 스르륵 보여주는 시간 제어 함수
    IEnumerator SkipActivationSequence()
    {
        // 설정한 시간(5초) 동안 가만히 기다립니다.
        yield return new WaitForSeconds(skipAppearanceTime);

        // 1초 동안 스킵 문구의 알파값을 0에서 1로 서서히 올립니다 (Fade In)
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            if (skipTextCanvasGroup != null)
            {
                skipTextCanvasGroup.alpha = timer / fadeDuration;
            }
            yield return null;
        }

        // 완전히 불투명하게 고정
        if (skipTextCanvasGroup != null)
        {
            skipTextCanvasGroup.alpha = 1f;
        }

        // 이제 스페이스바를 누르면 넘어갈 수 있도록 스위치를 켭니다!
        canSkip = true;
        Debug.Log("이제 스페이스바를 눌러 스킵할 수 있습니다.");
    }

    private void LoadNextScene()
    {
        if (!string.IsNullOrEmpty("SelectScene"))
        {
            SceneManager.LoadScene("SelectScene");
        }
        else
        {
            Debug.LogWarning("SelectScene이 입력되지 않았습니다");
        }
    }
}