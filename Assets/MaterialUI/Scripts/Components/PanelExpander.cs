using MaterialUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaterialUI
{
    public class PanelExpander : MonoBehaviour
    {
        public enum ExpandMode
        {
            Radial,
            Rectangular,
            Hybrid,
            AutoRadial
        }

        [SerializeField] private ExpandMode m_ExpandMode = ExpandMode.Hybrid;

        public ExpandMode expandMode
        {
            get { return m_ExpandMode; }
            set
            {
                m_ExpandMode = value;
                CalculateExpandMode();
            }
        }

        [SerializeField] private RectTransform m_PanelRootRectTransform;

        public RectTransform panelRootRectTransform
        {
            get { return m_PanelRootRectTransform; }
            set { m_PanelRootRectTransform = value; }
        }

        [SerializeField] private CanvasGroup m_PanelContentCanvasGroup;

        public CanvasGroup panelContentCanvasGroup
        {
            get { return m_PanelContentCanvasGroup; }
            set { m_PanelContentCanvasGroup = value; }
        }

        [SerializeField] private CanvasGroup m_PanelShadowCanvasGroup;

        public CanvasGroup panelShadowCanvasGroup
        {
            get { return m_PanelShadowCanvasGroup; }
            set { m_PanelShadowCanvasGroup = value; }
        }

        [SerializeField] private RectTransform m_BaseRectTransform;

        public RectTransform baseRectTransform
        {
            get { return m_BaseRectTransform; }
            set { m_BaseRectTransform = value; }
        }

        [SerializeField] private bool m_AutoExpandedSize = true;

        public bool autoExpandedSize
        {
            get { return m_AutoExpandedSize; }
            set { m_AutoExpandedSize = value; }
        }

        [SerializeField] private Vector2 m_ExpandedSize;

        public Vector2 expandedSize
        {
            get { return m_ExpandedSize; }
            set { m_ExpandedSize = value; }
        }

        [SerializeField] private bool m_AutoExpandedPosition = true;

        public bool autoExpandedPosition
        {
            get { return m_AutoExpandedPosition; }
            set { m_AutoExpandedPosition = value; }
        }

        [SerializeField] private Vector2 m_ExpandedPosition;

        public Vector2 expandedPosition
        {
            get { return m_ExpandedPosition; }
            set { m_ExpandedPosition = value; }
        }

        [SerializeField] private float m_AnimationDuration = 0.5f;

        public float animationDuration
        {
            get { return m_AnimationDuration; }
            set { m_AnimationDuration = value; }
        }

        private static RectTransform m_RippleMask;
        private static RectTransform rippleMask
        {
            get
            {
                if (m_RippleMask == null)
                {
                    m_RippleMask = new GameObject("PanelExpanderRippleMask").GetAddComponent<RectTransform>();
                    m_RippleMask.gameObject.AddComponent<RectMask2D>();
                }
                return m_RippleMask;
            }
        }

        [SerializeField] private bool m_RippleHasShadow;
        public bool rippleHasShadow
        {
            get { return m_RippleHasShadow; }
            set { m_RippleHasShadow = value; }
        }

        [SerializeField]
        private bool m_DarkenBackground = true;
        public bool darkenBackground
        {
            get { return m_DarkenBackground; }
            set { m_DarkenBackground = value; }
        }

        [SerializeField]
        private bool m_ClickBackgroundToClose = true;
        public bool clickBackgroundToClose
        {
            get { return m_ClickBackgroundToClose; }
            set { m_ClickBackgroundToClose = value; }
        }

        private ExpandMode m_FinalExpandMode;
        private RectTransform m_MaskRect;
        private Ripple m_Ripple;
        private float m_CircleExpandedSize;
        private float m_WidthDuration;
        private float m_HeightDuration;
        private bool m_HasExpandedInfo;
        private bool m_Shadow;
        private RectTransform m_ClickableBackground;
        private Image m_BackgroundImage;
        private EventTrigger m_BackgroundTrigger;

        public void Show()
        {
            m_PanelRootRectTransform.gameObject.SetActive(true);

            if (!m_HasExpandedInfo)
            {
                if (m_AutoExpandedPosition)
                {
                    m_ExpandedPosition = m_PanelRootRectTransform.GetPositionRegardlessOfPivot();
                }

                if (m_AutoExpandedSize)
                {
                    m_ExpandedSize = m_PanelRootRectTransform.sizeDelta;
                }

                CalculateExpandMode();

                m_HasExpandedInfo = true;
            }

            if (m_DarkenBackground || m_ClickBackgroundToClose)
            {
                if (m_ClickableBackground == null)
                {
                    m_ClickableBackground = PrefabManager.InstantiateGameObject(PrefabManager.ResourcePrefabs.clickableBackground, m_PanelRootRectTransform.parent).GetAddComponent<RectTransform>();
                    m_ClickableBackground.SetSiblingIndex(m_PanelRootRectTransform.GetSiblingIndex());
                    m_BackgroundImage = m_ClickableBackground.gameObject.GetComponent<Image>();
                    m_BackgroundTrigger = m_ClickableBackground.gameObject.GetComponent<EventTrigger>();
                    EventTrigger.Entry entry = new EventTrigger.Entry();
                    entry.callback.AddListener(baseEventData => OnBackgroundClick());
                    m_BackgroundTrigger.triggers.Add(entry);
                }

                m_ClickableBackground.sizeDelta = new Vector2(10000, 10000);
                m_ClickableBackground.anchoredPosition = m_ExpandedPosition;
                m_BackgroundImage.color = Color.clear;
                m_ClickableBackground.gameObject.SetActive(true);
            }

            m_PanelContentCanvasGroup.alpha = 0f;

            if (m_RippleHasShadow && m_PanelShadowCanvasGroup != null)
            {
                m_PanelShadowCanvasGroup.alpha = 0f;
            }

            if (m_FinalExpandMode != ExpandMode.Rectangular)
            {
                m_MaskRect = rippleMask;

                m_MaskRect.SetParentAndScale(m_PanelRootRectTransform.parent, Vector3.one);

                RippleData rippleData = new RippleData
                {
                    RippleParent = m_MaskRect,
                    Shadow = m_RippleHasShadow
                };

                m_Ripple = RippleManager.instance.GetRipple();
                m_Ripple.Setup(rippleData, m_MaskRect.position, null);
                m_Ripple.rectTransform.localScale = Vector3.one;
                m_Ripple.rectTransform.sizeDelta = m_BaseRectTransform.sizeDelta;
                m_Ripple.color = Color.white;
                m_Ripple.canvasGroup.alpha = 1f;
                m_Ripple.SubmitSizeForSoftening();

                m_CircleExpandedSize = Mathf.Sqrt(Mathf.Pow(Tween.QuintOut(0f, m_ExpandedSize.x, 1f, 0.5f), 2) +
                                                  Mathf.Pow(Tween.QuintOut(0f, m_ExpandedSize.y, 1f, 0.5f), 2));
            }

            if (m_FinalExpandMode != ExpandMode.Radial)
            {
                m_WidthDuration = 1f;
                m_HeightDuration = 1f;

                if (m_ExpandedSize.x > m_ExpandedSize.y)
                {
                    m_HeightDuration = (2 * (m_ExpandedSize.y / m_ExpandedSize.x) + 1f) / 3f;
                }
                else if (m_ExpandedSize.x < m_ExpandedSize.y)
                {
                    m_WidthDuration = (2 * (m_ExpandedSize.x / m_ExpandedSize.y) + 1f) / 3f;
                }
            }

            m_BaseRectTransform.gameObject.SetActive(false);

            m_Shadow = m_PanelShadowCanvasGroup != null;

            if (m_FinalExpandMode == ExpandMode.Radial)
            {
                m_PanelRootRectTransform.SetPositionRegardlessOfPivot(expandedPosition);
                m_PanelRootRectTransform.sizeDelta = expandedSize;

                TweenManager.TweenFloat(f =>
                {
                    m_MaskRect.sizeDelta = m_PanelRootRectTransform.sizeDelta;
                    m_MaskRect.position = m_PanelRootRectTransform.GetPositionRegardlessOfPivot();

                    m_Ripple.rectTransform.sizeDelta = Tween.CubeSoftOut(m_Ripple.rectTransform.sizeDelta, new Vector2(m_CircleExpandedSize, m_CircleExpandedSize), f, 1f);
                    m_Ripple.rectTransform.position = Tween.CubeSoftOut(m_BaseRectTransform.GetPositionRegardlessOfPivot(), m_PanelRootRectTransform.GetPositionRegardlessOfPivot(), f, 1f);
                    m_Ripple.SubmitSizeForSoftening();

                    if (m_Shadow)
                    {
                        m_PanelShadowCanvasGroup.alpha = Tween.CubeInOut(0f, 1f, f - 0.5f, 0.5f);
                    }

                    m_PanelContentCanvasGroup.alpha = Tween.CubeInOut(0f, 1f, f - 0.5f, 0.25f);
                    m_Ripple.color = m_Ripple.color.WithAlpha(Tween.CubeInOut(1f, 0f, f - 0.5f, 0.5f));
                },
                0f, 1f, m_AnimationDuration, tweenType: Tween.TweenType.Linear);

                TweenManager.TimedCallback(m_AnimationDuration, m_Ripple.InstantContract);
            }
            else if (m_FinalExpandMode == ExpandMode.Rectangular)
            {
                TweenManager.TweenFloat(f =>
                {
                    if (m_ExpandedPosition.y > m_BaseRectTransform.position.y)
                    {
                        m_PanelRootRectTransform.sizeDelta = new Vector2(
                            Tween.QuintOut(m_BaseRectTransform.sizeDelta.x, expandedSize.x, f, m_WidthDuration),
                            Tween.QuintSoftOut(m_BaseRectTransform.sizeDelta.y, expandedSize.y, f, m_HeightDuration));

                        m_PanelRootRectTransform.SetPositionRegardlessOfPivot(new Vector2(
                            Tween.QuintOut(m_BaseRectTransform.GetPositionRegardlessOfPivot().x, expandedPosition.x, f, m_WidthDuration),
                            Tween.QuintSoftOut(m_BaseRectTransform.GetPositionRegardlessOfPivot().y, expandedPosition.y, f, m_HeightDuration)));
                    }
                    else
                    {
                        m_PanelRootRectTransform.sizeDelta = new Vector2(
                            Tween.QuintSoftOut(m_BaseRectTransform.sizeDelta.x, expandedSize.x, f, m_WidthDuration),
                            Tween.QuintOut(m_BaseRectTransform.sizeDelta.y, expandedSize.y, f, m_HeightDuration));

                        m_PanelRootRectTransform.SetPositionRegardlessOfPivot(new Vector2(
                            Tween.QuintSoftOut(m_BaseRectTransform.GetPositionRegardlessOfPivot().x, expandedPosition.x, f, m_WidthDuration),
                            Tween.QuintOut(m_BaseRectTransform.GetPositionRegardlessOfPivot().y, expandedPosition.y, f, m_HeightDuration)));
                    }

                    if (m_Shadow)
                    {
                        m_PanelShadowCanvasGroup.alpha = Tween.QuintOut(0f, 1f, f, 0.25f);
                    }
                    m_PanelContentCanvasGroup.alpha = Tween.QuintInOut(0f, 1f, f - 0.5f, 0.5f);
                },
                0f, 1f, m_AnimationDuration, tweenType: Tween.TweenType.Linear);
            }
            else if (m_FinalExpandMode == ExpandMode.Hybrid)
            {
                Vector2 startPos = m_BaseRectTransform.GetPositionRegardlessOfPivot();

                m_PanelRootRectTransform.sizeDelta = m_BaseRectTransform.sizeDelta;
                m_PanelRootRectTransform.SetPositionRegardlessOfPivot(m_BaseRectTransform.GetPositionRegardlessOfPivot());

                TweenManager.TweenFloat(f =>
                {
                    if (m_ExpandedPosition.y > m_BaseRectTransform.position.y)
                    {
                        m_PanelRootRectTransform.sizeDelta = new Vector2(
                            Tween.QuintOut(m_BaseRectTransform.sizeDelta.x, expandedSize.x, f, m_WidthDuration),
                            Tween.QuintSoftOut(m_BaseRectTransform.sizeDelta.y, expandedSize.y, f, m_HeightDuration));

                        m_PanelRootRectTransform.SetPositionRegardlessOfPivot(new Vector2(
                            Tween.QuintOut(startPos.x, expandedPosition.x, f, m_WidthDuration),
                            Tween.QuintSoftOut(startPos.y, expandedPosition.y, f, m_HeightDuration)));
                    }
                    else
                    {
                        m_PanelRootRectTransform.sizeDelta = new Vector2(
                            Tween.QuintSoftOut(m_BaseRectTransform.sizeDelta.x, expandedSize.x, f, m_WidthDuration),
                            Tween.QuintOut(m_BaseRectTransform.sizeDelta.y, expandedSize.y, f, m_HeightDuration));

                        m_PanelRootRectTransform.SetPositionRegardlessOfPivot(new Vector2(
                            Tween.QuintSoftOut(startPos.x, expandedPosition.x, f, m_WidthDuration),
                            Tween.QuintOut(startPos.y, expandedPosition.y, f, m_HeightDuration)));
                    }

                    m_MaskRect.sizeDelta = m_PanelRootRectTransform.sizeDelta;
                    m_MaskRect.position = m_PanelRootRectTransform.GetPositionRegardlessOfPivot();

                    m_Ripple.rectTransform.sizeDelta = Tween.CubeSoftOut(m_Ripple.rectTransform.sizeDelta, new Vector2(m_CircleExpandedSize, m_CircleExpandedSize), f - 0.2f, 1f);
                    m_Ripple.rectTransform.anchoredPosition = Vector2.zero;
                    m_Ripple.SubmitSizeForSoftening();

                    if (m_Shadow)
                    {
                        m_PanelShadowCanvasGroup.alpha = Tween.CubeInOut(0f, 1f, f - 0.4f, 0.6f);
                    }

                    m_PanelContentCanvasGroup.alpha = Tween.CubeInOut(0f, 1f, f - 0.4f, 0.3f);
                    m_Ripple.color = m_Ripple.color.WithAlpha(Tween.CubeInOut(1f, 0f, f - 0.5f, 0.5f));
                },
                0f, 1f, m_AnimationDuration, tweenType: Tween.TweenType.Linear);
                
                if (m_DarkenBackground)
                {
                    TweenManager.TweenColor(color => m_BackgroundImage.color = color, m_BackgroundImage.color, MaterialColor.disabledDark, m_AnimationDuration * 0.5f, m_AnimationDuration * 0.5f, tweenType: Tween.TweenType.EaseInOutCubed);
                }

                TweenManager.TimedCallback(m_AnimationDuration * 0.7f, m_Ripple.InstantContract);
            }
        }

        public void Hide()
        {
            m_Shadow = m_PanelShadowCanvasGroup != null;

            if (m_FinalExpandMode != ExpandMode.Rectangular)
            {
                m_MaskRect = rippleMask;

                m_MaskRect.SetParentAndScale(m_PanelRootRectTransform.parent, Vector3.one);

                RippleData rippleData = new RippleData
                {
                    RippleParent = m_MaskRect,
                    Shadow = m_RippleHasShadow
                };

                m_Ripple = RippleManager.instance.GetRipple();
                m_Ripple.Setup(rippleData, m_MaskRect.position, null);
                m_Ripple.rectTransform.localScale = Vector3.one;
                m_Ripple.rectTransform.sizeDelta = m_BaseRectTransform.sizeDelta;
                m_Ripple.color = Color.white;
                m_Ripple.canvasGroup.alpha = 1f;
                m_Ripple.SubmitSizeForSoftening();

                m_CircleExpandedSize = Mathf.Sqrt(Mathf.Pow(Tween.QuintOut(0f, m_ExpandedSize.x, 1f, 0.5f), 2) + Mathf.Pow(Tween.QuintOut(0f, m_ExpandedSize.y, 1f, 0.5f), 2));
            }

            if (m_FinalExpandMode != ExpandMode.Radial)
            {
                m_WidthDuration = 1f;
                m_HeightDuration = 1f;

                if (m_ExpandedSize.x > m_ExpandedSize.y)
                {
                    m_HeightDuration = (2 * (m_ExpandedSize.y / m_ExpandedSize.x) + 1f) / 3f;
                }
                else if (m_ExpandedSize.x < m_ExpandedSize.y)
                {
                    m_WidthDuration = (2 * (m_ExpandedSize.x / m_ExpandedSize.y) + 1f) / 3f;
                }
            }

            if (m_FinalExpandMode == ExpandMode.Radial)
            {
                m_PanelRootRectTransform.SetPositionRegardlessOfPivot(expandedPosition);
                m_PanelRootRectTransform.sizeDelta = expandedSize;

                TweenManager.TweenFloat(f =>
                {
                    m_MaskRect.sizeDelta = m_PanelRootRectTransform.sizeDelta;
                    m_MaskRect.position = m_PanelRootRectTransform.GetPositionRegardlessOfPivot();

                    m_Ripple.rectTransform.sizeDelta = Tween.QuintSoftOut(new Vector2(m_CircleExpandedSize, m_CircleExpandedSize), m_BaseRectTransform.sizeDelta, f, 1f);
                    m_Ripple.rectTransform.position = Tween.QuintSoftOut(m_PanelRootRectTransform.GetPositionRegardlessOfPivot(), m_BaseRectTransform.GetPositionRegardlessOfPivot(), f, 1f);
                    m_Ripple.SubmitSizeForSoftening();

                    if (m_Shadow)
                    {
                        m_PanelShadowCanvasGroup.alpha = Tween.QuintOut(1f, 0f, f, 0.5f);
                    }

                    m_PanelContentCanvasGroup.alpha = Tween.QuintOut(1f, 0f, f, 0.5f);
                    m_Ripple.color = m_Ripple.color.WithAlpha(Tween.QuintSoftOut(1f, 0f, f - 0.8f, 0.2f));
                },
                0f, 1f, m_AnimationDuration, tweenType: Tween.TweenType.Linear);

                TweenManager.TimedCallback(m_AnimationDuration, m_Ripple.InstantContract);
                TweenManager.TimedCallback(m_AnimationDuration * 0.5f, () => m_BaseRectTransform.gameObject.SetActive(true));
            }
            else if (m_FinalExpandMode == ExpandMode.Rectangular)
            {
                TweenManager.TweenFloat(f =>
                {
                    if (m_ExpandedPosition.y > m_BaseRectTransform.position.y)
                    {
                        m_PanelRootRectTransform.sizeDelta = new Vector2(
                            Tween.QuintInOut(expandedSize.x, m_BaseRectTransform.sizeDelta.x, f, m_WidthDuration),
                            Tween.QuintSoftOut(expandedSize.y, m_BaseRectTransform.sizeDelta.y, f, m_HeightDuration));

                        m_PanelRootRectTransform.SetPositionRegardlessOfPivot(new Vector2(
                            Tween.QuintInOut(expandedPosition.x, m_BaseRectTransform.GetPositionRegardlessOfPivot().x, f, m_WidthDuration),
                            Tween.QuintSoftOut(expandedPosition.y, m_BaseRectTransform.GetPositionRegardlessOfPivot().y, f, m_HeightDuration)));
                    }
                    else
                    {
                        m_PanelRootRectTransform.sizeDelta = new Vector2(
                            Tween.QuintSoftOut(expandedSize.x, m_BaseRectTransform.sizeDelta.x, f, m_WidthDuration),
                            Tween.QuintInOut(expandedSize.y, m_BaseRectTransform.sizeDelta.y, f, m_HeightDuration));

                        m_PanelRootRectTransform.SetPositionRegardlessOfPivot(new Vector2(
                            Tween.QuintSoftOut(expandedPosition.x, m_BaseRectTransform.GetPositionRegardlessOfPivot().x, f, m_WidthDuration),
                            Tween.QuintInOut(expandedPosition.y, m_BaseRectTransform.GetPositionRegardlessOfPivot().y, f, m_HeightDuration)));
                    }

                    if (m_Shadow)
                    {
                        m_PanelShadowCanvasGroup.alpha = Tween.QuintOut(1f, 0f, f - 0.75f, 0.25f);
                    }

                    m_PanelContentCanvasGroup.alpha = Tween.QuintSoftOut(1f, 0f, f, 0.25f);
                },
                0f, 1f, m_AnimationDuration, tweenType: Tween.TweenType.Linear);

                TweenManager.TimedCallback(m_AnimationDuration * 0.7f, () => m_BaseRectTransform.gameObject.SetActive(true));
            }
            else if (m_FinalExpandMode == ExpandMode.Hybrid)
            {
                Vector2 endPos = m_BaseRectTransform.GetPositionRegardlessOfPivot();

                TweenManager.TweenFloat(f =>
                {
                    if (m_ExpandedPosition.y > m_BaseRectTransform.position.y)
                    {
                        m_PanelRootRectTransform.sizeDelta = new Vector2(
                            Tween.QuintInOut(expandedSize.x, m_BaseRectTransform.sizeDelta.x, f, m_WidthDuration),
                            Tween.QuintSoftOut(expandedSize.y, m_BaseRectTransform.sizeDelta.y, f, m_HeightDuration));

                        m_PanelRootRectTransform.SetPositionRegardlessOfPivot(new Vector2(
                            Tween.QuintInOut(expandedPosition.x, endPos.x, f, m_WidthDuration),
                            Tween.QuintSoftOut(expandedPosition.y, endPos.y, f, m_HeightDuration)));
                    }
                    else
                    {
                        m_PanelRootRectTransform.sizeDelta = new Vector2(
                            Tween.QuintSoftOut(expandedSize.x, m_BaseRectTransform.sizeDelta.x, f, m_WidthDuration),
                            Tween.QuintInOut(expandedSize.y, m_BaseRectTransform.sizeDelta.y, f, m_HeightDuration));

                        m_PanelRootRectTransform.SetPositionRegardlessOfPivot(new Vector2(
                            Tween.QuintSoftOut(expandedPosition.x, endPos.x, f, m_WidthDuration),
                            Tween.QuintInOut(expandedPosition.y, endPos.y, f, m_HeightDuration)));
                    }

                    m_MaskRect.sizeDelta = m_PanelRootRectTransform.sizeDelta;
                    m_MaskRect.position = m_PanelRootRectTransform.GetPositionRegardlessOfPivot();

                    m_Ripple.rectTransform.sizeDelta = Tween.QuintSoftOut(new Vector2(m_CircleExpandedSize, m_CircleExpandedSize), m_BaseRectTransform.sizeDelta, f, Mathf.Min(m_WidthDuration, m_HeightDuration));
                    m_Ripple.rectTransform.anchoredPosition = Vector2.zero;
                    m_Ripple.SubmitSizeForSoftening();

                    if (m_Shadow)
                    {
                        m_PanelShadowCanvasGroup.alpha = Tween.QuintOut(1f, 0f, f, 0.15f);
                    }

                    m_PanelContentCanvasGroup.alpha = Tween.QuintOut(1f, 0f, f, 0.15f);
                    m_Ripple.color = m_Ripple.color.WithAlpha(Tween.QuintSoftOut(1f, 0f, f - 0.8f, 0.2f));
                },
                0f, 1f, m_AnimationDuration, tweenType: Tween.TweenType.Linear);

                TweenManager.TimedCallback(m_AnimationDuration, m_Ripple.InstantContract);
                TweenManager.TimedCallback(m_AnimationDuration * 0.8f, () => m_BaseRectTransform.gameObject.SetActive(true));
            }

            if (m_DarkenBackground)
            {
                TweenManager.TweenColor(color => m_BackgroundImage.color = color, m_BackgroundImage.color, Color.clear, m_AnimationDuration * 0.5f, m_AnimationDuration * 0.5f, () => m_ClickableBackground.gameObject.SetActive(false), tweenType: Tween.TweenType.EaseInOutCubed);
            }

            TweenManager.TimedCallback(m_AnimationDuration, () => m_PanelRootRectTransform.gameObject.SetActive(false));
        }

        public void OnBackgroundClick()
        {
            if (m_ClickBackgroundToClose)
            {
                Hide();
            }
        }

        public void CalculateExpandMode()
        {
            if (m_ExpandMode == ExpandMode.AutoRadial)
            {
                Vector2 baseMin = new Vector2(
                    m_BaseRectTransform.GetPositionRegardlessOfPivot().x - m_BaseRectTransform.sizeDelta.x / 2f,
                    m_BaseRectTransform.GetPositionRegardlessOfPivot().y - m_BaseRectTransform.sizeDelta.y / 2f);
                Vector2 baseMax = new Vector2(
                    m_BaseRectTransform.GetPositionRegardlessOfPivot().x + m_BaseRectTransform.sizeDelta.x / 2f,
                    m_BaseRectTransform.GetPositionRegardlessOfPivot().y + m_BaseRectTransform.sizeDelta.y / 2f);

                Vector2 expandedMin = new Vector2(m_ExpandedPosition.x - m_ExpandedSize.x / 2f, m_ExpandedPosition.y - m_ExpandedSize.y / 2f);
                Vector2 expandedMax = new Vector2(m_ExpandedPosition.x + m_ExpandedSize.x / 2f, m_ExpandedPosition.y + m_ExpandedSize.y / 2f);

                if ((expandedMin.x <= baseMin.x) && (expandedMin.y <= baseMin.y) && (expandedMax.x >= baseMax.x) &&
                    (expandedMax.y >= baseMax.y))
                {
                    m_FinalExpandMode = ExpandMode.Radial;
                }
                else
                {
                    m_FinalExpandMode = ExpandMode.Hybrid;
                }
            }
            else
            {
                m_FinalExpandMode = m_ExpandMode;
            }
        }
    }

}