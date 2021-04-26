using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DwarfManager : MonoBehaviour
{
    [SerializeField] TextMeshPro livingDwarfCountText;
    [SerializeField] TextMeshPro deadDwarfCountText;

    readonly List<BaseDwarf> dwarves = new List<BaseDwarf>();

    int livingDwarfCount = 0;
    int deadDwarfCount = 0;

    public int DwarfCount { get => dwarves.Count; }
    public bool OnBreak { get; private set; } = false;

    public event Action NoDwarvesLeft;
    public event Action AllDwarvesSafe;

    private void Update()
    {
        livingDwarfCountText.text = livingDwarfCount.ToString();
        deadDwarfCountText.text = deadDwarfCount.ToString();

        if (OnBreak && AreAllDwarvesDrinking() && !GameController.GameEnded)
            AllDwarvesSafe?.Invoke();
    }

    public void RegisterDwarf(BaseDwarf dwarf)
    {
        if (dwarves.Contains(dwarf))
            return;

        dwarves.Add(dwarf);

        livingDwarfCount = dwarves.Count;
    }

    public void UnregisterDwarf(BaseDwarf dwarf)
    {
        if (!dwarves.Contains(dwarf))
            return;

        dwarves.Remove(dwarf);

        livingDwarfCount = dwarves.Count;
        deadDwarfCount++;

        if (dwarves.Count <= 0 && !GameController.GameEnded)
            NoDwarvesLeft?.Invoke();
    }

    public void CallBreak()
    {
        OnBreak = true;

        foreach (BaseDwarf dwarf in dwarves)
            dwarf.ForceJob(JobSelector.GetBreakJob());
    }

    public float GetLowestDwarfHeight()
    {
        if (dwarves.Count <= 0)
            return Mathf.NegativeInfinity;

        float minHeight = Mathf.Infinity;
        foreach (BaseDwarf dwarf in dwarves)
            if (dwarf.transform.position.y < minHeight)
                minHeight = dwarf.transform.position.y;

        return minHeight;
    }

    public bool AreAllDwarvesDrinking()
    {
        foreach (BaseDwarf dwarf in dwarves)
            if (!IsDwarfDrinking(dwarf))
                return false;

        return true;
    }

    bool IsDwarfDrinking(BaseDwarf dwarf)
    {
        if (dwarves.Count <= 0)
            return false;

        return dwarf.CurrentJob != null
            && dwarf.CurrentJob.GetJobType() == JobType.GET_BOOZE
            && ((GetBoozeJob)dwarf.CurrentJob).IsDrinking;
    }
}
