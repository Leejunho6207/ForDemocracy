using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class FactionButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Faction Settings")]
    public string factionName; // "Jorsen" 또는 "Cordia"
    public FactionButton oppositeFaction; // 반대편 진영 버튼 스크립트 연결

    private CanvasGroup canvasGroup;
    private Vector3 originalScale;
    private Vector3 targetScale;

    [Header("Animation Settings")]
    public float animationSpeed = 8f;

    void Start()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        // 마우스 호버 시 크기를 부드럽게 키우거나 줄이는 연출 (Lerp)
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * animationSpeed);
    }

    // 1. 마우스를 올렸을 때 (Hover In)
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 내 진영은 10% 확대
        targetScale = originalScale * 1.1f;

        // 상대 진영은 부드럽게 어둡게 만듦 (CanvasGroup Alpha를 0.4로 조정)
        if (oppositeFaction != null && oppositeFaction.canvasGroup != null)
        {
            oppositeFaction.StopAllCoroutines();
            oppositeFaction.StartCoroutine(oppositeFaction.FadeAlpha(0.4f));
        }
    }

    // 2. 마우스를 치웠을 때 (Hover Out)
    public void OnPointerExit(PointerEventData eventData)
    {
        // 내 진영 크기 원래대로
        targetScale = originalScale;

        // 상대 진영 밝기도 원래대로 (Alpha 1.0)
        if (oppositeFaction != null && oppositeFaction.canvasGroup != null)
        {
            oppositeFaction.StopAllCoroutines();
            oppositeFaction.StartCoroutine(oppositeFaction.FadeAlpha(1.0f));
        }
    }

    // 3. 진영을 최종 클릭(선택)했을 때
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"{factionName} 진영이 선택되었습니다!");

        // 데이터 저장 (예: PlayerPrefs를 이용해 어떤 진영을 골랐는지 저장하고 다음 씬에서 활용)
        PlayerPrefs.SetString("SelectedFaction", factionName);

        
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameSystem");
    }

    // 알파값을 부드럽게 바꿔주는 코루틴 함수
    public IEnumerator FadeAlpha(float targetAlpha)
    {
        float timer = 0f;
        float startAlpha = canvasGroup.alpha;
        float duration = 0.2f; // 0.2초 동안 부드럽게 변함

        while (timer < duration)
        {
            timer += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / duration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
}