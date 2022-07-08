using UnityEngine;
using ColossalFramework.UI;
using System.Collections.Generic;

namespace TransferManagerCE
{
    public class UITabStrip : UIPanel
    {
        struct TabData
        {
            public UIButton? button = null;
            public UIPanel? panel = null;
            public bool visible = true;

            public TabData()
            {

            }
        }

        public UIPanel? m_tabButtonPanel = null;
        private List<TabData> m_tabs = new List<TabData>();
        private int m_iSelectedIndex = -1;
        private OnTabChanged? m_tabChangedEvent = null;

        public delegate void OnTabChanged(int index);

        public static UITabStrip Create(UIComponent parent, float width, float height, OnTabChanged eventTabChanged)
        {
            UITabStrip tabStrip = parent.AddUIComponent<UITabStrip>();
            if (tabStrip != null)
            {
                tabStrip.name = "TransferManagerCE.tabStrip";
                tabStrip.width = width;
                tabStrip.height = height;
                tabStrip.autoLayout = true;
                tabStrip.autoLayoutDirection = LayoutDirection.Vertical;
                tabStrip.eventVisibilityChanged += OnTabVisibilityChanged;

                tabStrip.m_tabButtonPanel = tabStrip.AddUIComponent<UIPanel>();
                tabStrip.m_tabButtonPanel.name = "TransferManagerCE.m_tabButtonPanel";
                tabStrip.m_tabButtonPanel.width = width;
                tabStrip.m_tabButtonPanel.height = 25;
                tabStrip.m_tabButtonPanel.autoLayout = true;
                tabStrip.m_tabButtonPanel.autoLayoutDirection = LayoutDirection.Horizontal;
                tabStrip.m_tabChangedEvent = eventTabChanged;
            }

            return tabStrip;
        }

        public UIPanel? AddTab(string sText)
        {
            if (m_tabs != null)
            {
                TabData tab = new TabData();
                tab.button = CreateTabButton(m_tabButtonPanel);
                tab.button.text = sText;
                tab.button.eventMouseDown += OnTabSelected;

                tab.panel = AddUIComponent<UIPanel>();
                tab.panel.name = "TransferManagerCE.TabPanel";
                tab.panel.width = width;
                tab.panel.height = height - m_tabButtonPanel.height;
                tab.panel.autoLayout = true;
                tab.panel.autoLayoutDirection = LayoutDirection.Vertical;
                m_tabs.Add(tab);

                return tab.panel;
            }
            return null;
        }

        public static void OnTabVisibilityChanged(UIComponent component, bool bVisible)
        {
            UITabStrip tabStrip = component as UITabStrip;
            if (tabStrip != null)
            {
                tabStrip.OnTabVisibilityChanged(bVisible);
            }
        }

        public void OnTabVisibilityChanged(bool bVisible)
        {
            if (bVisible)
            {
                if (m_iSelectedIndex < 0 && m_iSelectedIndex >= m_tabs.Count)
                {
                    SelectTabIndex(GetFirstVisibleTab());
                }
                if (m_iSelectedIndex >= 0 && m_iSelectedIndex < m_tabs.Count)
                {
                    ShowTab(m_iSelectedIndex);
                }
            }
        }

        private int GetFirstVisibleTab()
        {
            for (int i = 0; i < m_tabs.Count; ++i)
            {
                if (m_tabs[i].visible)
                {
                    return i;
                }
            }
            return -1;
        }

        public void OnTabSelected(UIComponent component, UIMouseEventParameter args)
        {
            for (int i = 0; i < m_tabs.Count; ++i)
            {
                if (component == m_tabs[i].button)
                {
                    SelectTabIndex(i);
                    break;
                }
            }
        }

        public int Count
        {
            get
            {
                if (m_tabs != null)
                {
                    return m_tabs.Count;
                }
                else
                {
                    return 0;
                }
            }
        }

        public static void OnSelectedTabIndexChanged(UIComponent component, int value)
        {
            UITabStrip? parent = (UITabStrip?)component.parent;
            if (parent != null)
            {
                parent.OnSelectTabChanged(value);
            }
        }

        public void OnSelectTabChanged(int value)
        {
            // Show coorect tab
            for (int i = 0; i < m_tabs.Count; i++)
            {
                if (i == value)
                {
                    ShowTab(i);
                    break;
                }
            }
        }

        public void SelectTabIndex(int iIndex)
        {
            if (m_tabs != null)
            {
                m_iSelectedIndex = iIndex;
                ShowTab(iIndex);
                if (m_tabChangedEvent != null)
                {
                    m_tabChangedEvent(iIndex);
                }
            }
        }

        private void ShowTab(int iIndex)
        {
            if (m_tabs != null)
            {
                if (iIndex >= 0 && iIndex < m_tabs.Count)
                {
                    for (int i = 0; i < m_tabs.Count; ++i)
                    {
                        if (i == iIndex)
                        {
                            m_tabs[i].panel.Show();
                            m_tabs[i].button.normalBgSprite = "GenericTabFocused"; //"GenericTabHovered";
                            m_tabs[i].button.hoveredBgSprite = "GenericTabFocused"; //"GenericTabHovered";
                        }
                        else
                        {
                            m_tabs[i].panel.Hide();
                            m_tabs[i].button.normalBgSprite = "GenericTab";
                            m_tabs[i].button.hoveredBgSprite = "GenericTabHovered";
                        }

                    }
                    if (iIndex == m_iSelectedIndex)
                    {
                        m_tabs[iIndex].panel.Focus();
                    }
                    else
                    {
                        m_tabs[iIndex].panel.Unfocus();
                    }
                }
            }
        }

        public int GetSelectTabIndex()
        {
            return m_iSelectedIndex;
        }

        public void SetTabVisible(int iIndex, bool bVisible)
        {
            if (m_tabs != null)
            {
                TabData tab = m_tabs[iIndex];
                if (tab.visible != bVisible)
                {
                    tab.visible = bVisible;
                    m_tabs[iIndex] = tab;

                    if (bVisible)
                    {
                        //m_tabs[iIndex].panel.Show();
                        m_tabs[iIndex].button.Show();
                    }
                    else
                    {
                        m_tabs[iIndex].panel.Hide();
                        m_tabs[iIndex].button.Hide();
                    }
                    if (m_iSelectedIndex == iIndex)
                    {
                        SelectTabIndex(GetFirstVisibleTab());
                    }
                }
            }
        }

        public void SetTabText(int iIndex, string sTab)
        {
            if (m_tabs != null)
            {
                m_tabs[iIndex].button.text = sTab;
            }
        }

        public bool IsTabVisible(int iIndex)
        {
            if (m_tabs != null)
            {
                return m_tabs[iIndex].visible;
            }
            return false;
        }

        private static UIButton CreateTabButton(UIComponent parent)
        {
            UIButton button = parent.AddUIComponent<UIButton>();
            button.name = "TabButton";

            button.height = 26f;
            button.width = 120f;

            button.textHorizontalAlignment = UIHorizontalAlignment.Center;
            button.textVerticalAlignment = UIVerticalAlignment.Middle;

            button.normalBgSprite = "GenericTab";
            button.disabledBgSprite = "GenericTabDisabled";
            button.focusedBgSprite = "GenericTabFocused";
            button.hoveredBgSprite = "GenericTabHovered";
            button.pressedBgSprite = "GenericTabFocused"; //"GenericTabPressed";

            button.textColor = new Color32(255, 255, 255, 255);
            button.disabledTextColor = new Color32(111, 111, 111, 255);
            button.focusedTextColor = new Color32(16, 16, 16, 255);
            button.hoveredTextColor = new Color32(255, 255, 255, 255);
            button.pressedTextColor = new Color32(255, 255, 255, 255);
            //button.isVisible = false;

            return button;
        }
    }
}