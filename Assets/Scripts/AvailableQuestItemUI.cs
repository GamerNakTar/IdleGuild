using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class AvailableQuestItemUI : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text summaryText;
    [SerializeField] private TMP_Text rewardText;
    [SerializeField] private TMP_Text durationText;
    [SerializeField] private TMP_Text actionText;

    private QuestDefinition questDefinition;
    private Action<QuestDefinition> clickHandler;

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

    public void Initialize(QuestDefinition quest, Action<QuestDefinition> onClick)
    {
        ResolveReferences();

        questDefinition = quest;
        clickHandler = onClick;

        if (titleText != null)
        {
            titleText.text = quest.Title;
        }

        if (summaryText != null)
        {
            summaryText.text = quest.Description;
        }

        if (rewardText != null)
        {
            rewardText.text = quest.RewardLabel;
        }

        if (durationText != null)
        {
            durationText.text = quest.DurationLabel;
        }
    }

    private void ResolveReferences()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }

        var background = GetComponent<Image>();
        if (background == null)
        {
            background = gameObject.AddComponent<Image>();
            background.color = new Color(0.117f, 0.125f, 0.145f, 0.96f);
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
            layoutElement.minHeight = 132f;
            layoutElement.preferredHeight = 132f;
            layoutElement.flexibleWidth = 1f;
        }

        if (titleText == null)
        {
            titleText = CreateLabel(
                "TitleText",
                new Vector2(0f, 0.58f),
                new Vector2(0.72f, 1f),
                new Vector2(24f, 0f),
                new Vector2(0f, -16f),
                26f,
                FontStyles.Bold,
                TextAlignmentOptions.Left,
                new Color(0.933f, 0.918f, 0.867f, 1f));
        }

        if (summaryText == null)
        {
            summaryText = CreateLabel(
                "SummaryText",
                new Vector2(0f, 0.24f),
                new Vector2(0.72f, 0.62f),
                new Vector2(24f, 0f),
                new Vector2(0f, 0f),
                20f,
                FontStyles.Normal,
                TextAlignmentOptions.Left,
                new Color(0.722f, 0.753f, 0.804f, 1f));
        }

        if (rewardText == null)
        {
            rewardText = CreateLabel(
                "RewardText",
                new Vector2(0f, 0f),
                new Vector2(0.72f, 0.27f),
                new Vector2(24f, 12f),
                new Vector2(0f, 0f),
                19f,
                FontStyles.Normal,
                TextAlignmentOptions.Left,
                new Color(0.929f, 0.698f, 0.275f, 1f));
        }

        if (durationText == null)
        {
            durationText = CreateLabel(
                "DurationText",
                new Vector2(0.72f, 0.52f),
                new Vector2(1f, 0.92f),
                new Vector2(12f, 0f),
                new Vector2(-24f, 0f),
                22f,
                FontStyles.Bold,
                TextAlignmentOptions.Right,
                new Color(0.494f, 0.804f, 0.973f, 1f));
        }

        if (actionText == null)
        {
            actionText = CreateLabel(
                "ActionText",
                new Vector2(0.72f, 0.08f),
                new Vector2(1f, 0.42f),
                new Vector2(12f, 0f),
                new Vector2(-24f, 0f),
                22f,
                FontStyles.Bold,
                TextAlignmentOptions.Right,
                new Color(0.933f, 0.918f, 0.867f, 1f));
        }

        actionText.text = "상세";
    }

    private void HandleClick()
    {
        if (questDefinition != null)
        {
            clickHandler?.Invoke(questDefinition);
        }
    }

    private TMP_Text CreateLabel(
        string labelName,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 offsetMin,
        Vector2 offsetMax,
        float fontSize,
        FontStyles fontStyle,
        TextAlignmentOptions alignment,
        Color color)
    {
        var labelObject = new GameObject(labelName, typeof(RectTransform));
        labelObject.layer = gameObject.layer;
        labelObject.transform.SetParent(transform, false);

        var labelRect = labelObject.GetComponent<RectTransform>();
        labelRect.anchorMin = anchorMin;
        labelRect.anchorMax = anchorMax;
        labelRect.offsetMin = offsetMin;
        labelRect.offsetMax = offsetMax;

        var label = labelObject.AddComponent<TextMeshProUGUI>();
        label.fontSize = fontSize;
        label.fontStyle = fontStyle;
        label.alignment = alignment;
        label.color = color;
        label.raycastTarget = false;
        label.textWrappingMode = TextWrappingModes.Normal;
        return label;
    }
}
