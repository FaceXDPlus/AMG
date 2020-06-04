using MaterialUI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityExtensions;
using UnityExtensions.Localization;

namespace AMG
{
    public class LangController : MonoBehaviour
    {
        public bool InitStatus = false;
        [SerializeField] private Canvas Canvas;

        //语言选择
        [SerializeField] private SelectionBoxConfig LanguageDropdownBox;
        IEnumerator Start()
        {
            yield return StartCoroutine(InitLanguage());

            LanguageDropdownBox.listItems = new string[LocalizationManager.languageCount];
            for (int i = 0; i < LocalizationManager.languageCount; i++)
            {
                LanguageDropdownBox.listItems[i] = LocalizationManager.GetLanguageAttribute(i, "LanguageName");
            }
            LanguageDropdownBox.selectedText.text = LocalizationManager.GetLanguageAttribute(LocalizationManager.languageIndex, "LanguageName");
            LanguageDropdownBox.RefreshList();
            LanguageDropdownBox.ItemPicked += OnLanguageSelected;
            Canvas.gameObject.SetActive(true);
        }

        public void OnLanguageSelected(int id)
        {
            SetLanguage(id);
        }

        private IEnumerator InitLanguage()
        {
            var languageType = GetDefaultLanguageType();
            LocalizationManager.LoadMetaAsync();
            LocalizationManager.LoadLanguageAsync(languageType, _ => InitStatus = true);
            while (!InitStatus)
            {
                yield return null;
            }
        }

        static string GetDefaultLanguageType()
        {
            switch (Application.systemLanguage)
            {
                case SystemLanguage.Japanese:
                    return "ja-JP";

                case SystemLanguage.Chinese:
                case SystemLanguage.ChineseSimplified:
                case SystemLanguage.ChineseTraditional:
                    return "zh-CN";

                default:
                    return "en-US";
            }
        }

        void SetLanguage(int index)
        {
            if (index != LocalizationManager.languageIndex)
            {
                InitStatus = false;
                LocalizationManager.LoadLanguageAsync(index, _ => InitStatus = true);
            }
        }

        public string GetLang(string text, params object[] args)
        {
            if (InitStatus == true)
            {
                if (LocalizationManager.HasText(text))
                {
                    var lang = LocalizationManager.GetText(text);
                    lang = string.Format(lang, args);
                    return lang;
                }
            }
            return null;
        }
    }
}