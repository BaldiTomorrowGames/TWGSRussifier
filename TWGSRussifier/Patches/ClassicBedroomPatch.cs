using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using TWGSRussifier.API;
using Logger = TWGSRussifier.API.Logger;

namespace TWGSRussifier.Patches
{
    [HarmonyPatch(typeof(ClassicBasementManager), "BeginPlay", new System.Type[] { })]
    internal class ClassicBedroomPatch
    {
        private static Texture2D? sheetTexture = null;
        private static Sprite[] russianLetters = new Sprite[33];
        private static bool isInitialized = false;
        private static bool prefabChangesDone = false;

        private static readonly string[] AlphabetSheetData = new string[]
        {
            "А;24;383;66;43",
            "Б;94;383;60;46",
            "В;158;384;58;45",
            "Г;218;384;57;44",
            "Д;278;384;75;53",
            "Е;356;382;63;48",
            "Ё;420;382;63;58",
            "Ж;21;322;85;43",
            "З;109;322;55;44",
            "И;166;321;72;45",
            "Й;241;321;72;59",
            "К;315;322;60;44",
            "Л;376;322;76;44",
            "М;23;258;88;46",
            "Н;113;258;70;47",
            "О;184;259;75;45",
            "П;260;260;83;45",
            "Р;346;245;56;60",
            "С;404;259;60;45",
            "Т;23;195;61;44",
            "У;87;180;65;59",
            "Ф;155;180;79;59",
            "Х;236;194;73;45",
            "Ц;311;188;76;52",
            "Ч;388;195;61;45",
            "Ш;22;133;91;45",
            "Щ;118;125;98;53",
            "Ъ;219;134;77;43",
            "Ы;301;133;102;44",
            "Ь;405;134;57;43",
            "Э;22;71;62;44",
            "Ю;88;71;113;45",
            "Я;206;71;61;45"
        };

        [HarmonyPostfix]
        private static void Postfix(ClassicBasementManager __instance)
        {
            if (!ConfigManager.AreTexturesEnabled())
            {
               // Logger.Info("Замена букв в спальне отключена в конфигурации");
                return;
            }

            try
            {
                if (!isInitialized)
                {
                    LoadRussianAlphabet();
                    isInitialized = true;
                }

                if (sheetTexture == null || russianLetters[0] == null)
                {
                   // Logger.Warning("Русский алфавит не загружен, пропускаем замену букв");
                    return;
                }

                RoomController bedroom = __instance.Ec.rooms[6];
                Transform wallLetters = bedroom.objectObject.transform.Find("WallLetters(Clone)");

                if (wallLetters == null)
                {
                   // Logger.Warning("WallLetters(Clone) не найден в спальне");
                    return;
                }

                SpriteRenderer[] englishABCSorted = wallLetters.GetComponentsInChildren<SpriteRenderer>();
                Array.Sort(englishABCSorted, (x, y) => string.Compare(x.name, y.name));

              // Logger.Info($"Найдено {englishABCSorted.Length} английских букв, заменяем на русские");

                for (int i = 0; i < englishABCSorted.Length && i < russianLetters.Length; i++)
                {
                    if (russianLetters[i] != null)
                    {
                        englishABCSorted[i].sprite = russianLetters[i];
                    }
                }

                SpriteRenderer[] additionalLetters = new SpriteRenderer[7];
                for (int i = 0; i < additionalLetters.Length; i++)
                {
                    additionalLetters[i] = UnityEngine.Object.Instantiate(englishABCSorted[englishABCSorted.Length - 1]);
                    additionalLetters[i].transform.SetParent(englishABCSorted[englishABCSorted.Length - 1].transform.parent);
                    additionalLetters[i].sprite = russianLetters[26 + i];
                    additionalLetters[i].name = new string(additionalLetters[i].sprite.name[additionalLetters[i].sprite.name.Length - 1], 1);
                }

                SetupLetter(additionalLetters[5], new Vector3(2f, 2.2727f, 14.95f), new Vector3(0f, 0f, 11.1309f), new Color(1f, 0f, 0f, 1f)); // Щ
                SetupLetter(additionalLetters[1], new Vector3(5f, 3f, 14.95f), Vector3.zero, new Color(0f, 1f, 1f, 1f));                       // Я
                SetupLetter(additionalLetters[3], new Vector3(7.6253f, 2.6436f, 14.95f), Vector3.zero, new Color(0.64f, 0.4976f, 0.766f, 1f)); // Ь
                SetupLetter(additionalLetters[0], new Vector3(10f, 3f, 14.95f), Vector3.zero, new Color(0.4137f, 0.7f, 1f, 1f)); // Э
                SetupLetter(additionalLetters[6], new Vector3(12.2546f, 2.3164f, 14.95f), new Vector3(0f, 0f, 335.6689f), new Color(0.1137f, 0.1176f, 0.6906f, 1f)); // Ъ
                SetupLetter(additionalLetters[2], new Vector3(15f, 3f, 14.95f), new Vector3(0f, 0f, 348.052f), new Color(0.5937f, 0.3176f, 0.2f, 1f)); // Ы
                SetupLetter(additionalLetters[4], new Vector3(18.3128f, 2.3818f, 14.95f), new Vector3(0f, 0f, 11.42f), new Color(0.54f, 0f, 0.472f, 1f)); // Ю

               // Logger.Info("Русские буквы успешно размещены в спальне Балди");
            }
            catch (Exception)
            {
               // Logger.Error($"Ошибка при замене букв в спальне: {ex.Message}");
            }

            if (!prefabChangesDone)
            {
                try
                {
                    ApplyBookPrefabLocalization(__instance);
                    prefabChangesDone = true;
                }
                catch (Exception)
                {
                   // Logger.Error($"Ошибка при модификации префабов книг: {ex.Message}");
                }
            }
        }

        private static void LoadRussianAlphabet()
        {
            try
            {
                string texturePath = Path.Combine(RussifierTemp.GetTexturePath(), "Alphabet_Ru_Sheet.png");
                sheetTexture = AssetLoader.TextureFromFile(texturePath, TextureFormat.ARGB32);

                if (sheetTexture == null)
                {
                   // Logger.Error($"Не удалось загрузить Alphabet_Ru_Sheet.png из {texturePath}");
                   // Logger.Info("Поместите файл Alphabet_Ru_Sheet.png в папку Textures");
                    return;
                }

                sheetTexture.filterMode = FilterMode.Point;
                sheetTexture.Apply();
                sheetTexture.name = "Alphabet_Ru_Sheet";

               // Logger.Info($"Текстура русского алфавита загружена из Textures: {sheetTexture.width}x{sheetTexture.height}");

               // Logger.Info($"Обрабатываем {AlphabetSheetData.Length} букв русского алфавита");

                for (int i = 0; i < AlphabetSheetData.Length && i < russianLetters.Length; i++)
                {
                    string line = AlphabetSheetData[i];
                    
                    if (string.IsNullOrWhiteSpace(line))
                        continue;

                    string[] splits = line.Split(new char[] { ';' });

                    if (splits.Length < 5)
                    {
                       // Logger.Warning($"Некорректная строка данных: {line}");
                        continue;
                    }

                    string letterName = splits[0];
                    int x = int.Parse(splits[1]);
                    int y = int.Parse(splits[2]);
                    int width = int.Parse(splits[3]);
                    int height = int.Parse(splits[4]);

                    Sprite russianLetter = Sprite.Create(
                        sheetTexture,
                        new Rect(x, y, width, height),
                        new Vector2(0.5f, 0.5f),
                        40f
                    );
                    russianLetter.name = $"Alphabet_Ru_{letterName}";
                    russianLetters[i] = russianLetter;
                }

               // Logger.Info($"Создано {russianLetters.Length} спрайтов русских букв");
            }
            catch (Exception)
            {
               // Logger.Error($"Ошибка при загрузке русского алфавита: {ex.Message}");
            }
        }

        private static void SetupLetter(SpriteRenderer letter, Vector3 localPosition, Vector3 localEuler, Color color)
        {
            letter.transform.localPosition = localPosition;
            letter.transform.localEulerAngles = localEuler;
            letter.color = color;
        }

        private static void ApplyBookPrefabLocalization(ClassicBasementManager instance)
        {
            try
            {
                Type cmpType = instance.GetType();
                FieldInfo bookPreField = cmpType.GetField("bookPre", BindingFlags.Instance | BindingFlags.NonPublic);
                
                if (bookPreField == null)
                {
                   // Logger.Warning("Поле bookPre не найдено в ClassicBasementManager");
                    return;
                }

                ClassicMathBook[]? bookPre = bookPreField.GetValue(instance) as ClassicMathBook[];
                
                if (bookPre == null || bookPre.Length == 0)
                {
                   // Logger.Warning("Массив bookPre пуст или null");
                    return;
                }

               // Logger.Info($"Найдено {bookPre.Length} префабов книг, применяем локализацию");

                for (int i = 0; i < bookPre.Length; i++)
                {
                    if (bookPre[i] != null)
                    {
                        ApplyLocalizationToBookPrefab(bookPre[i]);
                    }
                }

               // Logger.Info("Локализация применена ко всем префабам книг");
            }
            catch (Exception)
            {
               // Logger.Error($"Ошибка при применении локализации к префабам книг: {ex.Message}");
            }
        }

        private static void ApplyLocalizationToBookPrefab(ClassicMathBook book)
        {
            try
            {
                Transform canvasChild = book.transform.GetChild(0);
                if (canvasChild == null) return;

                Transform coverText = canvasChild.Find("CoverText");
                if (coverText == null) return;

                Transform titleTransform = coverText.Find("TitleTMP");
                if (titleTransform != null)
                {
                    ApplyTextLocalizer(titleTransform, "TWGS_ClassicBook_Title", 26f);
                }

                Transform subTransform = coverText.Find("SubTMP");
                if (subTransform != null)
                {
                    string bookIdentifier = GetBookIdentifierFromName(book.name);
                    ApplyTextLocalizer(subTransform, $"TWGS_ClassicBook_{bookIdentifier}");
                }

                Transform authorTransform = coverText.Find("AuthorTMP");
                if (authorTransform != null)
                {
                    ApplyTextLocalizer(authorTransform, "TWGS_ClassicBook_Author");
                }

                Transform insideText = canvasChild.Find("InsideText");
                if (insideText != null)
                {
                    TMP_Text[] insideTexts = insideText.GetComponentsInChildren<TMP_Text>();
                    for (int i = 0; i < insideTexts.Length && i <= 5; i++)
                    {
                        ApplyTextLocalizer(insideTexts[i].transform, $"TWGS_ClassicBook_Text_{i}");
                    }
                }

               // Logger.Info($"Локализация применена к книге: {book.name}");
            }
            catch (Exception)
            {
               // Logger.Error($"Ошибка при применении локализации к книге {book.name}: {ex.Message}");
            }
        }

        private static void ApplyTextLocalizer(Transform textTransform, string localizationKey, float? fontSize = null)
        {
            if (textTransform == null) return;

            TextMeshProUGUI textComponent = textTransform.GetComponent<TextMeshProUGUI>();
            if (textComponent == null) return;

            TextLocalizer existingLocalizer = textComponent.GetComponent<TextLocalizer>();
            if (existingLocalizer != null)
            {
               // Logger.Info($"TextLocalizer уже существует для {textTransform.name}");
                return;
            }

            TextLocalizer localizer = textComponent.gameObject.AddComponent<TextLocalizer>();
            localizer.key = localizationKey;

            if (fontSize.HasValue)
            {
                textComponent.fontSize = fontSize.Value;
            }

            localizer.RefreshLocalization();

           // Logger.Info($"TextLocalizer добавлен: {textTransform.name} -> {localizationKey}");
        }

        private static string GetBookIdentifierFromName(string bookName)
        {
            if (bookName.Contains("Preschool")) return "Preschool";
            if (bookName.Contains("Kindergarten")) return "Kindergarten";
            if (bookName.Contains("College")) return "College";
            if (bookName.Contains("ClassicBook_10")) return "10";
            if (bookName.Contains("ClassicBook_11")) return "11";
            if (bookName.Contains("ClassicBook_12")) return "12";
            if (bookName.Contains("ClassicBook_1")) return "1";
            if (bookName.Contains("ClassicBook_2")) return "2";
            if (bookName.Contains("ClassicBook_3")) return "3";
            if (bookName.Contains("ClassicBook_4")) return "4";
            if (bookName.Contains("ClassicBook_5")) return "5";
            if (bookName.Contains("ClassicBook_6")) return "6";
            if (bookName.Contains("ClassicBook_7")) return "7";
            if (bookName.Contains("ClassicBook_8")) return "8";
            if (bookName.Contains("ClassicBook_9")) return "9";
            
            return "Default";
        }

        [HarmonyPatch(typeof(CoreGameManager), "ReturnToMenu")]
        private static class ReturnToMenuPatch
        {
            [HarmonyPrefix]
            private static void Prefix()
            {
                prefabChangesDone = false;
            }
        }
    }
}

