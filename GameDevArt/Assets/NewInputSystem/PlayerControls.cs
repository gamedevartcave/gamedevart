// GENERATED AUTOMATICALLY FROM 'Assets/NewInputSystem/PlayerControls.inputactions'

using System;
using UnityEngine;
using UnityEngine.Experimental.Input;


[Serializable]
public class PlayerControls : InputActionAssetReference
{
    public PlayerControls()
    {
    }
    public PlayerControls(InputActionAsset asset)
        : base(asset)
    {
    }
    private bool m_Initialized;
    private void Initialize()
    {
        // Player
        m_Player = asset.GetActionMap("Player");
        m_Player_Move = m_Player.GetAction("Move");
        m_Player_Look = m_Player.GetAction("Look");
        m_Player_Jump = m_Player.GetAction("Jump");
        m_Player_Dodge = m_Player.GetAction("Dodge");
        m_Player_Attack = m_Player.GetAction("Attack");
        m_Player_Shoot = m_Player.GetAction("Shoot");
        m_Player_Use = m_Player.GetAction("Use");
        m_Player_CameraChange = m_Player.GetAction("CameraChange");
        m_Player_Ability = m_Player.GetAction("Ability");
        m_Player_Pause = m_Player.GetAction("Pause");
        m_Player_LockOn = m_Player.GetAction("LockOn");
        m_Player_HeavyAttack = m_Player.GetAction("HeavyAttack");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        m_Player = null;
        m_Player_Move = null;
        m_Player_Look = null;
        m_Player_Jump = null;
        m_Player_Dodge = null;
        m_Player_Attack = null;
        m_Player_Shoot = null;
        m_Player_Use = null;
        m_Player_CameraChange = null;
        m_Player_Ability = null;
        m_Player_Pause = null;
        m_Player_LockOn = null;
        m_Player_HeavyAttack = null;
        m_Initialized = false;
    }
    public void SetAsset(InputActionAsset newAsset)
    {
        if (newAsset == asset) return;
        if (m_Initialized) Uninitialize();
        asset = newAsset;
    }
    public override void MakePrivateCopyOfActions()
    {
        SetAsset(ScriptableObject.Instantiate(asset));
    }
    // Player
    private InputActionMap m_Player;
    private InputAction m_Player_Move;
    private InputAction m_Player_Look;
    private InputAction m_Player_Jump;
    private InputAction m_Player_Dodge;
    private InputAction m_Player_Attack;
    private InputAction m_Player_Shoot;
    private InputAction m_Player_Use;
    private InputAction m_Player_CameraChange;
    private InputAction m_Player_Ability;
    private InputAction m_Player_Pause;
    private InputAction m_Player_LockOn;
    private InputAction m_Player_HeavyAttack;
    public struct PlayerActions
    {
        private PlayerControls m_Wrapper;
        public PlayerActions(PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Move { get { return m_Wrapper.m_Player_Move; } }
        public InputAction @Look { get { return m_Wrapper.m_Player_Look; } }
        public InputAction @Jump { get { return m_Wrapper.m_Player_Jump; } }
        public InputAction @Dodge { get { return m_Wrapper.m_Player_Dodge; } }
        public InputAction @Attack { get { return m_Wrapper.m_Player_Attack; } }
        public InputAction @Shoot { get { return m_Wrapper.m_Player_Shoot; } }
        public InputAction @Use { get { return m_Wrapper.m_Player_Use; } }
        public InputAction @CameraChange { get { return m_Wrapper.m_Player_CameraChange; } }
        public InputAction @Ability { get { return m_Wrapper.m_Player_Ability; } }
        public InputAction @Pause { get { return m_Wrapper.m_Player_Pause; } }
        public InputAction @LockOn { get { return m_Wrapper.m_Player_LockOn; } }
        public InputAction @HeavyAttack { get { return m_Wrapper.m_Player_HeavyAttack; } }
        public InputActionMap Get() { return m_Wrapper.m_Player; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(PlayerActions set) { return set.Get(); }
    }
    public PlayerActions @Player
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new PlayerActions(this);
        }
    }
    private int m_MouseAndKeyboardSchemeIndex = -1;
    public InputControlScheme MouseAndKeyboardScheme
    {
        get

        {
            if (m_MouseAndKeyboardSchemeIndex == -1) m_MouseAndKeyboardSchemeIndex = asset.GetControlSchemeIndex("MouseAndKeyboard");
            return asset.controlSchemes[m_MouseAndKeyboardSchemeIndex];
        }
    }
    private int m_GamepadSchemeIndex = -1;
    public InputControlScheme GamepadScheme
    {
        get

        {
            if (m_GamepadSchemeIndex == -1) m_GamepadSchemeIndex = asset.GetControlSchemeIndex("Gamepad");
            return asset.controlSchemes[m_GamepadSchemeIndex];
        }
    }
}
