using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actions
{
	public class Base { }
	public interface KeyBoardAction {
		KeyCode key { get; }
	}

	namespace User
	{
		// Triggered whenever the user presses the interact key
		public class InteractKeyPressed : Base
		{
			public static readonly KeyCode key = KeyCode.JoystickButton3;
			public static readonly string keystr = "Y";

			public InteractKeyPressed() { }
		}

		public class OrientationResetPressed : Base
		{
			public static readonly KeyCode key = KeyCode.BackQuote;
			public static readonly string keystr = "`";
		}

		public class MenuKeyPressed : Base
		{
			public static readonly KeyCode key = KeyCode.Tab;
			public static readonly string keystr = "Tab";
		}

		public class SlowMotionKeyPressed : Base
		{
			public static readonly KeyCode key = KeyCode.Z;
			public static readonly string keystr = "Z";
		}

        public class IncBodyScanAngleKeyPressed : Base
        {
            public static readonly KeyCode key = KeyCode.G;
            public static readonly string keystr = "G";
        }

        public class DecBodyScanAngleKeyPressed : Base
        {
            public static readonly KeyCode key = KeyCode.F;
            public static readonly string keystr = "F";
        }
        public class IncRadiusKeyPressed : Base
        {
            public static readonly KeyCode key = KeyCode.H;
            public static readonly string keystr = "H";
        }

        public class DecRadiusKeyPressed : Base
        {
            public static readonly KeyCode key = KeyCode.J;
            public static readonly string keystr = "J";
        }

        public class SelectKeyPressed : Base
		{
			public static readonly KeyCode key = KeyCode.JoystickButton9;
			public static readonly string keystr = "JoystickButton9";
		}

        public class CaneKeyPressed : Base
        {
            public static readonly KeyCode key = KeyCode.JoystickButton1;
            public static readonly string keystr = "JoystickButton1";
        }

		public class SelectedItemDetailKeyPressed : Base
		{
			public static readonly KeyCode key = KeyCode.Alpha3;
			public static readonly string keystr = "3";
		}

		public class ReachKeyPressed : Base
		{
			public static readonly KeyCode key = KeyCode.R;
			public static readonly string keystr = "R";
		}

		public class NorthHornPressed : Base
		{
			public static readonly KeyCode key = KeyCode.Q;
			public static readonly string keystr = "Q";
		}

		public class UserEnteredSpace : Base
		{
			public readonly AppConstants.SpaceNames buildingName;

			public UserEnteredSpace(AppConstants.SpaceNames buildingName)
			{
				this.buildingName = buildingName;
			}
		}

		public class OnBump : Base
		{
			public ControllerColliderHit hit;

			public OnBump(ControllerColliderHit hit)
			{
				this.hit = hit;
			}
		}

		public class SignificantMovement : Base { }

	

        public class BlindMode : Base
        {
            public static readonly KeyCode key = KeyCode.B;
            public static readonly string keystr = "B";
        }
    }

    namespace Inventory
    {
        public class HighlightOffItem : Base
        {
            public PickupAble pickup;
            public HighlightOffItem(PickupAble pk)
            {
                this.pickup = pk;
            }
        }

        public class HighlightItem : Base
        {
            public PickupAble pickup;
            public HighlightItem(PickupAble pk)
            {
                this.pickup = pk;
            }
        }

        public class ItemWentOutOfRange : Base
        {
            public PickupAble pickup;
            public ItemWentOutOfRange(PickupAble pk)
            {
                this.pickup = pk;
            }
        }

        public class ItemCameIntoRange : Base
        {
            public PickupAble pickup;
            public ItemCameIntoRange(PickupAble pk)
            {
                this.pickup = pk;
            }
        }

        public class BagIsOverweight : Base { }

        public class BagItemUpdated : Base
        {
            public PickupAble pickup;
            public BagItemUpdated(PickupAble pk)
            {
                this.pickup = pk;
            }
        }
    }

    namespace Accessibility
    {
        public class PushBasicDescription : Base
        {
            public static readonly KeyCode key = KeyCode.JoystickButton8;
            public static readonly string keystr = "JoystickButton8";

			public readonly float radius;

			public PushBasicDescription(float radius = -1)
			{
				if (radius > 0)
				{
					this.radius = radius; 
				}
				else
				{
					this.radius = AppConstants.Parameters.DEFAULT_MAX_VISIBLE_DISTANCE;
				}
			}
		}

		public class PushBasicDescriptionWithRadius : PushBasicDescription
		{
			new public static readonly KeyCode key = KeyCode.Alpha1;
			new public static readonly string keystr = "1";

			public PushBasicDescriptionWithRadius() : base(AppConstants.Parameters.DEFAULT_MAX_VISIBLE_DISTANCE_NEAR) { }
		}

		public class UserEnteredVantagePoint : Base
		{
			public VantagePoint vantagePoint;

			public UserEnteredVantagePoint(VantagePoint vantagePoint)
			{
				this.vantagePoint = vantagePoint;
			}
		}

		public class VantagePointLocked : Base { }

		public class UserExitedVantagePoint : Base { }
        public class BodyScanAngleChanged : Base { }

    }

    namespace AI {
		public class EnemyAttentionTrigger
		{
			public readonly GameObject enemy;
			public EnemyAttentionTrigger(GameObject enemy)
			{
				this.enemy = enemy;
			}
		}
	}
}
