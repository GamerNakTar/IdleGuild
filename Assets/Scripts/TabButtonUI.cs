using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class TabButtonUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private Image background;
    [SerializeField] private TMP_Text labelText;
    [SerializeField] private Color selectedBackground = new Color(0.929f, 0.698f, 0.275f, 1f);
    [SerializeField] private Color normalBackground = new Color(0.145f, 0.153f, 0.176f, 1f);
    [SerializeField] private Color selectedText = new Color(0.122f, 0.078f, 0.024f, 1f);
    [SerializeField] private Color normalText = new Color(0.933f, 0.918f, 0.867f, 1f);

    private Action clickAction;

    private void Awake()
    {
        ResolveReferences();

        if (button != null)
        {
            button.onClick.AddListener(HandleClick);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(HandleClick);
        }
    }

    public void Initialize(string label, Action onClick)
    {
        ResolveReferences();

        clickAction = onClick;

        if (labelText != null)
        {
            labelText.text = label;
        }
    }

    public void SetSelected(bool selected)
    {
        ResolveReferences();

        if (background != null)
        {
            background.color = selected ? selectedBackground : normalBackground;
        }

        if (labelText != null)
        {
            labelText.color = selected ? selectedText : normalText;
        }
    }

    private void ResolveReferences()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        if (background == null)
        {
            background = GetComponent<Image>();
        }

        if (background == null)
        {
            background = gameObject.AddComponent<Image>();
            background.type = Image.Type.Simple;
        }

        if (button == null)
        {
            button = gameObject.AddComponent<Button>();
            button.targetGraphic = background;
        }

        if (button.targetGraphic == null)
        {
            button.targetGraphic = background;
        }

        var layoutElement = GetComponent<LayoutElement>();
        if (layoutElement == null)
        {
            layoutElement = gameObject.AddComponent<LayoutElement>();
            layoutElement.minHeight = 80f;
            layoutElement.preferredHeight = 92f;
            layoutElement.flexibleWidth = 1f;
        }

        if (labelText == null)
        {
            labelText = GetComponentInChildren<TMP_Text>(true);
        }

        if (labelText == null)
        {
            var labelObject = new GameObject("Label", typeof(RectTransform));
            labelObject.layer = gameObject.layer;
            labelObject.transform.SetParent(transform, false);

            var labelRect = labelObject.GetComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;

            labelText = labelObject.AddComponent<TextMeshProUGUI>();
            labelText.fontSize = 26f;
            labelText.fontStyle = FontStyles.Bold;
            labelText.alignment = TextAlignmentOptions.Center;
            labelText.raycastTarget = false;
        }
    }

    private void HandleClick()
    {
        clickAction?.Invoke();
    }
}
