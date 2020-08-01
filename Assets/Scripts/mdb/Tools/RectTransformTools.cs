using UnityEngine;

namespace mdb.Tools
{
    public static class RectTransformTools
    {
        public static void SetLeft(RectTransform rectTransform, float left)
        {
            rectTransform.offsetMin = new Vector2(left, rectTransform.offsetMin.y);
        }

        public static void SetRight(this RectTransform rectTransform, float right)
        {
            rectTransform.offsetMax = new Vector2(-right, rectTransform.offsetMax.y);
        }

        public static void SetTop(this RectTransform rectTransform, float top)
        {
            rectTransform.offsetMax = new Vector2(rectTransform.offsetMax.x, -top);
        }

        public static void SetBottom(this RectTransform rectTransform, float bottom)
        {
            rectTransform.offsetMin = new Vector2(rectTransform.offsetMin.x, bottom);
        }

        public static RectTransform CreateStretched(string name, RectTransform parent, Vector2 anchorMin, Vector2 anchorMax)
        {
            GameObject gameObject = new GameObject(name, typeof(RectTransform));
            RectTransform rectTransform = gameObject.GetComponent<RectTransform>();

            rectTransform.anchoredPosition = parent.position;
            rectTransform.anchorMin = anchorMin;
            rectTransform.anchorMax = anchorMax;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            rectTransform.sizeDelta = parent.rect.size;
            rectTransform.transform.SetParent(parent);

            SetTop(rectTransform, 0);
            SetLeft(rectTransform, 0);
            SetBottom(rectTransform, 0);
            SetRight(rectTransform, 0);

            return rectTransform;
        }
    }
}