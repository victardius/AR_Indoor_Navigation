using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AppConstants {
    public static class Resources
    {
        public static readonly string IconHolderPrefab = "Responsive Spatial Audio for Immersive Gaming/Prefabs/UI/BagItemImage";

		public static class Audio
		{
			public static readonly string AudioMixer = "Responsive Spatial Audio for Immersive Gaming/Audio/AudioMixer";
			public static readonly string EmptySpatialEmitter = "Responsive Spatial Audio for Immersive Gaming/Audio/EmptySpatialEmitter";
            public static readonly float BumpVolume = 0.5f;

			public static class Clips
			{
				public static readonly string BEEP = "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/Beep";
				public static readonly string DEFAULT_PICKUP = "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/clink";
				public static readonly string[] BUMPS = new string[] {
                    "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/bumps/woodBumps/w1",
                    "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/bumps/woodBumps/w2",
                    "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/bumps/woodBumps/w3",
                    "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/bumps/woodBumps/w4",
                    "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/bumps/woodBumps/w5",
                    "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/bumps/woodBumps/w6",
                    "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/bumps/woodBumps/w7",
                    "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/bumps/woodBumps/w8",
                    "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/bumps/woodBumps/w9",
                    "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/bumps/woodBumps/w10",
                    "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/bumps/woodBumps/w11",
                    "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/bumps/woodBumps/w12",
                    "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/bumps/woodBumps/w13",
                    "Responsive Spatial Audio for Immersive Gaming/Audio/Clips/bumps/woodBumps/w14"

                };
			}
		}

	}
    public static class Parameters
    {
        // Default BodyScan radius
        public static readonly float DEFAULT_MAX_VISIBLE_DISTANCE = 40.0f; // metres
        public static readonly float DEFAULT_MAX_VISIBLE_DISTANCE_NEAR = 4.0f;
        public static readonly float BODY_SCAN_HALF_ANGLE = Mathf.PI / 6; //30 degrees
        public static readonly float DELTA_BODY_SCAN_RADIUS = 1.0f;
        public static readonly float DELTA_BODY_SCAN_HALF_ANGLE = Mathf.PI / 72; //2.5 degrees
        public static readonly float ITEM_IN_PICKUPABLE_RANGE_DISTANCE = 2.3f;
        public static readonly float VIRTUAL_CANE_SCAN_RADIUS = 1.0f;
        public static readonly float VIRTUAL_CANE_SCAN_ANGLE = Mathf.PI / 8; //90 degrees

    }


    public static class Tags
	{
		public static readonly string ANNOUNCER_AUDIOSOURCE = "ANNOUNCER_AUDIOSOURCE";
		public static readonly string VANTAGE_POINT_BEEPER_AUDIOSOURCE = "VANTAGE_POINT_BEEPER_AUDIOSOURCE";
		public static readonly string Player = "Player";
    }

	public enum SpaceNames
	{
		DOCK,
		BLACKSMITH,
		WORLD,
		UNSPECIFIED,
		FOOTPATH,
		DIRT
	}
}
