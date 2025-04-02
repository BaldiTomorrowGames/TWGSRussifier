using UnityEngine;

namespace TWGSRussifier.API
{
    [System.Serializable]
    public class BigScreenReplacement
    {
        public string elementName; 
        public string newText;     
        public Vector2 anchoredPosition; 
        public Vector2 sizeDelta;        
    }

    [System.Serializable]
    public class OverwritesStruct
    {
        public string langType = "Base";
        public string floors_one = "F1";
        public string floors_tutorial = "TUT";
        public string floors_two = "F2";
        public string floors_three = "F3";
        public string floors_end = "END";
        public string floors_store = "PIT";
        public string floors_win = "YAY";
        public string challanges_stealthy = "C1";
        public string challanges_speedy = "C2";
        public string challanges__grapple = "C3";
        public string trips_farm = "Farm";
        public string trips_camp = "Camp";

        public BigScreenReplacement[] bigScreenReplacements = new BigScreenReplacement[]
        {
            new BigScreenReplacement { elementName = "ResultsText", newText = "Результаты:", anchoredPosition = new Vector2(-64,104), sizeDelta = new Vector2(137.01f,33.45f) },
            new BigScreenReplacement { elementName = "TimeText", newText = "Время:" },
            new BigScreenReplacement { elementName = "YTPText", newText = "Получено ОТМ:", anchoredPosition = new Vector2(30,74.5f), sizeDelta = new Vector2(202,30) },
            new BigScreenReplacement { elementName = "YTPValue", anchoredPosition = new Vector2(38,74.5f) },
            new BigScreenReplacement { elementName = "TotalText", newText = "Всего ОТМ:" },
            new BigScreenReplacement { elementName = "GradeText", newText = "Текущая Оценка: ", anchoredPosition = new Vector2(35,-30.5f) },
            new BigScreenReplacement { elementName = "MultiplierText", newText = "Энерго-бонус:", anchoredPosition = new Vector2(30,39.5f) },
            new BigScreenReplacement { elementName = "MultiplierValue", anchoredPosition = new Vector2(35,39.5f) },
            new BigScreenReplacement { elementName = "GradeValue", anchoredPosition = new Vector2(35,-30.5f) },
            new BigScreenReplacement { elementName = "TimeBonusText", newText = "Бонус за Время!", anchoredPosition = new Vector2(70, -65), sizeDelta = new Vector2(94,30) },
            new BigScreenReplacement { elementName = "TimeBonusValue", anchoredPosition = new Vector2(85,-79) },
            new BigScreenReplacement { elementName = "GradeBonusText", newText = "Бонус за Оценку!", anchoredPosition = new Vector2(60,-31), sizeDelta = new Vector2(103,30) },
            new BigScreenReplacement { elementName = "GradeBonusValue", anchoredPosition = new Vector2(85,-45) },
        };
    }
}
