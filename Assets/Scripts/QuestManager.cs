using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

public sealed class QuestManager : MonoBehaviour
{
    private const string RuntimeRootName = "QuestManagerUI";

    [Header("Prefabs")]
    [SerializeField] private QuestUIController acceptedQuestPrefab;
    [SerializeField] private AvailableQuestItemUI availableQuestPrefab;
    [SerializeField] private QuestDetailPopupUI questDetailPopupPrefab;
    [SerializeField] private TabButtonUI tabButtonPrefab;

    private readonly List<QuestDefinition> questDefinitions = new List<QuestDefinition>();
    private readonly HashSet<string> acceptedQuestIds = new HashSet<string>();
    private readonly List<TabButtonUI> mainTabButtons = new List<TabButtonUI>();
    private readonly List<TabButtonUI> guildTabButtons = new List<TabButtonUI>();

    private RectTransform acceptedQuestContent;
    private RectTransform availableQuestContent;
    private QuestDetailPopupUI questDetailPopup;
    private TMP_Text acceptedEmptyText;
    private TMP_Text availableEmptyText;

    private GameObject acceptedQuestScreen;
    private GameObject guildScreen;
    private GameObject settingsScreen;
    private GameObject guildQuestListPanel;
    private GameObject guildInfoPanel;

    private enum MainTab
    {
        AcceptedQuests,
        Guild,
        Settings
    }

    private enum GuildTab
    {
        QuestList,
        GuildInfo
    }

    private void Awake()
    {
        EnsureEventSystemInput();
        InitializeQuestDefinitions();
        BuildLayout();
        RefreshAvailableQuestList();
        ShowMainTab(MainTab.AcceptedQuests);
    }

    private void InitializeQuestDefinitions()
    {
        questDefinitions.Clear();
        questDefinitions.Add(new QuestDefinition(
            "training_patrol",
            "훈련장 순찰",
            "훈련장 주변을 순찰하고 신입 길드원의 장비 상태를 점검합니다.",
            5f,
            120,
            35));
        questDefinitions.Add(new QuestDefinition(
            "herb_delivery",
            "약초 배달",
            "마을 약재상에게 회복 약초 꾸러미를 전달합니다.",
            5f,
            90,
            30));
        questDefinitions.Add(new QuestDefinition(
            "cellar_cleanup",
            "지하 창고 정리",
            "길드 지하 창고에 쌓인 낡은 보급품을 분류합니다.",
            5f,
            150,
            45));
        questDefinitions.Add(new QuestDefinition(
            "notice_board",
            "의뢰 게시판 정비",
            "오래된 의뢰서를 정리하고 새 의뢰서를 게시합니다.",
            5f,
            80,
            25));
        questDefinitions.Add(new QuestDefinition(
            "escort_scout",
            "초보 정찰 동행",
            "초보 모험가의 숲 입구 정찰을 지원합니다.",
            5f,
            180,
            55));
    }

    private void BuildLayout()
    {
        RemoveExistingRuntimeRoot();

        var runtimeRoot = CreateRect(RuntimeRootName, transform);
        Stretch(runtimeRoot, 0f, 0f, 0f, 0f);
        AddImage(runtimeRoot.gameObject, new Color(0.063f, 0.071f, 0.086f, 1f), false);

        BuildHeader(runtimeRoot);

        var mainTabArea = CreateRect("MainTabArea", runtimeRoot);
        Stretch(mainTabArea, 32f, 152f, 32f, 156f);
        AddImage(mainTabArea.gameObject, new Color(0.102f, 0.11f, 0.129f, 0.98f), false);

        acceptedQuestScreen = BuildAcceptedQuestScreen(mainTabArea);
        guildScreen = BuildGuildScreen(mainTabArea);
        settingsScreen = BuildSimpleScreen(mainTabArea, "설정", "설정 항목 준비 중");

        BuildBottomTabBar(runtimeRoot);

        if (questDetailPopupPrefab != null)
        {
            questDetailPopup = Instantiate(questDetailPopupPrefab, runtimeRoot);
            questDetailPopup.name = "QuestDetailPopup";
            Stretch(questDetailPopup.GetComponent<RectTransform>(), 0f, 0f, 0f, 0f);
            questDetailPopup.Hide();
        }
    }

    private void BuildHeader(Transform parent)
    {
        var header = CreateRect("Header", parent);
        AnchorTop(header, 132f, 32f, 32f, 16f);

        var title = CreateText(
            "Title",
            header,
            "Idle Guild",
            44f,
            FontStyles.Bold,
            TextAlignmentOptions.Left,
            new Color(0.933f, 0.918f, 0.867f, 1f));
        Stretch(title.rectTransform, 0f, 0f, 0f, 0f);

        var status = CreateText(
            "Status",
            header,
            "길드 대기실",
            24f,
            FontStyles.Normal,
            TextAlignmentOptions.Right,
            new Color(0.659f, 0.702f, 0.765f, 1f));
        Stretch(status.rectTransform, 0f, 0f, 0f, 0f);
    }

    private GameObject BuildAcceptedQuestScreen(Transform parent)
    {
        var screen = CreateRect("AcceptedQuestScreen", parent);
        Stretch(screen, 0f, 0f, 0f, 0f);

        var header = CreateText(
            "ScreenTitle",
            screen,
            "수락된 퀘스트",
            34f,
            FontStyles.Bold,
            TextAlignmentOptions.Left,
            new Color(0.933f, 0.918f, 0.867f, 1f));
        AnchorTop(header.rectTransform, 86f, 28f, 28f, 8f);

        acceptedQuestContent = CreateScrollContent("AcceptedQuestScroll", screen, 24f, 24f, 24f, 104f);
        acceptedEmptyText = CreateEmptyText(screen, "수락된 퀘스트가 없습니다.");

        return screen.gameObject;
    }

    private GameObject BuildGuildScreen(Transform parent)
    {
        var screen = CreateRect("GuildScreen", parent);
        Stretch(screen, 0f, 0f, 0f, 0f);

        var tabBar = CreateRect("GuildInnerTabBar", screen);
        AnchorTop(tabBar, 92f, 20f, 20f, 16f);
        AddHorizontalLayout(tabBar, 12, 0, 0, 0, 0);

        guildTabButtons.Clear();
        guildTabButtons.Add(CreateTabButton("퀘스트 목록", tabBar, () => ShowGuildTab(GuildTab.QuestList)));
        guildTabButtons.Add(CreateTabButton("길드 정보", tabBar, () => ShowGuildTab(GuildTab.GuildInfo)));

        var contentArea = CreateRect("GuildContentArea", screen);
        Stretch(contentArea, 0f, 0f, 0f, 116f);

        guildQuestListPanel = BuildGuildQuestListPanel(contentArea);
        guildInfoPanel = BuildGuildInfoPanel(contentArea);
        ShowGuildTab(GuildTab.QuestList);

        return screen.gameObject;
    }

    private GameObject BuildGuildQuestListPanel(Transform parent)
    {
        var panel = CreateRect("AvailableQuestPanel", parent);
        Stretch(panel, 0f, 0f, 0f, 0f);

        availableQuestContent = CreateScrollContent("AvailableQuestScroll", panel, 24f, 24f, 24f, 16f);
        availableEmptyText = CreateEmptyText(panel, "수락 가능한 퀘스트가 없습니다.");

        return panel.gameObject;
    }

    private GameObject BuildGuildInfoPanel(Transform parent)
    {
        var panel = CreateRect("GuildInfoPanel", parent);
        Stretch(panel, 24f, 24f, 24f, 16f);

        var title = CreateText(
            "GuildInfoTitle",
            panel,
            "길드 정보",
            34f,
            FontStyles.Bold,
            TextAlignmentOptions.Left,
            new Color(0.933f, 0.918f, 0.867f, 1f));
        AnchorTop(title.rectTransform, 80f, 0f, 0f, 0f);

        var body = CreateText(
            "GuildInfoBody",
            panel,
            "길드 정보 항목 준비 중",
            26f,
            FontStyles.Normal,
            TextAlignmentOptions.Center,
            new Color(0.659f, 0.702f, 0.765f, 1f));
        Stretch(body.rectTransform, 0f, 0f, 0f, 88f);

        return panel.gameObject;
    }

    private GameObject BuildSimpleScreen(Transform parent, string titleText, string bodyText)
    {
        var screen = CreateRect($"{titleText}Screen", parent);
        Stretch(screen, 24f, 24f, 24f, 24f);

        var title = CreateText(
            "ScreenTitle",
            screen,
            titleText,
            34f,
            FontStyles.Bold,
            TextAlignmentOptions.Left,
            new Color(0.933f, 0.918f, 0.867f, 1f));
        AnchorTop(title.rectTransform, 80f, 0f, 0f, 0f);

        var body = CreateText(
            "ScreenBody",
            screen,
            bodyText,
            26f,
            FontStyles.Normal,
            TextAlignmentOptions.Center,
            new Color(0.659f, 0.702f, 0.765f, 1f));
        Stretch(body.rectTransform, 0f, 0f, 0f, 88f);

        return screen.gameObject;
    }

    private void BuildBottomTabBar(Transform parent)
    {
        var bottomBar = CreateRect("BottomTabBar", parent);
        AnchorBottom(bottomBar, 136f, 0f, 0f, 0f);
        AddImage(bottomBar.gameObject, new Color(0.075f, 0.082f, 0.098f, 1f), false);
        AddHorizontalLayout(bottomBar, 14, 20, 20, 16, 16);

        mainTabButtons.Clear();
        mainTabButtons.Add(CreateTabButton("퀘스트", bottomBar, () => ShowMainTab(MainTab.AcceptedQuests)));
        mainTabButtons.Add(CreateTabButton("길드", bottomBar, () => ShowMainTab(MainTab.Guild)));
        mainTabButtons.Add(CreateTabButton("설정", bottomBar, () => ShowMainTab(MainTab.Settings)));
    }

    private void RefreshAvailableQuestList()
    {
        if (availableQuestContent == null)
        {
            return;
        }

        ClearChildren(availableQuestContent);

        var visibleCount = 0;
        foreach (var questDefinition in questDefinitions)
        {
            if (acceptedQuestIds.Contains(questDefinition.Id))
            {
                continue;
            }

            visibleCount++;
            var item = Instantiate(availableQuestPrefab, availableQuestContent);
            item.name = $"AvailableQuest_{questDefinition.Id}";
            item.Initialize(questDefinition, OpenQuestDetail);
        }

        if (availableEmptyText != null)
        {
            availableEmptyText.gameObject.SetActive(visibleCount == 0);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(availableQuestContent);
    }

    private void OpenQuestDetail(QuestDefinition questDefinition)
    {
        if (questDetailPopup == null)
        {
            AcceptQuest(questDefinition);
            return;
        }

        questDetailPopup.Show(questDefinition, AcceptQuest);
    }

    private void AcceptQuest(QuestDefinition questDefinition)
    {
        if (!acceptedQuestIds.Add(questDefinition.Id))
        {
            return;
        }

        if (acceptedQuestPrefab != null && acceptedQuestContent != null)
        {
            var questUi = Instantiate(acceptedQuestPrefab, acceptedQuestContent);
            questUi.name = $"AcceptedQuest_{questDefinition.Id}";
            questUi.Initialize(questDefinition);
            LayoutRebuilder.ForceRebuildLayoutImmediate(acceptedQuestContent);
        }

        if (acceptedEmptyText != null)
        {
            acceptedEmptyText.gameObject.SetActive(false);
        }

        RefreshAvailableQuestList();
    }

    private void ShowMainTab(MainTab tab)
    {
        SetActive(acceptedQuestScreen, tab == MainTab.AcceptedQuests);
        SetActive(guildScreen, tab == MainTab.Guild);
        SetActive(settingsScreen, tab == MainTab.Settings);

        for (var i = 0; i < mainTabButtons.Count; i++)
        {
            mainTabButtons[i].SetSelected((int)tab == i);
        }
    }

    private void ShowGuildTab(GuildTab tab)
    {
        SetActive(guildQuestListPanel, tab == GuildTab.QuestList);
        SetActive(guildInfoPanel, tab == GuildTab.GuildInfo);

        for (var i = 0; i < guildTabButtons.Count; i++)
        {
            guildTabButtons[i].SetSelected((int)tab == i);
        }
    }

    private TabButtonUI CreateTabButton(string label, Transform parent, Action onClick)
    {
        if (tabButtonPrefab == null)
        {
            Debug.LogError("TabButton prefab is not assigned.", this);
            return null;
        }

        var tabButton = Instantiate(tabButtonPrefab, parent);
        tabButton.name = $"{label}TabButton";
        tabButton.Initialize(label, onClick);
        return tabButton;
    }

    private RectTransform CreateScrollContent(
        string name,
        Transform parent,
        float left,
        float bottom,
        float right,
        float top)
    {
        var scrollRoot = CreateRect(name, parent);
        Stretch(scrollRoot, left, bottom, right, top);

        var scrollRect = scrollRoot.gameObject.AddComponent<ScrollRect>();
        scrollRect.horizontal = false;
        scrollRect.vertical = true;
        scrollRect.scrollSensitivity = 42f;
        scrollRect.movementType = ScrollRect.MovementType.Clamped;

        var viewport = CreateRect("Viewport", scrollRoot);
        Stretch(viewport, 0f, 0f, 0f, 0f);
        AddImage(viewport.gameObject, new Color(1f, 1f, 1f, 0.01f), true);
        var mask = viewport.gameObject.AddComponent<Mask>();
        mask.showMaskGraphic = false;

        var content = CreateRect("Content", viewport);
        content.anchorMin = new Vector2(0f, 1f);
        content.anchorMax = new Vector2(1f, 1f);
        content.pivot = new Vector2(0.5f, 1f);
        content.anchoredPosition = Vector2.zero;
        content.sizeDelta = Vector2.zero;

        var layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(0, 0, 0, 0);
        layout.spacing = 14f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        var fitter = content.gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.viewport = viewport;
        scrollRect.content = content;

        return content;
    }

    private TMP_Text CreateEmptyText(Transform parent, string text)
    {
        var emptyText = CreateText(
            "EmptyText",
            parent,
            text,
            28f,
            FontStyles.Normal,
            TextAlignmentOptions.Center,
            new Color(0.659f, 0.702f, 0.765f, 1f));
        Stretch(emptyText.rectTransform, 24f, 24f, 24f, 24f);
        return emptyText;
    }

    private static RectTransform CreateRect(string name, Transform parent)
    {
        var gameObject = new GameObject(name, typeof(RectTransform));
        gameObject.layer = parent.gameObject.layer;

        var rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.SetParent(parent, false);
        return rectTransform;
    }

    private static TMP_Text CreateText(
        string name,
        Transform parent,
        string text,
        float fontSize,
        FontStyles fontStyle,
        TextAlignmentOptions alignment,
        Color color)
    {
        var rectTransform = CreateRect(name, parent);
        var textComponent = rectTransform.gameObject.AddComponent<TextMeshProUGUI>();
        textComponent.text = text;
        textComponent.fontSize = fontSize;
        textComponent.fontStyle = fontStyle;
        textComponent.alignment = alignment;
        textComponent.color = color;
        textComponent.raycastTarget = false;
        textComponent.textWrappingMode = TextWrappingModes.Normal;
        return textComponent;
    }

    private static Image AddImage(GameObject gameObject, Color color, bool raycastTarget)
    {
        var image = gameObject.AddComponent<Image>();
        image.color = color;
        image.raycastTarget = raycastTarget;
        image.type = Image.Type.Simple;
        return image;
    }

    private static void AddHorizontalLayout(
        RectTransform rectTransform,
        int spacing,
        int left,
        int right,
        int top,
        int bottom)
    {
        var layout = rectTransform.gameObject.AddComponent<HorizontalLayoutGroup>();
        layout.padding = new RectOffset(left, right, top, bottom);
        layout.spacing = spacing;
        layout.childAlignment = TextAnchor.MiddleCenter;
        layout.childControlWidth = true;
        layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = true;
    }

    private static void Stretch(RectTransform rectTransform, float left, float bottom, float right, float top)
    {
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = new Vector2(left, bottom);
        rectTransform.offsetMax = new Vector2(-right, -top);
    }

    private static void AnchorTop(RectTransform rectTransform, float height, float left, float right, float top)
    {
        rectTransform.anchorMin = new Vector2(0f, 1f);
        rectTransform.anchorMax = new Vector2(1f, 1f);
        rectTransform.pivot = new Vector2(0.5f, 1f);
        rectTransform.offsetMin = new Vector2(left, -top - height);
        rectTransform.offsetMax = new Vector2(-right, -top);
    }

    private static void AnchorBottom(RectTransform rectTransform, float height, float left, float right, float bottom)
    {
        rectTransform.anchorMin = new Vector2(0f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 0f);
        rectTransform.pivot = new Vector2(0.5f, 0f);
        rectTransform.offsetMin = new Vector2(left, bottom);
        rectTransform.offsetMax = new Vector2(-right, bottom + height);
    }

    private static void ClearChildren(Transform parent)
    {
        for (var i = parent.childCount - 1; i >= 0; i--)
        {
            var child = parent.GetChild(i);
            child.SetParent(null);
            Destroy(child.gameObject);
        }
    }

    private void RemoveExistingRuntimeRoot()
    {
        var existingRoot = transform.Find(RuntimeRootName);
        if (existingRoot != null)
        {
            existingRoot.SetParent(null);
            Destroy(existingRoot.gameObject);
        }
    }

    private static void SetActive(GameObject gameObject, bool isActive)
    {
        if (gameObject != null)
        {
            gameObject.SetActive(isActive);
        }
    }

    private static void EnsureEventSystemInput()
    {
        var eventSystem = EventSystem.current;
        if (eventSystem == null)
        {
            var eventSystemObject = new GameObject("EventSystem", typeof(EventSystem));
            eventSystem = eventSystemObject.GetComponent<EventSystem>();
        }

#if ENABLE_INPUT_SYSTEM
        var inputModule = eventSystem.GetComponent<InputSystemUIInputModule>();
        if (inputModule == null)
        {
            inputModule = eventSystem.gameObject.AddComponent<InputSystemUIInputModule>();
        }

        if (inputModule.actionsAsset == null)
        {
            inputModule.AssignDefaultActions();
        }
#else
        if (eventSystem.GetComponent<StandaloneInputModule>() == null)
        {
            eventSystem.gameObject.AddComponent<StandaloneInputModule>();
        }
#endif
    }
}
