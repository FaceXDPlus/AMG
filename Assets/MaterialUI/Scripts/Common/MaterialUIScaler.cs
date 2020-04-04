//  Copyright 2017 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MaterialUI
{
    /// <summary>
    /// A component that handles scaling of a Canvas and callbacks to child MaterialUI components upon scale or orientation change.
    /// <para></para>
    /// Attach to a Canvas GameObject.
    /// <para></para>
    /// Can be used to scale the Canvas, or in tandem with a CanvasScaler.
    /// </summary>
    [ExecuteInEditMode]
    [RequireComponent(typeof(Canvas))]
    [Serializable]
    public class MaterialUIScaler : UIBehaviour
    {
        /// <summary>
        /// The orientations a Canvas can have.
        /// </summary>
        public enum Orientation
        {
            Vertical,
            Horizontal
        }

        /// <summary>
        /// The modes that a MaterialUIScaler can use.
        /// </summary>
        public enum ScalerMode
        {
            /// <summary>
            /// Scales the Canvas so that elements are roughly the same size on varying displays.
            /// <para></para>
            /// Works like CanvasScaler's 'constant physical size' mode, but better.
            /// </summary>
            ConstantPhysicalSize,
            /// <summary>
            /// References another MaterialUIScaler and uses its settings to scale the Canvas identically.
            /// </summary>
            CopyOtherScaler,
            /// <summary>
            /// Doesn't scale the Canvas, but still provides callbacks upon scale and orientation changes.
            /// <para></para>
            /// A CanvasScaler can also be attached to scale the canvas.
            /// </summary>
            DontChangeScale
        }

        /// <summary>
        /// The Canvas that this component controls.
        /// </summary>
        [SerializeField]
        private Canvas m_Canvas;
        /// <summary>
        /// The Canvas that this component controls.
        /// </summary>
        public Canvas canvas
        {
            get
            {
                if (m_Canvas == null)
                {
                    m_Canvas = GetComponent<Canvas>();
                }
                return m_Canvas;
            }
        }

        /// <summary>
        /// The currently selected scaling mode.
        /// </summary>
        [SerializeField]
        private ScalerMode m_ScalerMode = ScalerMode.DontChangeScale;
        /// <summary>
        /// The currently selected scaling mode.
        /// </summary>
        public ScalerMode scalerMode
        {
            get { return m_ScalerMode; }
            set { m_ScalerMode = value; }
        }

        /// <summary>
        /// The resulting scale factor, after all calculations are done.
        /// In screenspace render modes, this value should match the canvas' scaleFactor value.
        /// </summary>
        [SerializeField]
        private float m_ScaleFactor;
        /// <summary>
        /// The resulting scale factor, after all calculations are done.
        /// In screenspace render modes, this value should match the canvas' scaleFactor value.
        /// </summary>
        public float scaleFactor
        {
            get { return m_ScaleFactor; }
        }

#if UNITY_EDITOR

        /// <summary>
        /// Should a fake DPI be used in the editor?
        /// <para></para>
        /// This helps when designing the UI.
        /// </summary>
        [SerializeField]
        private bool m_EditorForceDpi = true;

        /// <summary>
        /// What DPI should be forced in the editor, if enabled?
        /// </summary>
        [SerializeField]
        private float m_EditorForceDpiValue = 160f;

#endif

        /// <summary>
        /// The reference pixels per Unity unit in world space.
        /// </summary>
        [SerializeField]
        private float m_ReferencePixelsPerUnit = 100f;
        /// <summary>
        /// The reference pixels per Unity unit in world space.
        /// <para></para>
        /// Scaling is recalculated when this value is changed.
        /// </summary>
        public float referencePixelsPerUnit
        {
            get { return m_ReferencePixelsPerUnit; }
            set
            {
                m_ReferencePixelsPerUnit = value;
                Handle();
            }
        }

        /// <summary>
        /// The dynamic pixels per unit to use in world space mode.
        /// </summary>
        [SerializeField]
        private float m_DynamicPixelsPerUnit = 1f;
        /// <summary>
        /// The dynamic pixels per unit to use in world space mode.
        /// <para></para>
        /// Scaling is recalculated when this value is changed.
        /// </summary>
        public float dynamicPixelsPerUnit
        {
            get { return m_DynamicPixelsPerUnit; }
            set
            {
                m_DynamicPixelsPerUnit = value;
                Handle();
            }
        }

        /// <summary>
        /// The base DPI to use for scaling.
        /// </summary>
        [SerializeField]
        private float m_BaseDpi = 160f;
        /// <summary>
        /// The base DPI to use for scaling.
        /// <para></para>
        /// Scaling is recalculated when this value is changed.
        /// </summary>
        public float baseDpi
        {
            get { return m_BaseDpi; }
            set
            {
                m_BaseDpi = value;
                Handle();
            }
        }

        /// <summary>
        /// The DPI to use when Unity is unable to retreive the display's DPI.
        /// </summary>
        [SerializeField]
        private float m_FallbackDpi = 160f;
        /// <summary>
        /// The DPI to use when Unity is unable to retreive the display's DPI.
        /// <para></para>
        /// Scaling is recalculated when this value is changed.
        /// </summary>
        public float fallbackDpi
        {
            get { return m_FallbackDpi; }
            set
            {
                m_FallbackDpi = value;
                Handle();
            }
        }

        /// <summary>
        /// This value is used to modify the scale value after DPI calculation, but before snapping.
        /// <para></para>
        /// Useful as a global scaling value.
        /// </summary>
        [SerializeField]
        private float m_ScaleModifier = 1f;
        /// <summary>
        /// This value is used to modify the scale value after DPI calculation, but before snapping.
        /// <para></para>
        /// Useful as a global scaling value.
        /// <para></para>
        /// Scaling is recalculated when this value is changed.
        /// </summary>
        public float scaleModifier
        {
            get { return m_ScaleModifier; }
            set
            {
                m_ScaleModifier = value;
                Handle();
            }
        }

        /// <summary>
        /// The interval size to use when snapping the scaleFactor.
        /// <para></para>
        /// The interval size is 1 / m_SnapScaleLevel, so a value of 4 would snap scaling to 0.25x intervals.
        /// </summary>
        [SerializeField]
        private int m_ScaleSnapLevel = 8;
        /// <summary>
        /// The interval size to use when snapping the scaleFactor.
        /// <para></para>
        /// The interval size is 1 / m_SnapScaleLevel, so a value of 4 would snap scaling to 0.25x intervals.
        /// <para></para>
        /// Scaling is recalculated when this value is changed.
        /// </summary>
        public int scaleSnapLevel
        {
            get { return m_ScaleSnapLevel; }
            set
            {
                if (m_ScaleSnapLevel == value) return;

                m_ScaleSnapLevel = value;
                if (m_ScaleSnapLevel < 0)
                {
                    m_ScaleSnapLevel = 0;
                }
                if (m_ScaleSnapLevel > 16)
                {
                    m_ScaleSnapLevel = 16;
                }
                Handle();
            }
        }

        /// <summary>
        /// The scaler to use when set to 'copy other scaler' mode.
        /// </summary>
        [SerializeField]
        private MaterialUIScaler m_TargetScaler;
        /// <summary>
        /// The scaler to use when set to 'copy other scaler' mode.
        /// <para></para>
        /// Scaling is recalculated when this value is changed.
        /// </summary>
        public MaterialUIScaler targetScaler
        {
            get { return m_TargetScaler; }
            set
            {
                m_TargetScaler = value;
                Handle();
            }
        }

        /// <summary>
        /// The CanvasScaler to use when set to 'dont change scale' mode.
        /// <para></para>
        /// This helps with scale/orientation callbacks.
        /// <para></para>
        /// Scaling is recalculated when this value is changed.
        /// </summary>
        [SerializeField]
        private CanvasScaler m_TargetCanvasScaler;
        public CanvasScaler targetCanvasScaler
        {
            get
            {
                if (m_TargetCanvasScaler == null)
                {
                    m_TargetCanvasScaler = gameObject.GetComponent<CanvasScaler>();
                }
                return m_TargetCanvasScaler;
            }
        }

        /// <summary>
        /// The current orientation of the Canvas.
        /// </summary>
        private Orientation m_Orientation;
        /// <summary>
        /// The current orientation of the Canvas.
        /// </summary>
        public Orientation orientation
        {
            get { return m_Orientation; }
        }

        /// <summary>
        /// Event for when the scaleFactor or orientation are changed.
        /// </summary>
        [Serializable]
        public class CanvasAreaChangedEvent : UnityEvent<bool, bool> { }

        /// <summary>
        /// Called when the scaleFactor or orientation are changed.
        /// <para></para>
        /// bool1 = Was the scaleFactor changed?
        /// <para></para>
        /// bool2 = Was the orientation changed?
        /// </summary>
        [SerializeField]
        private CanvasAreaChangedEvent m_OnCanvasAreaChanged = new CanvasAreaChangedEvent();
        /// <summary>
        /// Called when the scaleFactor or orientation are changed.
        /// <para></para>
        /// bool1 = Was the scaleFactor changed?
        /// <para></para>
        /// bool2 = Was the orientation changed?
        /// </summary>
        public CanvasAreaChangedEvent onCanvasAreaChanged
        {
            get { return m_OnCanvasAreaChanged; }
            set { m_OnCanvasAreaChanged = value; }
        }

        /// <summary>
        /// The previous calculated scaleFactor. Helps with optimisation.
        /// </summary>
        private float m_PrevScaleFactor;
        /// <summary>
        /// The previous referencePixelsPerUnit. Helps with optimisation.
        /// </summary>
        private float m_PrevReferencePixelsPerUnit;
        /// <summary>
        /// The previous orientation. Helps with optimisation.
        /// </summary>
        private Orientation m_PrevOrientation;

        /// <summary>
        /// Handles the canvas scaling (if applicable) and checks if the callback should be invoked.
        /// </summary>
        public void Handle()
        {
            if (!enabled) return;

            switch (scalerMode)
            {
                case ScalerMode.ConstantPhysicalSize:
                    HandleConstantPhysicalSize();
                    break;
                case ScalerMode.CopyOtherScaler:
                    HandleCopyOtherScaler();
                    break;
                case ScalerMode.DontChangeScale:
                    HandleDontChangeScale();
                    break;
            }
        }

        private int m_ScreenWidth;
        public int screenWidth
        {
            get
            {
                if (m_ScreenWidth == 0)
                {
                    m_ScreenWidth = Display.displays[canvas.targetDisplay].renderingWidth;
                }
                return m_ScreenWidth;
            }
        }

        private int m_ScreenHeight;
        public int screenHeight
        {
            get
            {
                if (m_ScreenHeight == 0)
                {
                    m_ScreenHeight = Display.displays[canvas.targetDisplay].renderingHeight;
                }
                return m_ScreenHeight;
            }
        }

        private float m_Dpi;
        public float dpi
        {
            get
            {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
                m_Dpi = Screen.dpi;
                //m_Dpi = Mathf.Round(1.445f * float.Parse(Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE").OpenSubKey("Microsoft").OpenSubKey("Windows NT").OpenSubKey("CurrentVersion").OpenSubKey("FontDPI").GetValue("LogPixels").ToString()));
#else
                m_Dpi = Screen.dpi;
#endif

#if UNITY_EDITOR
                if (m_EditorForceDpi && m_EditorForceDpiValue > baseDpi / 50f)
                    {
                        m_Dpi = m_EditorForceDpiValue;
                    }
                #endif

                if (m_Dpi == 0f)
                {
                    m_Dpi = fallbackDpi;
                }

                return m_Dpi;
            }
        }

        private float m_ScreenSizeDiagonal;
        public float screenSizeDigonal
        {
            get
            {
                if (m_ScreenSizeDiagonal == 0f)
                {
                    m_ScreenSizeDiagonal = Mathf.Sqrt(Mathf.Pow(screenWidth, 2f) + Mathf.Pow(screenHeight, 2f)) / dpi;
                }
                return m_ScreenSizeDiagonal;
            }
        }

        /// <summary>
        /// Handles the canvas scaling and callbacks when in 'constant physical size' mode.
        /// </summary>
        private void HandleConstantPhysicalSize()
        {
            float scale;

            if (canvas.renderMode == RenderMode.WorldSpace)
            {
                scale = m_DynamicPixelsPerUnit;
            }
            else
            {
                scale = (dpi / m_BaseDpi) * m_ScaleModifier;

                if (m_ScaleSnapLevel > 0 && scale > 0f && dpi > m_BaseDpi / 50f)
                {
                    float interval = 1f / m_ScaleSnapLevel;
                    float levelAbove = 0f;

                    for (int i = 0; i < 1000; i++)
                    {
                        if(levelAbove >= scale)
                        {
                            break;
                        }
                        levelAbove += interval;
                    }

                    float levelBelow = levelAbove - interval;

                    scale = Mathf.Abs(levelBelow - scale) * 1.5f < Mathf.Abs(levelAbove - scale) * 0.5f
                        ? levelBelow
                        : levelAbove;
                }
            }

            ApplyScaleSettings(scale, m_ReferencePixelsPerUnit, true);
        }

        /// <summary>
        /// Handles the canvas scaling and callbacks when in 'copy other scaler' mode.
        /// </summary>
        private void HandleCopyOtherScaler()
        {
            if (m_TargetScaler == null)
            {
                return;
            }

            if ((canvas.renderMode == RenderMode.WorldSpace) !=
                (m_TargetScaler.canvas.renderMode == RenderMode.WorldSpace))
            {
                Debug.LogWarning(
                    "The Canvas belonging to the MaterialUIScaler reference has a different render mode to this one, skipping scaling",
                    this);
                return;
            }

            ApplyScaleSettings(targetScaler.canvas.scaleFactor, targetScaler.canvas.referencePixelsPerUnit, true);
        }

        /// <summary>
        /// Handles the callbacks when in 'dont change scale' mode.
        /// </summary>
        private void HandleDontChangeScale()
        {
            ApplyScaleSettings(canvas.scaleFactor, canvas.referencePixelsPerUnit, false);
        }

        /// <summary>
        /// Checks if the new values are different from the previous ones, and applies the settings to the canvas (if applicable). 
        /// Applies the calculated scale settings.
        /// </summary>
        /// <param name="currentScale">The current scale to use.</param>
        /// <param name="currentPixelsPerUnit">The current pixels per unit to use.</param>
        /// <param name="applyToCanvas">if set to <c>true</c> apply the scaling values to the canvas.</param>
        private void ApplyScaleSettings(float currentScale, float currentPixelsPerUnit, bool applyToCanvas)
        {
            m_Orientation = canvas.pixelRect.width > canvas.pixelRect.height
                ? Orientation.Horizontal
                : Orientation.Vertical;

            bool scaleChanged = false;
            bool orientationChanged = false;

            if ((double)m_PrevScaleFactor != (double)currentScale)
            {
                scaleChanged = true;
                m_PrevScaleFactor = currentScale;
                m_ScaleFactor = canvas.renderMode == RenderMode.WorldSpace ? 1f : currentScale;

                if (applyToCanvas)
                {
                    canvas.scaleFactor = currentScale;
                }
            }

            if ((double)m_PrevReferencePixelsPerUnit != (double)currentPixelsPerUnit)
            {
                scaleChanged = true;
                m_PrevReferencePixelsPerUnit = currentPixelsPerUnit;

                if (applyToCanvas)
                {
                    canvas.referencePixelsPerUnit = currentPixelsPerUnit;
                }
            }

            if (m_PrevOrientation != m_Orientation)
            {
                orientationChanged = true;
                m_PrevOrientation = m_Orientation;
            }

            if(scaleChanged || orientationChanged)
            {
                m_ScreenWidth = 0;
                m_ScreenHeight = 0;
                m_ScreenSizeDiagonal = 0f;

                float i = screenWidth;
                i = screenHeight;
                i = screenSizeDigonal;
            }

            if ((scaleChanged || orientationChanged) && m_OnCanvasAreaChanged != null)
            {
                m_OnCanvasAreaChanged.Invoke(scaleChanged, orientationChanged);
            }
        }

        /// <summary>
        /// See MonoBehaviour.OnEnable.
        /// </summary>
        protected override void OnEnable()
        {
            if (targetCanvasScaler != null && targetCanvasScaler.enabled && scalerMode != ScalerMode.DontChangeScale)
            {
                m_ScalerMode = ScalerMode.DontChangeScale;
            }

            if (m_ScalerMode == ScalerMode.CopyOtherScaler && m_TargetScaler != null)
            {
                m_TargetScaler.onCanvasAreaChanged.AddListener((b1, b2) => Handle());
            }
        }

        /// <summary>
        /// See MonoBehaviour.Awake.
        /// </summary>
        protected override void Awake()
        {
            Handle();
        }

        /// <summary>
        /// MonoBehaviour.Update.
        /// </summary>
        void Update()
        {
            Handle();
        }

        /// <summary>
        /// This callback is called if an associated RectTransform has its dimensions changed. The call is also made to all child rect transforms, even if the child transform itself doesn't change - as it could have, depending on its anchoring.
        /// </summary>
        protected override void OnRectTransformDimensionsChange()
        {
            Handle();
        }

        /// <summary>
        /// See MonoBehaviour.OnDisable.
        /// </summary>
        protected override void OnDisable()
        {
            if (m_TargetScaler != null)
            {
                m_TargetScaler.onCanvasAreaChanged.RemoveListener((b1, b2) => Handle());
            }
        }

        /// <summary>
        /// Gets the root scaler from a given Transform.
        /// <para></para>
        /// If transform has a root canvas, it will be returned. Otherwise, returns null.
        /// </summary>
        /// <param name="transform">The transform to get the root scaler of.</param>
        public static MaterialUIScaler GetRootScaler(Transform transform)
        {
            if (transform == null)
            {
                return null;
            }

            Canvas canvas = transform.GetRootCanvas();

            if (canvas == null)
            {
                return null;
            }

            return canvas.gameObject.GetAddComponent<MaterialUIScaler>();
        }
    }
}