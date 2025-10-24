using UnityEngine;

public interface IMain
{
    /// <summary>
    /// Scene 전환을 비동기로 처리하고 처리 완료 시 호출용.
    /// Awake 와 Start 사이에서 실행됨에 주의.
    /// </summary>
    void Init(object param = null);
}