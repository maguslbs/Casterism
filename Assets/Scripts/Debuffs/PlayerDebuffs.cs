using System.Collections.Generic;
using UnityEngine;

public class PlayerDebuffs : MonoBehaviour
{
    [System.Serializable]
    private class DebuffVFX
    {
        public DebuffType type;
        public GameObject prefab;
        public Vector3 offset;
    }

    [SerializeField] private Transform vfxParent;
    [SerializeField] private List<DebuffVFX> vfxList = new List<DebuffVFX>();

    private readonly Dictionary<DebuffType, Debuff> activeDebuffs = new Dictionary<DebuffType, Debuff>();
    private readonly Dictionary<DebuffType, GameObject> activeVFX = new Dictionary<DebuffType, GameObject>();

    private Health health;

    public bool IsStunned => HasDebuff(DebuffType.Stun);

    private void Awake()
    {
        health = GetComponent<Health>();
    }

    private void OnEnable()
    {
        TurnEvents.OnPlayerTurnStart += HandlePlayerTurnStart;
        TurnEvents.OnPlayerTurnEnd += HandlePlayerTurnEnd;
    }

    private void OnDisable()
    {
        TurnEvents.OnPlayerTurnStart -= HandlePlayerTurnStart;
        TurnEvents.OnPlayerTurnEnd -= HandlePlayerTurnEnd;
    }

    public bool HasDebuff(DebuffType type)
    {
        return activeDebuffs.ContainsKey(type);
    }

    public void Apply(Debuff debuff)
    {
        if (debuff == null) return;

        if (activeDebuffs.TryGetValue(debuff.Type, out Debuff existing))
        {
            existing.Refresh(debuff);
            return;
        }

        activeDebuffs.Add(debuff.Type, debuff);
        debuff.OnApply(health);
        SpawnVFX(debuff.Type);
    }

    public void Remove(DebuffType type)
    {
        if (!activeDebuffs.TryGetValue(type, out Debuff debuff)) return;

        activeDebuffs.Remove(type);
        debuff.OnRemove(health);
        DestroyVFX(type);
    }

    public void CleanseAll()
    {
        List<DebuffType> types = new List<DebuffType>(activeDebuffs.Keys);

        foreach (DebuffType type in types)
        {
            Remove(type);
        }
    }

    private void HandlePlayerTurnStart()
    {
        foreach (Debuff debuff in new List<Debuff>(activeDebuffs.Values))
        {
            debuff.OnPlayerTurnStart(health);
        }

        RemoveExpired();
    }

    private void HandlePlayerTurnEnd()
    {
        foreach (Debuff debuff in new List<Debuff>(activeDebuffs.Values))
        {
            debuff.OnPlayerTurnEnd(health);
        }

        RemoveExpired();
    }

    private void RemoveExpired()
    {
        List<DebuffType> expired = new List<DebuffType>();

        foreach (Debuff debuff in activeDebuffs.Values)
        {
            if (debuff.IsExpired) expired.Add(debuff.Type);
        }

        foreach (DebuffType type in expired)
        {
            Remove(type);
        }
    }

    private void SpawnVFX(DebuffType type)
    {
        DebuffVFX entry = vfxList.Find(v => v.type == type);
        if (entry == null || entry.prefab == null || vfxParent == null) return;

        GameObject vfx = Instantiate(entry.prefab, vfxParent);
        vfx.transform.localPosition = entry.offset;
        activeVFX[type] = vfx;
    }

    private void DestroyVFX(DebuffType type)
    {
        if (!activeVFX.TryGetValue(type, out GameObject vfx)) return;

        if (vfx != null) Destroy(vfx);
        activeVFX.Remove(type);
    }
}