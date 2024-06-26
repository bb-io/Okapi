﻿using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Okapi.DataSourceHandlers.EnumHandlers;

public class LanguageDataHandler : IStaticDataSourceHandler
{
    private static Dictionary<string, string> EnumValues => new()
    {
        { "ab", "Abkhazian" },
        { "ab-GE", "Abkhazian (*Georgia)" },
        { "om", "Afaan Oromo" },
        { "om-ET", "Afaan Oromo (*Ethiopia)" },
        { "aa", "Afar" },
        { "aa-DJ", "Afar (*Djibouti)" },
        { "af", "Afrikaan" },
        { "af-ZA", "Afrikaan (*South Africa)" },
        { "am", "Amharic" },
        { "am-ET", "Amharic (*Etiopia)" },
        { "ar", "Arabic" },
        { "ar-SA", "Arabic (*Saudi Arabia)" },
        { "ar-DZ", "Arabic (Algeria)" },
        { "ar-BH", "Arabic (Bahrain)" },
        { "ar-KM", "Arabic (Comoros)" },
        { "ar-DJ", "Arabic (Djibouti)" },
        { "ar-EG", "Arabic (Egypt)" },
        { "ar-IQ", "Arabic (Iraq)" },
        { "ar-JO", "Arabic (Jordan)" },
        { "ar-KW", "Arabic (Kuwait)" },
        { "ar-LB", "Arabic (Lebanon)" },
        { "ar-LY", "Arabic (Libya)" },
        { "ar-MA", "Arabic (Morocco)" },
        { "ar-OM", "Arabic (Oman)" },
        { "ar-PS", "Arabic (Palestine)" },
        { "ar-QA", "Arabic (Qatar)" },
        { "ar-SD", "Arabic (Sudan)" },
        { "ar-SY", "Arabic (Syria)" },
        { "ar-TN", "Arabic (Tunisia)" },
        { "ar-AE", "Arabic (U.A.E.)" },
        { "ar-YE", "Arabic (Yemen)" },
        { "hy", "Armenian" },
        { "hy-AM", "Armenian (*Armenia)" },
        { "as", "Assamese" },
        { "as-IN", "Assamese (*India)" },
        { "ay", "Aymara" },
        { "ay-BO", "Aymara (*Bolivia)" },
        { "az", "Azerbaijani" },
        { "az-AZ", "Azerbaijani (*Azerbaijan)" },
        { "az-Cyrl-AZ", "Azerbaijani (Azerbaijan, Cyrillic)" },
        { "ba", "Bashkir" },
        { "ba-RU", "Bashkir (*Russia)" },
        { "eu", "Basque" },
        { "eu-ES", "Basque (*Spain)" },
        { "be", "Belarusian" },
        { "be-BY", "Belarusian (*Belarus)" },
        { "bn", "Bengali" },
        { "bn-BD", "Bengali (*Bengladesh)" },
        { "dz", "Bhutani" },
        { "dz-BT", "Bhutani (*Bhutan)" },
        { "bh", "Bihari" },
        { "bh-IN", "Bihari (*India)" },
        { "bi", "Bislama" },
        { "bi-VU", "Bislama (*Vanuatu)" },
        { "bs", "Bosnian" },
        { "bs-BA", "Bosnian (*Bosnia and Herzegovina)" },
        { "br", "Breton" },
        { "br-FR", "Breton (*France)" },
        { "bg", "Bulgarian" },
        { "bg-BG", "Bulgarian (*Bulgaria)" },
        { "my", "Burmese" },
        { "my-MM", "Burmese (*Myanmar)" },
        { "ca", "Catalan" },
        { "ca-ES", "Catalan (*Spain)" },
        { "zh", "Chinese" },
        { "zh-CN", "Chinese (*China, =Simplified)" },
        { "zh-HK", "Chinese (Hong-Kong)" },
        { "zh-MO", "Chinese (Macau)" },
        { "zh-SG", "Chinese (Singapore)" },
        { "zh-TW", "Chinese (Taiwan, =Traditional)" },
        { "zh-Hans", "Chinese (Simplified)" },
        { "zh-Hant", "Chinese (Traditional)" },
        { "kw", "Cornish Gaelic" },
        { "kw-GB", "Cornish Gaelic (*United Kingdom)" },
        { "co", "Corsican" },
        { "co-FR", "Corsican (*France)" },
        { "ht", "Haitian Creole" },
        { "ht-HT", "Haitian Creole (*Haiti)" },
        { "hr", "Croatian" },
        { "hr-HR", "Croatian (*Croatia)" },
        { "hr-BA", "Croatian (Bosnia and Herzegovina)" },
        { "cs", "Czech" },
        { "cs-CZ", "Czech (*Czech Republic)" },
        { "da", "Danish" },
        { "da-DK", "Danish (Denmark)" },
        { "snm", "Dhivehi" },
        { "snm-MV", "Dhivehi (*Maldives)" },
        { "nl", "Dutch" },
        { "nl-NL", "Dutch (*The Netherlands)" },
        { "nl-BE", "Dutch (Belgium, =Flemish)" },
        { "nl-SR", "Dutch (Surinam)" },
        { "en", "English" },
        { "en-US", "English (*United States)" },
        { "en-AU", "English (Australia)" },
        { "en-BS", "English (Bahamas)" },
        { "en-BB", "English (Barbados)" },
        { "en-BZ", "English (Belize)" },
        { "en-CA", "English (Canada)" },
        { "en-DM", "English (Dominica)" },
        { "en-GM", "English (Gambia)" },
        { "en-GH", "English (Ghana)" },
        { "en-GD", "English (Grenada)" },
        { "en-GY", "English (Guyana)" },
        { "en-IE", "English (Ireland)" },
        { "en-IN", "English (India)" },
        { "en-JM", "English (Jamaica)" },
        { "en-KI", "English (Kiribati)" },
        { "en-LS", "English (Lesotho)" },
        { "en-LR", "English (Liberia)" },
        { "en-MW", "English (Malawi)" },
        { "en-MU", "English (Mauritius)" },
        { "en-NA", "English (Namibia)" },
        { "en-NZ", "English (New Zealand)" },
        { "en-NG", "English (Nigeria)" },
        { "en-PG", "English (Papua New-Guinea)" },
        { "en-PH", "English (Philippines)" },
        { "en-ZA", "English (South Africa)" },
        { "en-SL", "English (Sierra Leone)" },
        { "en-VC", "English (St Vincent and the Grenadines)" },
        { "en-TT", "English (Trinidad and Tobago)" },
        { "en-GB", "English (United Kingdom)" },
        { "x-en-Carib", "English (Windows Caribbean)" },
        { "x-en-Neutral", "English (Windows Neutral)" },
        { "en-ZM", "English (Zambia)" },
        { "en-ZW", "English (Zimbabwe)" },
        { "eo", "Esperanto" },
        { "et", "Estonian" },
        { "et-EE", "Estonian (*Estonia)" },
        { "fo", "Faeroese" },
        { "fo-FO", "Faeroese (*Faeroese Islands)" },
        { "fa", "Farsi" },
        { "fa-IR", "Farsi (*Iran)" },
        { "fj", "Fijian" },
        { "fj-FJ", "Fijian (*Fiji)" },
        { "fi", "Finnish" },
        { "fi-FI", "Finnish (*Finland)" },
        { "fr", "French" },
        { "fr-FR", "French (*France)" },
        { "fr-BE", "French (Belgium)" },
        { "fr-BF", "French (Burkina Faso)" },
        { "fr-CM", "French (Cameroon)" },
        { "fr-CA", "French (Canada, =Québécois)" },
        { "fr-TD", "French (Chad)" },
        { "fr-KM", "French (Comoros)" },
        { "fr-CG", "French (Congo)" },
        { "fr-CI", "French (Côte d'Ivoire)" },
        { "fr-DJ", "French (Djibouti)" },
        { "fr-GA", "French (Gabon)" },
        { "fr-GN", "French (Guinea)" },
        { "fr-HT", "French (Haiti)" },
        { "fr-LU", "French (Luxembourg)" },
        { "fr-ML", "French (Mali)" },
        { "fr-MR", "French (Mauritania)" },
        { "fr-MC", "French (Monaco)" },
        { "fr-SN", "French (Senegal)" },
        { "fr-CH", "French (Switzerland)" },
        { "fr-TG", "French (Togo)" },
        { "fr-ZR", "French (Zaire)" },
        { "fy", "Frisian" },
        { "fy-NL", "Frisian (*The Netherlands)" },
        { "gl", "Galician" },
        { "gl-ES", "Gallegan (*Spain)" },
        { "ka", "Georgian" },
        { "ka-GE", "Georian (*Georgia)" },
        { "de", "German" },
        { "de-DE", "German (*Germany)" },
        { "de-AT", "German (Austria)" },
        { "de-LI", "German (Liechtenstein)" },
        { "de-LU", "German (Luxembourg)" },
        { "de-CH", "German (Switzerland)" },
        { "gil", "Gilbertese" },
        { "gil-KI", "Gilbertese (*Kiribati)" },
        { "gil-TV", "Gilbertese (Tuvalu)" },
        { "el", "Greek" },
        { "el-GR", "Greek (*Greece)" },
        { "el-CY", "Greek (Cyprus)" },
        { "gn", "Guarani" },
        { "gn-PY", "Guarani (*Paraguay)" },
        { "gu", "Gujarati" },
        { "gu-IN", "Gujarati (*India)" },
        { "ha", "Hausa" },
        { "ha-NG", "Hausa (*Nigeria)" },
        { "he", "Hebrew" },
        { "he-IL", "Hebrew (*Israel)" },
        { "hi", "Hindi" },
        { "hi-IN", "Hindi (*India)" },
        { "hu", "Hungarian" },
        { "hu-HU", "Hungarian (*Hungarian)" },
        { "is", "Icelandic" },
        { "is-IS", "Icelandic (*Iceland)" },
        { "id", "Indonesian" },
        { "id-id", "Indonesian (*Indonesia)" },
        { "ia", "Interlingua" },
        { "ie", "Interlingue" },
        { "iu", "Inuktitut" },
        { "iu-CA", "Inuktitut (*Canada)" },
        { "ik", "Inupiak" },
        { "ik-US", "Inupiak (*United States)" },
        { "ga", "Irish Gaelic" },
        { "ga-IE", "Irish Gaelic (*Ireland)" },
        { "it", "Italian" },
        { "it-IT", "Italian (*Italy)" },
        { "it-SM", "Italian (San Marino)" },
        { "it-CH", "Italian (Switzerland)" },
        { "ja", "Japanese" },
        { "ja-JP", "Japanese (*Japan)" },
        { "jw", "Javanese" },
        { "jw-ID", "Javanese (*Indonesia)" },
        { "kl", "Kalaallisut" },
        { "kl-GL", "Kalaallisut (*Greenland)" },
        { "kn", "Kannada" },
        { "kn-IN", "Kannada (*India)" },
        { "ks", "Kashmiri" },
        { "ks-IN", "Kashmiri (*India)" },
        { "kk", "Kazakh" },
        { "kk-KZ", "Kazakh (*Kazakhstan)" },
        { "rw", "Kinyarwanda" },
        { "rw-RW", "Kinyarwanda (*Rwanda)" },
        { "ky", "Kirghiz" },
        { "ky-KG", "Kirghiz (*Kyrgyzstan)" },
        { "rn", "Kirundi" },
        { "rn-BI", "Kirundi (*Burundi)" },
        { "km", "Khmer" },
        { "km-KH", "Khmer (*Cambodia)" },
        { "kok", "Konkani" },
        { "kok-IN", "Konkani (*India)" },
        { "ko", "Korean" },
        { "ko-KR", "Korean (*Korea)" },
        { "ko-KP", "Korean (North Korea)" },
        { "lo", "Lao" },
        { "lo-LA", "Lao (*Laos)" },
        { "lap", "Lappish" },
        { "lap-NO", "Lappish (*Norway)" },
        { "la", "Latin" },
        { "la-VA", "Latin (*Vatican)" },
        { "lv", "Latvian" },
        { "lv-LV", "Latvian (*Latvia)" },
        { "ln", "Lingala" },
        { "ln-CG", "Lingala (*Congo)" },
        { "lt", "Lithuanian" },
        { "lt-LT", "Lithuanian (*Lithuania)" },
        { "x-lt-LT-Classic", "Lithuanian (Lithuania, Classical)" },
        { "lb", "Luxemburgian" },
        { "lb-LU", "Luxemburgian (*Luxembourg)" },
        { "mk", "Macedonian" },
        { "mk-MK", "Macedonian (*Macedonia)" },
        { "mg", "Malagasy" },
        { "mg-MG", "Malagasy (*Madagascar)" },
        { "ms", "Malay" },
        { "ms-MY", "Malay (*Malaysia)" },
        { "ms-BN", "Malay (Brunei Darussalam)" },
        { "ml", "Malayalam" },
        { "ml-IN", "Malayalam (*India)" },
        { "mt", "Maltese" },
        { "mt-MT", "Maltese (*Malta)" },
        { "mni", "Manipuri" },
        { "mni-IN", "Manipuri (*India)" },
        { "gv", "Manx Gaelic" },
        { "gv-GB", "Manx Gaelic (*United Kingdom)" },
        { "mi", "Maori" },
        { "mi-NZ", "Maori (*New Zealand)" },
        { "mr", "Marathi" },
        { "mr-IN", "Marathi (*India)" },
        { "mo", "Moldovan" },
        { "mo-MD", "Moldovan (*Moldova)" },
        { "mn", "Mongolian" },
        { "mn-MN", "Mongolian (*Mongolia)" },
        { "na", "Nauruan" },
        { "na-NR", "Nauruan (*Nauru)" },
        { "nv", "Navajo" },
        { "nv-US", "Navajo (*United States)" },
        { "ne", "Nepali" },
        { "ne-NP", "Nepali (*Nepal)" },
        { "ne-IN", "Nepali (India)" },
        { "no", "Norwegian" },
        { "no-NO", "Norwegian (*Norway)" },
        { "nn-NO", "Norwegian Nynorsk (*Norway)" },
        { "oc", "Occitan" },
        { "oc-FR", "Occitan (*France)" },
        { "or", "Oriya" },
        { "or-IN", "Oriya (*India)" },
        { "ps", "Pashto" },
        { "ps-AF", "Pashto (*Afghanistan)" },
        { "pl", "Polish" },
        { "pl-PL", "Polish (*Poland)" },
        { "pt", "Portuguese" },
        { "pt-BR", "Portuguese (*Brazil)" },
        { "pt-AO", "Portuguese (Angola)" },
        { "pt-CV", "Portuguese (Cape Verde)" },
        { "pt-GW", "Portuguese (Guinea-Bissau)" },
        { "pt-MZ", "Portuguese (Mozambique)" },
        { "pt-PT", "Portuguese (Portugal)" },
        { "pa", "Punjabi" },
        { "pa-IN", "Punjabi (*India)" },
        { "qu", "Quechua" },
        { "qu-PE", "Quechua (*Peru)" },
        { "ro", "Romanian" },
        { "ro-ro", "Romanian (*Romania)" },
        { "ro-md", "Romanian (Moldova)" },
        { "rm", "Raetho-Roman" },
        { "rm-ch", "Raetho-Roman (*Switzerland)" },
        { "ru", "Russian" },
        { "ru-ru", "Russian (*Russia)" },
        { "ru-md", "Russian (Moldova)" },
        { "sa", "Sanskrit" },
        { "sa-in", "Sanskrit (*India)" },
        { "sd", "Sindhi" },
        { "sd-in", "Sindhi (*India)" },
        { "si", "Singhalese" },
        { "si-lk", "Singhalese (*Sri-Lanka)" },
        { "sk", "Slovak" },
        { "sk-sk", "Slovak (*Slovakia)" },
        { "sl", "Slovenian" },
        { "sl-si", "Slovenian (*Slovenia)" },
        { "sm", "Samoan" },
        { "sm-ws", "Samoan (*Western Samoa)" },
        { "sq", "Albanian" },
        { "sq-AL", "Albanian (*Albania)" },
        { "sr", "Serbian" },
        { "sr-Cyrl-SP", "Serbian (*Serbia)" },
        { "sr-Cyrl-BA", "Serbian (Bosnia and Herzegovina)" },
        { "ss", "Siswati" },
        { "ss-sz", "Siswati (*Swaziland)" },
        { "ss-za", "Siswati (South-Africa)" },
        { "st", "Sesotho" },
        { "st-ls", "Sesotho (*Lesotho)" },
        { "sv", "Swedish" },
        { "sv-SE", "Swedish (*Sweden)" },
        { "sv-AX", "Swedish (Åland Islands)" },
        { "sv-FI", "Swedish (Finland)" },
        { "sw", "Swahili" },
        { "sw-KE", "Swahili (*Kenya)" },
        { "syr", "Syriac" },
        { "gd", "Scottish Gaelic" },
        { "gd-GB", "Scottish Gaelic (*United Kingdom)" },
        { "tet", "Tetum" },
        { "tet-TL", "Tetum (*Timor-Leste)" },
        { "yo", "Yoruba" },
        { "yo-NG", "Yoruba (*Nigeria)" },
        { "yi", "Yiddish" },
        { "yi-IL", "Yiddish (*Israel)" },
        { "vi", "Vietnamese" },
        { "vi-VN", "Vietnamese (*Vietnam)" },
        { "wo", "Wolof" },
        { "wo-SN", "Wolof (*Senegal)" },
        { "wen", "Sorbian" },
        { "wen-DE", "Sorbian (*Germany)" },
        { "x-win-Neutral", "Windows Neutral" },
        { "x-win-Default", "Windows Neutral (Default)" },
        { "x-win-System", "Windows Neutral (System)" },
        { "bo", "Tibetan" },
        { "bo-ZH", "Tibetan (*China)" },
        { "cy", "Welsh" },
        { "cy-GB", "Welsh (*United Kingdom)" },
        { "es", "Spanish" },
        { "es-ES", "Spanish (*Spain)" },
        { "es-AR", "Spanish (Argentina)" },
        { "es-BO", "Spanish (Bolivia)" },
        { "es-CL", "Spanish (Chile)" },
        { "es-CO", "Spanish (Colombia)" },
        { "es-CR", "Spanish (Costa Rica)" },
        { "es-CU", "Spanish (Cuba)" },
        { "es-DO", "Spanish (Dominican Republic)" },
        { "es-EC", "Spanish (Ecuador)" },
        { "es-GQ", "Spanish (Equatorial Guinea)" },
        { "es-GT", "Spanish (Guatemala)" },
        { "es-HN", "Spanish (Honduras)" },
        { "es-MX", "Spanish (Mexico)" },
        { "es-NI", "Spanish (Nicaragua)" },
        { "es-PA", "Spanish (Panama)" },
        { "es-PE", "Spanish (Peru)" },
        { "es-PR", "Spanish (Puerto Rico)" },
        { "es-PY", "Spanish (Paraguay)" },
        { "es-SV", "Spanish (El Salvador)" },
        { "es-UY", "Spanish (Uruguay)" },
        { "es-VE", "Spanish (Venezuela)" },
        { "es-419", "Spanish (Latin American &amp; the Caribbean)" },
        { "es-x-Modern", "Spanish (Modern Sort)" },
        { "th", "Thai" },
        { "th-TH", "Thai (*Thailand)" },
        { "tr", "Turkish" },
        { "tr-TR", "Turkish (*Turkey)" },
        { "tr-CY", "Turkish (Cyprus)" },
        { "uk", "Ukrainian" },
        { "uk-UA", "Ukrainian (*Ukraine)" },
        { "ur", "Urdu" },
        { "uk-PK", "Urdu (*Pakistan)" },
        { "ug", "Uighur" },
        { "ug-CN", "Uighur (*China)" },
        { "zu", "Zulu" },
        { "zu-ZA", "Zulu (*south Africa)" },
    };

    public Dictionary<string, string> GetData()
    {
        return EnumValues;
    }
}