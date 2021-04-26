using UnityEngine;

public static class Constants
{
    public static readonly int nonworkingDwarvesLayer = SortingLayer.NameToID("Nonworking dwarves");
    public static readonly int workingDwarvesLayer = SortingLayer.NameToID("Working dwarves");

    public static readonly int wagonLayer = LayerMask.NameToLayer("Wagon");

    public static readonly float horizontalInteractionDistance = 0.05f;
    public static readonly float fallingSpeedThreshold = 0.3f;
}
