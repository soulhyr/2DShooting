using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public static class Utility
{
    #region # Scene 관련

    public static void ChangeScene(string sceneName) => SceneManager.LoadScene(sceneName);
    public static void ChangeSceneAsync<T>(string sceneName, ObjectType objectType) where T : MonoBehaviour, IMain
    {
        var op = SceneManager.LoadSceneAsync(sceneName);
        op.completed += (ao) =>
        {
            var manager = GameObject.FindFirstObjectByType<T>();
            manager.Init(objectType);
        };
    }
    public static string GetAcitvateScene() => SceneManager.GetActiveScene().name;

    #endregion

    #region # 문자열 표시 관련
    
    public static string ConvertDateTimeToStirng(DateTime dateTime) => dateTime.ToString(GameDef.Formats.DateTime);
    public static string CommaString(int value, string tail = "") => $"{value:N0} {tail}";

    #endregion

    #region # 동작 방식 관련

    public static bool IsClick()
    {
        bool pcClick = Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame;
        bool mobileTouch = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;

        return pcClick || mobileTouch;
    }
    
    public static bool GetFire()
    {
        bool pcFire = false;
#if ENABLE_LEGACY_INPUT_MANAGER
        pcFire = Input.GetButton("Fire1");
#endif
        bool gamepadFire = Gamepad.current != null && Gamepad.current.buttonSouth.isPressed;
        bool mobileFire = Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame;

        return pcFire || gamepadFire || mobileFire;
    }
    
    // PC/모바일/게임패드 공용 수평 입력
    public static float GetHorizontal()
    {
        float value = 0f;

#if ENABLE_LEGACY_INPUT_MANAGER
    value += Input.GetAxisRaw("Horizontal");
#endif

        // 1. PC 키보드
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed)
                value -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed)
                value += 1f;
        }

        // 2. 게임패드
        if (Gamepad.current != null)
            value += Gamepad.current.leftStick.x.ReadValue();

        // 3. 모바일 터치 / 가상 조이스틱
        value += GetMobileHorizontal();

        return Mathf.Clamp(value, -1f, 1f);
    }

    public static float GetVertical()
    {
        float value = 0f;

#if ENABLE_LEGACY_INPUT_MANAGER
    value += Input.GetAxisRaw("Vertical");
#endif
// 1. PC 키보드
        if (Keyboard.current != null)
        {
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed)
                value += 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed)
                value -= 1f;
        }

        // 2. 게임패드
        if (Gamepad.current != null)
            value += Gamepad.current.leftStick.y.ReadValue();

        // 3. 모바일 터치 / 가상 조이스틱
        value += GetMobileVertical();

        return Mathf.Clamp(value, -1f, 1f);
    }

    // 모바일 터치용 수평 입력 계산 (예시: 화면 좌측 하단 가상 조이스틱)
    private static float GetMobileHorizontal()
    {
        if (Touchscreen.current == null || Touchscreen.current.primaryTouch.press.isPressed == false)
            return 0f;

        // 터치 위치 및 스와이프 계산
        Vector2 delta = Touchscreen.current.primaryTouch.delta.ReadValue();
        return Mathf.Clamp(delta.x / 50f, -1f, 1f); // 50은 감도 조정 가능
    }

    // 모바일 터치용 수직 입력 계산
    private static float GetMobileVertical()
    {
        if (Touchscreen.current == null || Touchscreen.current.primaryTouch.press.isPressed == false)
            return 0f;

        Vector2 delta = Touchscreen.current.primaryTouch.delta.ReadValue();
        return Mathf.Clamp(delta.y / 50f, -1f, 1f);
    }
    
    // Horizontal 버튼 눌림 감지
    public static bool GetHorizontalDown()
    {
        bool pcDown = false;
#if ENABLE_LEGACY_INPUT_MANAGER
        pcDown = Input.GetButtonDown("Horizontal");
#endif

        bool gamepadDown = Gamepad.current != null &&
                           (Gamepad.current.dpad.left.wasPressedThisFrame ||
                            Gamepad.current.dpad.right.wasPressedThisFrame ||
                            Gamepad.current.leftStick.left.wasPressedThisFrame ||
                            Gamepad.current.leftStick.right.wasPressedThisFrame);

        bool mobileDown = false;
        if (Touchscreen.current != null)
        {
            // 예: 화면 왼쪽/오른쪽 터치 구간 감지
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.wasPressedThisFrame)
                {
                    mobileDown = true;
                    break;
                }
            }
        }

        return pcDown || gamepadDown || mobileDown;
    }

    // Horizontal 버튼 떼짐 감지
    public static bool GetHorizontalUp()
    {
        bool pcUp = false;
#if ENABLE_LEGACY_INPUT_MANAGER
        pcUp = Input.GetButtonUp("Horizontal");
#endif

        bool gamepadUp = Gamepad.current != null &&
                         (Gamepad.current.dpad.left.wasReleasedThisFrame ||
                          Gamepad.current.dpad.right.wasReleasedThisFrame ||
                          Gamepad.current.leftStick.left.wasReleasedThisFrame ||
                          Gamepad.current.leftStick.right.wasReleasedThisFrame);

        bool mobileUp = false;
        if (Touchscreen.current != null)
        {
            foreach (var touch in Touchscreen.current.touches)
            {
                if (touch.press.wasReleasedThisFrame)
                {
                    mobileUp = true;
                    break;
                }
            }
        }

        return pcUp || gamepadUp || mobileUp;
    }

    #endregion
}