using System;
using TMPro;
using UnityEngine;

namespace AngleVisualiser
{
    [Serializable]
    public class TextData
    {
        public Canvas canvas;
        public TMP_Text description;
        public TextData(Transform parent, string name)
        {
            CreateTextData(parent, name);
        }
        public void ClearText()
        {
            description.SetText("");
        }
        public void DrawText(Vector3 worlPosition, Quaternion worldRotation, string value)
        {
            canvas.transform.position = worlPosition;

            canvas.transform.rotation = worldRotation;

            description.SetText(value);
        }
        void CreateTextData(Transform parent, string name)
        {
            //Canvas
            var canvasObj = new GameObject($"{name}");
            canvasObj.transform.SetParent(parent);
            canvasObj.transform.localPosition = Vector3.zero;

            var canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
            var rect = canvas.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(.5f, .2f);

            //Text
            var textObj = new GameObject($"{name}");
            textObj.transform.SetParent(canvasObj.transform);
            textObj.transform.localPosition = Vector3.zero;

            var tmpUGUI = textObj.AddComponent<TextMeshProUGUI>();
            tmpUGUI.fontSize = 0.1f;
            tmpUGUI.color = Color.black;
            var textRect = tmpUGUI.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0, 0);
            textRect.anchorMax = new Vector2(1, 1);
            textRect.pivot = new Vector2(0.5f, 0.5f);
            textRect.sizeDelta = new Vector2(0, 0);

            this.canvas = canvas;
            this.description = tmpUGUI.GetComponent<TMP_Text>();
        }
    }
}
