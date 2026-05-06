using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class QuestDetailPopupUI : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text durationText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private Button acceptButton;
    [SerializeField] private Button closeButton;

    private QuestDefinition questDefinition;
    private Action<QuestDefinition> acceptHandler;

    private void Awake()
    {
        ResolveReferences();

        if (acceptButton != null)
        {
            acceptButton.onClick.AddListener(HandleAccept);
        }

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(Hide);
        }
    }

    private void OnDestroy()
    {
        if (acceptButton != null)
        {
            acceptButton.onClick.RemoveListener(HandleAccept);
        }

        if (closeButton != null)
        {
            closeButton.onClick.RemoveListener(Hide);
        }
    }

    public void Show(QuestDefinition quest, Action<QuestDefinition> onAccept)
    {
        ResolveReferences();

        questDefinition = quest;
        acceptHandler = onAccept;

        if (titleText != null)
        {
            titleText.text = quest.Title;
        }

        if (descriptionText != null)
        {
            descriptionText.text = quest.Description;
        }

        if (durationText != null)
        {
            durationText.text = $"소요 시간: {quest.DurationLabel}";
        }

        if (rewardText != null)
        {
            rewardText.text = $"보상: {quest.RewardLabel}";
        }

        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    private void HandleAccept()
    {
        if (questDefinition == null)
        {
            return;
        }

        acceptHandler?.Invoke(questDefinition);
        Hide();
    }

    private void ResolveReferences()
    {
        if (titleText != null &&
            descriptionText != null &&
            durationText != null &&
            rewardText != null &&
            acceptButton != null &&
            closeButton != null)
        {
            return;
        }

        var overlay = GetComponent<Image>();
        if (overlay == null)
        {
            overlay = gameObject.AddComponent<Image>();
            overlay.color = new Color(0f, 0f, 0f, 0.58f);
            overlay.raycastTarget = true;
        }

        var panel = CreateRect("Panel", transform);
        panel.anchorMin = new Vector2(0.5f, 0.5f);
        panel.anchorMax = new Vector2(0.5f, 0.5f);
        panel.pivot = new Vector2(0.5f, 0.5f);
        panel.anchoredPosition = Vector2.zero;
        panel.sizeDelta = new Vector2(820f, 720f);

        var panelImage = panel.gameObject.AddComponent<Image>();
        panelImage.color = new Color(0.117f, 0.125f, 0.145f, 1f);
        panelImage.type = Image.Type.Simple;

        titleText = CreateLabel(
            "TitleText",
            panel,
            new Vector2(0f, 1f),
            new Vector2(1f, 1f),
            new Vector2(40f, -116f),
            new Vector2(-40f, -32f),
            38f,
            FontStyles.Bold,
            TextAlignmentOptions.Left,
            new Color(0.933f, 0.918f, 0.867f, 1f));
        descriptionText = CreateLabel(
            "DescriptionText",
            panel,
            new Vector2(0f, 0.44f),
            new Vector2(1f, 0.84f),
            new Vector2(40f, 0f),
            new Vector2(-40f, 0f),
            27f,
            FontStyles.Normal,
            TextAlignmentOptions.TopLeft,
            new Color(0.831f, 0.851f, 0.886f, 1f));
        durationText = CreateLabel(
            "DurationText",
            panel,
            new Vector2(0f, 0.31f),
            new Vector2(1f, 0.42f),
            new Vector2(40f, 0f),
            new Vector2(-40f, 0f),
            25f,
            FontStyles.Bold,
            TextAlignmentOptions.Left,
            new Color(0.494f, 0.804f, 0.973f, 1f));
        rewardText = CreateLabel(
            "RewardText",
            panel,
            new Vector2(0f, 0.2f),
            new Vector2(1f, 0.31f),
            new Vector2(40f, 0f),
            new Vector2(-40f, 0f),
            25f,
            FontStyles.Bold,
            TextAlignmentOptions.Left,
            new Color(0.929f, 0.698f, 0.275f, 1f));
        acceptButton = CreateButton(
            "AcceptButton",
            panel,
            "수락",
            new Vector2(0.5f, 0f),
            new Vector2(1f, 0f),
            new Vector2(12f, 36f),
            new Vector2(-40f, 116f),
            new Color(0.929f, 0.698f, 0.275f, 1f),
            new Color(0.122f, 0.078f, 0.024f, 1f));
        closeButton = CreateButton(
            "CloseButton",
            panel,
            "닫기",
            new Vector2(0f, 0f),
            new Vector2(0.5f, 0f),
            new Vector2(40f, 36f),
            new Vector2(-12f, 116f),
            new Color(0.18f, 0.196f, 0.231f, 1f),
            new Color(0.933f, 0.918f, 0.867f, 1f));
    }

    private static RectTransform CreateRect(string objectName, Transform parent)
    {
        var gameObject = new GameObject(objectName, typeof(RectTransform));
        gameObject.layer = parent.gameObject.layer;
        gameObject.transform.SetParent(parent, false);
        return gameObject.GetComponent<RectTransform>();
    }

    private static TMP_Text CreateLabel(
        string objectName,
        Transform parent,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 offsetMin,
        Vector2 offsetMax,
        float fontSize,
        FontStyles fontStyle,
        TextAlignmentOptions alignment,
        Color color)
    {
        var rectTransform = CreateRect(objectName, parent);
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = offsetMin;
        rectTransform.offsetMax = offsetMax;

        var label = rectTransform.gameObject.AddComponent<TextMeshProUGUI>();
        label.fontSize = fontSize;
        label.fontStyle = fontStyle;
        label.alignment = alignment;
        label.color = color;
        label.raycastTarget = false;
        label.textWrappingMode = TextWrappingModes.Normal;
        return label;
    }

    private static Button CreateButton(
        string objectName,
        Transform parent,
        string labelText,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 offsetMin,
        Vector2 offsetMax,
        Color backgroundColor,
        Color textColor)
    {
        var rectTransform = CreateRect(objectName, parent);
        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
        rectTransform.offsetMin = offsetMin;
        rectTransform.offsetMax = offsetMax;

        var image = rectTransform.gameObject.AddComponent<Image>();
        image.color = backgroundColor;
        image.type = Image.Type.Simple;

        var button = rectTransform.gameObject.AddComponent<Button>();
        button.targetGraphic = image;

        var label = CreateLabel(
            "Label",
            rectTransform,
            Vector2.zero,
            Vector2.one,
            Vector2.zero,
            Vector2.zero,
            27f,
            FontStyles.Bold,
            TextAlignmentOptions.Center,
            textColor);
        label.text = labelText;

        return button;
    }
}
