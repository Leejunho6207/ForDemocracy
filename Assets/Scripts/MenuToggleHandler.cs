using UnityEngine;

public class MenuToggleHandler : MonoBehaviour
{
    [Header("껐다 켰다 할 대상 오브젝트")]
    public GameObject PlayerHand;

    // 버튼을 누를 때마다 이 함수가 실행되면서 알아서 켜고 끕니다.
    public void ToggleObject()
    {
        if (PlayerHand == null)
        {
            Debug.LogError("대상 오브젝트가 지정되지 않았습니다!");
            return;
        }

        // 현재 켜져있으면 끄고, 꺼져있으면 켜주는 핵심 한 줄입니다.
        bool isActive = PlayerHand.activeSelf;
        PlayerHand.SetActive(!isActive);

        Debug.Log($"{PlayerHand.name}의 상태를 {!isActive}로 변경했습니다.");
    }
}