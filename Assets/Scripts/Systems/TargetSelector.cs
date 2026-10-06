using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class TargetSelector : Singleton<TargetSelector>
{
    [SerializeField] private GameObject targetIndicator;
    [SerializeField] private Vector3 indicatorOffset = new Vector3(0f, 2f, 0f);

    public Enemy CurrentTarget { get; private set; }

    private void Update()
    {
        if (CurrentTarget != null && !CurrentTarget.IsAlive())
        {
            ClearTarget();
        }

        if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
        {
            TrySelectTargetUnderMouse();
        }

        UpdateIndicator();
    }

    private void TrySelectTargetUnderMouse()
    {
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

        ContactFilter2D filter = new ContactFilter2D();
        filter.useTriggers = true;

        List<Collider2D> hits = new List<Collider2D>();
        Physics2D.OverlapPoint(worldPoint, filter, hits);

        Debug.Log($"[Target] klik kanan di {worldPoint}, kena {hits.Count} collider");   // LOG 1

        foreach (Collider2D hit in hits)
        {
            Debug.Log($"[Target] kena: {hit.name}", hit);                                // LOG 2

            Enemy enemy = hit.GetComponentInParent<Enemy>();

            if (enemy != null && enemy.IsAlive())
            {
                CurrentTarget = enemy;
                return;
            }
        }
    }

    public void ClearTarget()
    {
        CurrentTarget = null;
    }

    private void UpdateIndicator()
    {
        if (targetIndicator == null) return;

        bool hasTarget = CurrentTarget != null;
        targetIndicator.SetActive(hasTarget);

        if (hasTarget)
        {
            targetIndicator.transform.position = CurrentTarget.GetIndicatorPosition(indicatorOffset);
        }
    }
}