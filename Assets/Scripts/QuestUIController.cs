using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed class QuestUIController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private TMP_Text descriptionText;
    [SerializeField] private TMP_Text startButtonLabel;
    [SerializeField] private RectTransform progressFillRect;
    [SerializeField, Min(0.1f)] private float questDuration = 5f;

    private Coroutine progressRoutine;
    private bool isCompleted;

    private void Awake()
    {
        ResolveReferences();
        ResetProgress();
        SetButtonLabel("시작");

        if (startButton != null)
        {
            startButton.onClick.AddListener(StartQuest);
        }
    }

    private void OnDestroy()
    {
        if (startButton != null)
        {
            startButton.onClick.RemoveListener(StartQuest);
        }
    }

    public void Initialize(QuestDefinition questDefinition)
    {
        ResolveReferences();

        if (descriptionText != null)
        {
            descriptionText.text =
                $"{questDefinition.Title}\n{questDefinition.Description}\n보상: {questDefinition.RewardLabel}";
        }

        questDuration = questDefinition.DurationSeconds;
        isCompleted = false;
        ResetProgress();
        SetButtonLabel("시작");

        if (startButton != null)
        {
            startButton.interactable = true;
        }
    }

    private void StartQuest()
    {
        if (isCompleted)
        {
            return;
        }

        if (progressRoutine != null)
        {
            StopCoroutine(progressRoutine);
        }

        progressRoutine = StartCoroutine(RunProgress());
    }

    private IEnumerator RunProgress()
    {
        if (startButton != null)
        {
            startButton.interactable = false;
        }

        SetButtonLabel("진행중");

        var elapsed = 0f;
        SetProgress(0f);

        while (elapsed < questDuration)
        {
            elapsed += Time.deltaTime;
            SetProgress(elapsed / questDuration);
            yield return null;
        }

        SetProgress(1f);
        isCompleted = true;
        SetButtonLabel("완료");
        progressRoutine = null;
    }

    private void ResetProgress()
    {
        SetProgress(0f);
    }

    private void SetProgress(float value)
    {
        if (progressFillRect == null)
        {
            return;
        }

        var normalizedValue = Mathf.Clamp01(value);
        var anchorMax = progressFillRect.anchorMax;
        anchorMax.x = normalizedValue;
        progressFillRect.anchorMax = anchorMax;
        progressFillRect.gameObject.SetActive(normalizedValue > 0f);
    }

    private void ResolveReferences()
    {
        if (descriptionText == null)
        {
            var descriptionTransform = transform.Find("DescriptionText");
            descriptionText = descriptionTransform != null
                ? descriptionTransform.GetComponent<TMP_Text>()
                : GetComponentInChildren<TMP_Text>(true);
        }

        if (startButton == null)
        {
            startButton = GetComponentInChildren<Button>(true);
        }

        if (startButtonLabel == null && startButton != null)
        {
            startButtonLabel = startButton.GetComponentInChildren<TMP_Text>(true);
        }
    }

    private void SetButtonLabel(string label)
    {
        if (startButtonLabel != null)
        {
            startButtonLabel.text = label;
        }
    }
}
