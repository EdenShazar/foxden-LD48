using System;
using System.Collections.Generic;
using UnityEngine;

public class DwarfManager : MonoBehaviour
{
    readonly List<BaseDwarf> dwarves = new List<BaseDwarf>();

    public int DwarfCount { get => dwarves.Count; }
    public bool OnBreak { get; private set; } = false;

    public event Action NoDwarvesLeft;
    public event Action AllDwarvesSafe;

    private void Update()
    {
        if (OnBreak && AreAllDwarvesDrinking())
            AllDwarvesSafe?.Invoke();
    }

    public void RegisterDwarf(BaseDwarf dwarf)
    {
        if (dwarves.Contains(dwarf))
            return;

        dwarves.Add(dwarf);
    }

    public void UnregisterDwarf(BaseDwarf dwarf)
    {
        if (!dwarves.Contains(dwarf))
            return;

        dwarves.Remove(dwarf);

        if (dwarves.Count <= 0)
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