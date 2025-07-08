using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace TWGSRussifier.Patches
{
    class TransformFixator : MonoBehaviour
    {
        private RectTransform? rectTransform;
        private Vector2 anchorMin, anchorMax;
        private Vector2 offsetMin, offsetMax;
        private Vector3 localPosition;
        private Quaternion localRotation;
        private Vector2 pivot;
        private Vector2 sizeDelta;

        private void Awake()
        {
            localPosition = transform.localPosition;
            localRotation = transform.localRotation;

            rectTransform = GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                anchorMin = rectTransform.anchorMin;
                anchorMax = rectTransform.anchorMax;
                offsetMin = rectTransform.offsetMin;
                offsetMax = rectTransform.offsetMax;
                pivot = rectTransform.pivot;
                sizeDelta = rectTransform.sizeDelta;
            }
        }

        private void LateUpdate()
        {
            transform.localPosition = localPosition;
            transform.localRotation = localRotation;

            if (rectTransform != null)
            {
                rectTransform.anchorMin = anchorMin;
                rectTransform.anchorMax = anchorMax;
                rectTransform.offsetMin = offsetMin;
                rectTransform.offsetMax = offsetMax;
                rectTransform.pivot = pivot;
                rectTransform.sizeDelta = sizeDelta;
            }
        }

    }
}
