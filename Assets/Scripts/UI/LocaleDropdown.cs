using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace UI
{
    public class LocaleDropdown : MonoBehaviour
    {
        public TMP_Dropdown dropdown;

        private IEnumerator Start()
        {
            // Wait for the localization system to initialize
            yield return LocalizationSettings.InitializationOperation;
        
            /*LocalizationSettings.Instance.SetSelectedLocale(Application.absoluteURL.Contains("lang=en")
            ? LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier("en"))
            : LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier("fr_ca"))); */

            // Generate list of available Locales
            var options = new List<TMP_Dropdown.OptionData>();
            int selected = 0;
        
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
            {
                var locale = LocalizationSettings.AvailableLocales.Locales[i];
                if (LocalizationSettings.SelectedLocale == locale)
                    selected = i;
            }

            // Build dropdown, and select locale based on URL.
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
            {
                var locale = LocalizationSettings.AvailableLocales.Locales[i];

                if (Application.absoluteURL.Contains("lang=" + locale.Identifier.Code))
                {
                    selected = i;
                    LocalizationSettings.Instance.SetSelectedLocale(locale);
                }

                string localeName = locale.LocaleName;
                if(localeName.Contains("English"))
                    localeName = "English";
                else if(localeName.Contains("French"))
                    localeName = "Français";
            
                options.Add(new TMP_Dropdown.OptionData(localeName));
            }
        
            dropdown.options = options;
            dropdown.value = selected;
            dropdown.onValueChanged.AddListener(LocaleSelected);
        }

        private static void LocaleSelected(int index)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
        }
    }
}