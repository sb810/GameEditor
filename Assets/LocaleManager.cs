using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SocialPlatforms;

public class LocaleManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        LocalizationSettings.Instance.SetSelectedLocale(Application.absoluteURL.Contains("lang=en")
            ? LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier("en"))
            : LocalizationSettings.AvailableLocales.GetLocale(new LocaleIdentifier("fr_ca")));
    }
}
