using System.Collections.Generic;

public class AppState
{
    // Inventory
    public PickupAble activePickup;
    public float weight;
    public HashSet<PickupAble> currentlyReachable;

    // Maps a pickupable object's metadata's hash to its quantity
    public Dictionary<string, uint> bag;
    public bool isMenuDisplayed;
    public bool blindMode;


    // Item that is being currently read out
    public AccessibilitySoundGenerator currentlySelected;



    // Current viewdio
    public VantagePoint currentVantagePoint;

    //body scan radius
    public float bodyScanRadius;
    public float bodyScanHalfAngle;

    public static AppState initial
    {
        get
        {
            return new AppState
            {
                activePickup = null,
                weight = 0,
                currentlyReachable = new HashSet<PickupAble>(),

                bag = new Dictionary<string, uint>(),
                isMenuDisplayed = false,
                blindMode = false,


                bodyScanRadius = AppConstants.Parameters.DEFAULT_MAX_VISIBLE_DISTANCE,
                bodyScanHalfAngle = AppConstants.Parameters.BODY_SCAN_HALF_ANGLE,
                currentlySelected = null,
                currentVantagePoint = null,

            };
        }
    }
}
