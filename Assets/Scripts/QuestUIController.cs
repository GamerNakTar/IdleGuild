using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public sealed class QuestUIController : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private RectTransform progressFillRect;
    [SerializeField, Min(0.1f)] private float questDuration = 5f;

    private Coroutine progressRoutine;

    private void Awake()
    {
        ResetProgress();

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

    private void StartQuest()
    {
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

        var elapsed = 0f;
        SetProgress(0f);

        while (elapsed < questDuration)
        {
            elapsed += Time.deltaTime;
            SetProgress(elapsed / questDuration);
            yield return null;
        }

        SetProgress(1f);

        if (startButton != null)
        {
            startButton.interactable = true;
        }

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
}
