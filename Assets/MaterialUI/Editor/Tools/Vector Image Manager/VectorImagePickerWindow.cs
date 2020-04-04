//  Copyright 2017 MaterialUI for Unity http://materialunity.com
//  Please see license file for terms and conditions of use, and more information.

using UnityEditor;
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DuoVia.FuzzyStrings;
using Object = UnityEngine.Object;

namespace MaterialUI
{
    public class VectorImagePickerWindow : EditorWindow
    {
        private int m_IconsPerPage = 600;
        private int m_CurrentPage;

        private int m_NumberOfIcons;
        private int m_NumberOfPages;

        private struct GlyphSearchProbability
        {
            public Glyph glyph;
            public double probability;
        }

        private EditorCoroutine m_SearchCoroutine;

        private string m_SearchText;
        private Glyph[] m_GlyphArray;

        private Vector2 m_ScrollPosition;
        private Vector2 m_IconViewScrollPosition;
        private static VectorImageData[] m_VectorImageDatas;
        private static Action m_RefreshAction;
        private static Object[] m_ObjectsToRefresh;
        private static int m_PreviewSize = 48;
        private VectorImageSet m_VectorImageSet;
        private Font m_IconFont;
        private GUIStyle m_GuiStyle;
        private static Texture2D m_BackdropTexture;

        private float m_LastClickTime = float.MinValue;
        private GUIStyle m_BottomBarBg = "ProjectBrowserBottomBarBg";

        public static void Show(VectorImageData data, Object objectToRefresh)
        {
            Show(new[] { data }, new[] { objectToRefresh }, null);
        }

        public static void Show(VectorImageData data, Object objectToRefresh, Action refreshAction)
        {
            Show(new[] { data }, new[] { objectToRefresh }, refreshAction);
        }

        public static void Show(VectorImageData[] datas, Object[] objectsToRefresh)
        {
            Show(datas, objectsToRefresh, null);
        }

        public static void Show(VectorImageData[] datas, Object[] objectsToRefresh, Action refreshAction)
        {
            m_VectorImageDatas = datas;
            m_ObjectsToRefresh = objectsToRefresh;
            m_RefreshAction = refreshAction;

            VectorImagePickerWindow window = CreateInstance<VectorImagePickerWindow>();
            window.ShowAuxWindow();
            window.minSize = new Vector2(397, 446);
            window.titleContent = new GUIContent("Icon Picker");

            m_PreviewSize = EditorPrefs.GetInt("ICON_CONFIG_PREVIEW_SIZE", 48);
        }

        void OnEnable()
        {
            hideFlags = HideFlags.HideAndDontSave;
        }

        private void OnFocus()
        {
            Repaint();
        }

        private void OnSelectionChange()
        {
            Repaint();
        }

        void OnGUI()
        {
            if (Event.current.isKey) // If we detect the user pressed the keyboard
            {
                EditorGUI.FocusTextInControl("SearchInputField");
            }

            if (Event.current.type == EventType.KeyDown)
            {
                KeyCode keyCode = Event.current.keyCode;
                if (keyCode != KeyCode.Return)
                {
                    if (keyCode == KeyCode.Escape)
                    {
                        base.Close();
                        GUIUtility.ExitGUI();
                        return;
                    }

                    if (keyCode == KeyCode.PageDown)
                    {
                        MovePageDown();
                        Repaint();
                        return;
                    }
                    else if (keyCode == KeyCode.PageUp)
                    {
                        MovePageUp();
                        Repaint();
                        return;
                    }
                }
                else
                {
                    Close();
                    GUIUtility.ExitGUI();
                    return;
                }
            }

            using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(m_ScrollPosition))
            {
                m_ScrollPosition = scrollViewScope.scrollPosition;

                using (new GUILayout.HorizontalScope())
                {
                    GUILayout.Space(5f);

                    using (new GUILayout.VerticalScope())
                    {
                        GUILayout.Space(10f);
                        DrawSearchTextField();
                        DrawPicker();
                    }

                    GUILayout.Space(5f);
                }
            }
        }

        public static void DrawIconPickLine(VectorImageData data, Object objectToRefresh, bool indent = false)
        {
            using (new GUILayout.HorizontalScope())
            {
                if (data.font == null)
                {
                    data.font = VectorImageManager.GetIconFont(VectorImageManager.GetAllIconSetNames()[0]);
                }

                GUIStyle iconGuiStyle = new GUIStyle { font = VectorImageManager.GetIconFont(data.font.name) };

                EditorGUILayout.PrefixLabel("Icon");

                if (indent)
                {
                    EditorGUI.indentLevel--;
                }

                EditorGUILayout.LabelField(IconDecoder.Decode(data.glyph.unicode), iconGuiStyle, GUILayout.Width(18f));
                EditorGUILayout.LabelField(data.glyph.name, GUILayout.MaxWidth(100f), GUILayout.MinWidth(0f));

                GUILayout.FlexibleSpace();

                if (GUILayout.Button("Pick icon", EditorStyles.miniButton, GUILayout.MaxWidth(60f)))
                {
                    Show(data, objectToRefresh);
                    return;
                }

                if (GUILayout.Button("X", EditorStyles.miniButton, GUILayout.MaxWidth(20f)))
                {
                    for (int i = 0; i < m_VectorImageDatas.Length; i++)
                    {
                        m_VectorImageDatas[i] = null;
                    }
                    return;
                }

                if (indent)
                {
                    EditorGUI.indentLevel++;
                }
            }
        }

        private void DrawPicker()
        {
            if (m_VectorImageDatas[0] == null)
            {
                GUILayout.Label("Invalid vector image");
                return;
            }

            if (m_VectorImageDatas[0].glyph == null)
            {
                GUILayout.Label("Invalid glyph");
                return;
            }

            if (m_VectorImageDatas[0].font == null)
            {
                m_VectorImageDatas[0].font = VectorImageManager.GetIconFont(VectorImageManager.GetAllIconSetNames()[1]);
            }

            string[] names = VectorImageManager.GetAllIconSetNames();

            if (!names.Contains(m_VectorImageDatas[0].font.name))
            {
                m_VectorImageDatas[0].font.name = names[1];
            }

            if (VectorImageManager.GetAllIconSetNames().Length > 0)
            {
                EditorGUI.BeginChangeCheck();
                GUIContent[] namesContents = new GUIContent[names.Length];
                for (int i = 0; i < names.Length; i++)
                {
                    namesContents[i] = new GUIContent(names[i]);
                }

                m_VectorImageDatas[0].font = VectorImageManager.GetIconFont(names[EditorGUILayout.Popup(new GUIContent("Current Pack"), names.ToList().IndexOf(m_VectorImageDatas[0].font.name), namesContents)]);

                bool changed = EditorGUI.EndChangeCheck();

                if (changed)
                {
                    m_IconViewScrollPosition = Vector2.zero;
                }

                if (changed || m_VectorImageSet == null || m_IconFont == null)
                {
                    UpdateFontPackInfo();
                }

                DrawIconList();
            }
            else
            {
                EditorGUILayout.HelpBox("No VectorImage fonts detected!", MessageType.Warning);
            }

            DrawBottomBar();
        }

        private void DrawBottomBar()
        {
            Rect bottomBarRect = new Rect(0f, base.position.height - 17f, base.position.width, 17f);
            GUI.Label(bottomBarRect, GUIContent.none, m_BottomBarBg);

            Rect iconLabelRect = new Rect(bottomBarRect.x + 4f, bottomBarRect.y + 1f, (bottomBarRect.width / 2f) - 8f, 17f);
            Rect scaleControlRect = new Rect((bottomBarRect.x + (bottomBarRect.width) - 55f - 16f) + 8f, bottomBarRect.y + 1f, 55f, 17f);
            Rect pageControlRect = new Rect(scaleControlRect.x - 128f - 12f, bottomBarRect.y + 2f, 128f, 14f);

            if (m_VectorImageDatas.Length > 0)
            {
                GUI.Label(iconLabelRect, m_VectorImageDatas[0].glyph.name);
            }

            if (m_NumberOfPages > 1)
            {
                Rect buttonPrevRect = new Rect(pageControlRect.x, pageControlRect.y, 40f, pageControlRect.height);
                Rect pageLabelRect = new Rect(pageControlRect.x + 40f + 4f, pageControlRect.y, (pageControlRect.width - 40f - 4f) * 2f, pageControlRect.height);
                Rect buttonNextRect = new Rect((pageControlRect.x + (pageControlRect.width) - 40f), pageControlRect.y, 40f, pageControlRect.height);

                if (GUI.Button(buttonPrevRect, "Prev"))
                {
                    MovePageDown();
                }

                if (GUI.Button(buttonNextRect, "Next"))
                {
                    MovePageUp();
                }

                GUI.Label(pageLabelRect, (m_CurrentPage + 1) + " of " + (m_NumberOfPages + 1));
            }
            
            EditorGUI.BeginChangeCheck();
            m_PreviewSize = (int)GUI.HorizontalSlider(scaleControlRect, m_PreviewSize, 15, 100);
            if (EditorGUI.EndChangeCheck())
            {
                EditorPrefs.SetInt("ICON_CONFIG_PREVIEW_SIZE", m_PreviewSize);
            }
        }

        private void DrawSearchTextField()
        {
            using (new GUILayout.HorizontalScope())
            {
                GUI.SetNextControlName("SearchInputField");
                EditorGUI.BeginChangeCheck();
                m_SearchText = EditorGUILayout.TextField("", m_SearchText, "SearchTextField");
                if (EditorGUI.EndChangeCheck())
                {
                    m_SearchText = m_SearchText.Trim();
                    OnSearchTextChanged();
                }

                if (GUILayout.Button("", "SearchCancelButton", GUILayout.Width(18f)))
                {
                    m_SearchText = String.Empty;
                    UpdateGlyphList();
                    GUI.FocusControl(null);
                }
            }

            GUILayout.Space(5f);
        }

        private void DrawIconList()
        {
            if (m_GlyphArray.Length == 0)
            {
                GUIStyle guiStyle = new GUIStyle();
                guiStyle.fontStyle = FontStyle.Bold;
                guiStyle.alignment = TextAnchor.MiddleCenter;

                EditorGUILayout.LabelField("No icon found for your search term: " + m_SearchText, guiStyle, GUILayout.Height(Screen.height - 80f));
                return;
            }

            float padded = m_PreviewSize + 5f;
            int columns = Mathf.FloorToInt((Screen.width - 25f) / padded);
            if (columns < 1) columns = 1;

            int offset = m_CurrentPage * m_IconsPerPage;
            Rect rect = new Rect(0f, 0, m_PreviewSize, m_PreviewSize);

            GUILayout.Space(5f);

            using (GUILayout.ScrollViewScope scrollViewScope = new GUILayout.ScrollViewScope(m_IconViewScrollPosition, GUILayout.Height(Screen.height - 80f)))
            {
                m_IconViewScrollPosition = scrollViewScope.scrollPosition;

                while (offset < Mathf.Min((1 + m_CurrentPage) * m_IconsPerPage, m_GlyphArray.Length))
                {
                    using (new GUILayout.HorizontalScope())
                    {
                        int col = 0;
                        rect.x = 0f;

                        for (; offset < m_GlyphArray.Length; ++offset)
                        {
                            // Change color of the selected VectorImage
                            if (m_VectorImageDatas[0].glyph.name == m_GlyphArray[offset].name)
                            {
                                GUI.backgroundColor = MaterialColor.iconDark;
                            }

                            if (GUI.Button(rect, new GUIContent("", m_GlyphArray[offset].name)))
                            {
                                if (Event.current.button == 0)
                                {
                                    SetGlyph(offset);

                                    if (Time.realtimeSinceStartup - m_LastClickTime < 0.3f)
                                    {
                                        Close();
                                    }

                                    m_LastClickTime = Time.realtimeSinceStartup;
                                }
                            }

                            if (Event.current.type == EventType.Repaint)
                            {
                                drawTiledTexture(rect);

                                m_GuiStyle.fontSize = m_PreviewSize;

                                string iconText = IconDecoder.Decode(@"\u" + m_GlyphArray[offset].unicode);
                                Vector2 size = m_GuiStyle.CalcSize(new GUIContent(iconText));

                                float maxSide = size.x > size.y ? size.x : size.y;
                                float scaleFactor = (m_PreviewSize / maxSide) * 0.9f;

                                m_GuiStyle.fontSize = Mathf.RoundToInt(m_PreviewSize * scaleFactor);
                                size *= scaleFactor;

                                Vector2 padding = new Vector2(rect.width - size.x, rect.height - size.y);
                                Rect iconRect = new Rect(rect.x + (padding.x / 2f), rect.y + (padding.y / 2f), rect.width - padding.x, rect.height - padding.y);

                                GUI.Label(iconRect, new GUIContent(iconText), m_GuiStyle);
                            }

                            GUI.backgroundColor = Color.white;

                            if (++col >= columns)
                            {
                                ++offset;
                                break;
                            }
                            rect.x += padded;
                        }
                    }
                    GUILayout.Space(padded);
                    rect.y += padded;
                }
            }
        }

        private void drawTiledTexture(Rect rect)
        {
            createCheckerTexture();

            GUI.BeginGroup(rect);
            {
                int width = Mathf.RoundToInt(rect.width);
                int height = Mathf.RoundToInt(rect.height);

                for (int y = 0; y < height; y += m_BackdropTexture.height)
                {
                    for (int x = 0; x < width; x += m_BackdropTexture.width)
                    {
                        GUI.DrawTexture(new Rect(x, y, m_BackdropTexture.width, m_BackdropTexture.height), m_BackdropTexture);
                    }
                }
            }
            GUI.EndGroup();
        }

        private static void createCheckerTexture()
        {
            if (m_BackdropTexture != null)
            {
                return;
            }

            Color c0 = new Color(0.1f, 0.1f, 0.1f, 0.5f);
            Color c1 = new Color(0.2f, 0.2f, 0.2f, 0.5f);

            m_BackdropTexture = new Texture2D(16, 16);
            m_BackdropTexture.name = "[Generated] Checker Texture";
            m_BackdropTexture.hideFlags = HideFlags.DontSave;

            for (int y = 0; y < 8; ++y) for (int x = 0; x < 8; ++x) m_BackdropTexture.SetPixel(x, y, c1);
            for (int y = 8; y < 16; ++y) for (int x = 0; x < 8; ++x) m_BackdropTexture.SetPixel(x, y, c0);
            for (int y = 0; y < 8; ++y) for (int x = 8; x < 16; ++x) m_BackdropTexture.SetPixel(x, y, c0);
            for (int y = 8; y < 16; ++y) for (int x = 8; x < 16; ++x) m_BackdropTexture.SetPixel(x, y, c1);

            m_BackdropTexture.Apply();
            m_BackdropTexture.filterMode = FilterMode.Point;
        }

        private void UpdateFontPackInfo()
        {
            string name = m_VectorImageDatas[0].font.name;
            m_VectorImageSet = VectorImageManager.GetIconSet(name);
            m_IconFont = VectorImageManager.GetIconFont(name);
            m_GuiStyle = new GUIStyle { font = m_IconFont };
            m_GuiStyle.normal.textColor = Color.white;

            UpdateGlyphList();

            // Assign the very first icon of the imageSet if the glyph is null
            Glyph glyph = m_VectorImageSet.iconGlyphList.Where(x => x.name.Equals(m_VectorImageDatas[0].glyph.name) && x.unicode.Equals(m_VectorImageDatas[0].glyph.unicode.Replace("\\u", ""))).FirstOrDefault();
            if (glyph == null)
            {
                SetGlyph(0);
            }

            m_NumberOfIcons = m_GlyphArray.Length;
            m_NumberOfPages = Mathf.CeilToInt(m_NumberOfIcons / m_IconsPerPage);
        }

        private void MovePageDown()
        {
            if (m_CurrentPage > 0)
            {
                m_CurrentPage--;
                m_IconViewScrollPosition = Vector2.zero;
            }
        }

        private void MovePageUp()
        {
            if (m_CurrentPage < m_NumberOfPages)
            {
                m_CurrentPage++;
                m_IconViewScrollPosition = Vector2.zero;
            }
        }

        //  Fuzzy search is nice, but slow compared to simply matching contents, so we only perform search if the text doesn't change within a period of time.
        //  This prevents the search from uselessly running multiple times if the user adds/removes characters quickly.
        
        private void OnSearchTextChanged()
        {

            if (m_SearchCoroutine != null)
            {
                m_SearchCoroutine.Stop();
                m_SearchCoroutine = null;
            }

            m_SearchCoroutine = EditorCoroutine.Start(SearchCoroutine());
        }

        private IEnumerator SearchCoroutine()
        {
            for (int i = 0; i < 50; i++)
            {
                yield return new WaitForEndOfFrame();
            }
            
            UpdateGlyphList();
        }

        private void UpdateGlyphList()
        {
            if (string.IsNullOrEmpty(m_SearchText))
            {
                m_GlyphArray = m_VectorImageSet.iconGlyphList.ToArray();
            }
            else
            {
                List<GlyphSearchProbability> glyphProbabilityList = new List<GlyphSearchProbability>();
                GlyphSearchProbability glyphSearchProbability = new GlyphSearchProbability();

                foreach (Glyph glyph in m_VectorImageSet.iconGlyphList)
                {
                    glyphSearchProbability.glyph = glyph;
                    glyphSearchProbability.probability = m_SearchText.FuzzyMatch(glyph.name);

                    if (glyphSearchProbability.probability > 0.15)
                    {
                        glyphProbabilityList.Add(glyphSearchProbability);
                    }
                }

                glyphProbabilityList.Sort((probability1, probability2) =>
                    {
                        if (probability1.probability < probability2.probability)
                        {
                            return 1;
                        }

                        if (probability1.probability > probability2.probability)
                        {
                            return -1;
                        }

                        return 0;
                    });

                m_GlyphArray = new Glyph[glyphProbabilityList.Count];

                for (var i = 0; i < glyphProbabilityList.Count; i++)
                {
                    m_GlyphArray[i] = glyphProbabilityList[i].glyph;
                }

                m_CurrentPage = 0;
                m_IconViewScrollPosition = Vector2.zero;

                Repaint();
            }
        }

        private void SetGlyph(int index)
        {
            if (m_VectorImageDatas != null)
            {
                if (m_ObjectsToRefresh != null)
                {
                    Undo.RecordObjects(m_ObjectsToRefresh, "Set Icon");
                }

                Glyph glyph = new Glyph(m_GlyphArray[index].name, m_GlyphArray[index].unicode, true);

                for (int i = 0; i < m_VectorImageDatas.Length; i++)
                {
                    m_VectorImageDatas[i].glyph = glyph;
                    m_VectorImageDatas[i].font = m_IconFont;
                }

                m_RefreshAction.InvokeIfNotNull();
            }
        }
    }
}