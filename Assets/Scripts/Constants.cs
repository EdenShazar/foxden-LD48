using UnityEngine;

public static class Constants
{
    public static readonly int nonworkingDwarvesLayer = SortingLayer.NameToID("Nonworking dwarves");
    public static readonly int workingDwarvesLayer = SortingLayer.NameToID("Working dwarves");

    public static readonly int wagonLayer = LayerMask.NameToLayer("Wagon");

    public static readonly float horizontalInteractionDistance = 0.05f;
    public static readonly float fallingSpeedThreshold = 0.3f;
    
    public static readonly Color clearColor = new Color(0f, 0f, 0f, 0f);
    public static readonly Color maxSobrietyWarningColor = new Color(0.945f, 0.341f, 0.341f);

    public static readonly int digCost = 1;
    public static readonly int stopSignCost = 1;
    public static readonly int ropeCost = 1;
}
