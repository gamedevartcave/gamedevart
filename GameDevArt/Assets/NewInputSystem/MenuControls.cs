// GENERATED AUTOMATICALLY FROM 'Assets/NewInputSystem/MenuControls.inputactions'

using System;
using UnityEngine;
using UnityEngine.Experimental.Input;


[Serializable]
public class MenuControls : InputActionAssetReference
{
    public MenuControls()
    {
    }
    public MenuControls(InputActionAsset asset)
        : base(asset)
    {
    }
    private bool m_Initialized;
    private void Initialize()
    {
        // Menu
        m_Menu = asset.GetActionMap("Menu");
        m_Menu_Nav = m_Menu.GetAction("Nav");
        m_Menu_Confirm = m_Menu.GetAction("Confirm");
        m_Menu_Back = m_Menu.GetAction("Back");
        m_Initialized = true;
    }
    private void Uninitialize()
    {
        m_Menu = null;
        m_Menu_Nav = null;
        m_Menu_Confirm = null;
        m_Menu_Back = null;
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
    // Menu
    private InputActionMap m_Menu;
    private InputAction m_Menu_Nav;
    private InputAction m_Menu_Confirm;
    private InputAction m_Menu_Back;
    public struct MenuActions
    {
        private MenuControls m_Wrapper;
        public MenuActions(MenuControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Nav { get { return m_Wrapper.m_Menu_Nav; } }
        public InputAction @Confirm { get { return m_Wrapper.m_Menu_Confirm; } }
        public InputAction @Back { get { return m_Wrapper.m_Menu_Back; } }
        public InputActionMap Get() { return m_Wrapper.m_Menu; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled { get { return Get().enabled; } }
        public InputActionMap Clone() { return Get().Clone(); }
        public static implicit operator InputActionMap(MenuActions set) { return set.Get(); }
    }
    public MenuActions @Menu
    {
        get
        {
            if (!m_Initialized) Initialize();
            return new MenuActions(this);
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
