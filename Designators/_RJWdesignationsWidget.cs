using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace rjw
{
	/// <summary>
	/// Handles creation of RJWdesignations button group for gui
	/// </summary>
	public class RJWdesignations : Command
	{
		//is actually a group of four buttons, but I've wanted them be small and so vanilla gizmo system is mostly bypassed for them
	
		private const float ContentPadding = 5f;
		private const float IconSize = 32f;
		private const float IconGap = 1f;

		private readonly Pawn parent;
		private Rect gizmoRect;
		/// <summary>
		/// This should keep track of last pressed pseudobutton. It is set in the pseudobutton callback. 
		/// Then, the callback to encompassed Gizmo is performed by game, and this field is used to determine what exact button was pressed
		/// The callback is called by the tynancode right after the loop which detects click on button, 
		/// so it will be called then and only then when it should be (no unrelated events). 
		/// event handling  shit is needed to apply settings to all selected pawns.
		/// </summary>
		private static SubIcon lastClicked;

		static readonly List<SubIcon> subIcons = new List<SubIcon> {
			new Comfort(),
			//new Service(),
			//new BreederHuman(),
			new BreedingHuman(),
			new BreedingAnimal(),
			new Breeder(),
			//new Milking(),
			new Hero(),
			new HelpImStupid()
		};

		public RJWdesignations(Pawn pawn)
		{
			parent = pawn;
			defaultLabel = "RJWdesignations";
			defaultDesc = "RJWdesignations";
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft, float maxWidth, GizmoRenderParms parms)
		{
			Rect rect = new Rect(topLeft.x, topLeft.y, 75f, 75f);
			gizmoRect = rect.ContractedBy(ContentPadding);
			Widgets.DrawWindowBackground(rect);

			//Log.Message("RJWgizmo");
			foreach (SubIcon icon in subIcons)
			{
				if (DrawSubIcon(icon))
				{
					lastClicked = icon;
					icon.state = icon.applied(parent);
					return new GizmoResult(GizmoState.Interacted, Event.current);
				}
			}
			return new GizmoResult(GizmoState.Clear);
		}

		//this and class mess below was supposed to be quick shortcut to not write four repeated chunks of code in GizmoOnGUI
		private bool DrawSubIcon(SubIcon icon)
		{
			if (!icon.applicable(parent)) { return false; }
			//Log.Message("sub gizmo");
			Rect iconRect = new Rect(gizmoRect.x + icon.offset.x, gizmoRect.y + icon.offset.y, IconSize, IconSize);
			TooltipHandler.TipRegion(iconRect, icon.desc(parent).Translate());
			bool applied = icon.applied(parent);
			Texture2D texture = applied ? icon.cancel : icon.texture(parent);
			GUI.DrawTexture(iconRect, texture);
			//GUI.color = Color.white;

			return Widgets.ButtonInvisible(iconRect, false);
		}

		public override void ProcessInput(Event ev)
		{
			SubIcon icon = lastClicked;
			if (icon.state)
				icon.unapply(parent);
			else
				icon.apply(parent);
		}

		[StaticConstructorOnStartup]//this  is needed for textures
		public abstract class SubIcon
		{
			public abstract Texture2D texture(Pawn pawn);
			public abstract Vector2 offset { get; }
			public abstract string desc(Pawn pawn);
			public abstract bool applicable(Pawn pawn);
			public abstract bool applied(Pawn pawn);
			public abstract void apply(Pawn pawn);
			public abstract void unapply(Pawn pawn);

			static readonly Texture2D cancellText = ContentFinder<Texture2D>.Get("UI/Commands/cancel");
			public virtual Texture2D cancel => cancellText;

			public bool state;
		}
		[StaticConstructorOnStartup]
		public class Comfort : SubIcon
		{
			//comfort raping
			static readonly Texture2D iconAccept = ContentFinder<Texture2D>.Get("UI/Commands/ComfortPrisoner_off");
			static readonly Texture2D iconRefuse = ContentFinder<Texture2D>.Get("UI/Commands/ComfortPrisoner_Refuse");
			static readonly Texture2D iconCancel = ContentFinder<Texture2D>.Get("UI/Commands/ComfortPrisoner_on");
			public override Texture2D texture(Pawn pawn) => (pawn.CanDesignateComfort() || pawn.IsDesignatedComfort()) && xxx.is_human(pawn) ? iconAccept : iconRefuse;
			public override Texture2D cancel { get; } = iconCancel;

			static readonly Vector2 posComf = new Vector2(IconGap + IconSize, 0);
			public override Vector2 offset => posComf;

			public override string desc(Pawn pawn) => pawn.CanDesignateComfort() ? "ForComfortDesc" : 
														!pawn.CanChangeDesignationPrisoner() ? "ForHeroRefuse2Desc" : 
														!pawn.CanChangeDesignationColonist() ? "ForHeroRefuse1Desc" : "ForComfortRefuseDesc";

			public override bool applicable(Pawn pawn) => RJWSettings.rape_enabled && xxx.is_human(pawn);
			public override bool applied(Pawn pawn) => pawn.IsDesignatedComfort();
			public override void apply(Pawn pawn) => pawn.ToggleComfort();
			public override void unapply(Pawn pawn) => pawn.ToggleComfort();
		}
		[StaticConstructorOnStartup]
		public class Service : SubIcon
		{
			//whoring
			static readonly Texture2D iconAccept = ContentFinder<Texture2D>.Get("UI/Commands/Service_off");
			static readonly Texture2D iconRefuse = ContentFinder<Texture2D>.Get("UI/Commands/Service_Refuse");
			static readonly Texture2D iconCancel = ContentFinder<Texture2D>.Get("UI/Commands/Service_on");
			public override Texture2D texture(Pawn pawn) => (pawn.CanDesignateService() || pawn.IsDesignatedService()) && xxx.is_human(pawn) ? iconAccept : iconRefuse;
			public override Texture2D cancel { get; } = iconCancel;

			public override Vector2 offset { get; } = new Vector2(0, 0);

			public override string desc(Pawn pawn) => pawn.CanDesignateService() ? "ForServiceDesc" :
														!pawn.CanChangeDesignationPrisoner() ? "ForHeroRefuse2Desc" : 
														!pawn.CanChangeDesignationColonist() ? "ForHeroRefuse1Desc" : "ForServiceRefuseDesc";

			public override bool applicable(Pawn pawn) => xxx.is_human(pawn);
			public override bool applied(Pawn pawn) => pawn.IsDesignatedService() && xxx.is_human(pawn);
			public override void apply(Pawn pawn) => pawn.ToggleService();
			public override void unapply(Pawn pawn) => pawn.ToggleService();
		}
		[StaticConstructorOnStartup]
		public class BreedingHuman : SubIcon
		{
			//Breed humanlike
			static readonly Texture2D iconAccept = ContentFinder<Texture2D>.Get("UI/Commands/Breeding_Pawn_off");
			static readonly Texture2D iconRefuse = ContentFinder<Texture2D>.Get("UI/Commands/Breeding_Pawn_Refuse");
			static readonly Texture2D iconCancel = ContentFinder<Texture2D>.Get("UI/Commands/Breeding_Pawn_on");
			public override Texture2D texture(Pawn pawn) => (pawn.CanDesignateBreeding() || pawn.IsDesignatedBreeding()) && xxx.is_human(pawn) ? iconAccept : iconRefuse;
			public override Texture2D cancel { get; } = iconCancel;

			public override Vector2 offset { get; } = new Vector2(0, IconSize + IconGap);

			public override string desc(Pawn pawn) => pawn.CanDesignateBreeding() && xxx.is_human(pawn) ? "ForBreedingDesc" :
														!pawn.CanChangeDesignationPrisoner() ? "ForHeroRefuse2Desc" : 
														!pawn.CanChangeDesignationColonist() ? "ForHeroRefuse1Desc" : "ForBreedingRefuseDesc";

			public override bool applicable(Pawn pawn) =>  RJWSettings.bestiality_enabled && xxx.is_human(pawn);
			public override bool applied(Pawn pawn) => pawn.IsDesignatedBreeding() && xxx.is_human(pawn);
			public override void apply(Pawn pawn) => pawn.ToggleBreeding();
			public override void unapply(Pawn pawn) => pawn.ToggleBreeding();
		}

		[StaticConstructorOnStartup]
		public class BreedingAnimal : SubIcon
		{
			//Breed animal
			static readonly Texture2D iconAccept = ContentFinder<Texture2D>.Get("UI/Commands/Breeding_Animal_off");
			static readonly Texture2D iconCancel = ContentFinder<Texture2D>.Get("UI/Commands/Breeding_Animal_on");
			public override Texture2D texture(Pawn pawn) => iconAccept;
			public override Texture2D cancel { get; } = iconCancel;

			public override Vector2 offset { get; } = new Vector2(0, IconSize + IconGap);

			public override string desc(Pawn pawn) => !pawn.CanChangeDesignationPrisoner() ? "ForHeroRefuse2Desc" : "ForBreedingDesc";

			public override bool applicable(Pawn pawn) => (pawn.CanDesignateBreeding() || pawn.IsDesignatedBreeding()) && xxx.is_animal(pawn);
			public override bool applied(Pawn pawn) => pawn.IsDesignatedBreeding() && xxx.is_animal(pawn);
			public override void apply(Pawn pawn) => pawn.ToggleBreeding();
			public override void unapply(Pawn pawn) => pawn.ToggleBreeding();
		}
		[StaticConstructorOnStartup]
		public class Breeder : SubIcon
		{
			//Breeder(fucker) animal
			static readonly Texture2D iconAccept = ContentFinder<Texture2D>.Get("UI/Commands/Breeder_Animal_off");
			static readonly Texture2D iconCancel = ContentFinder<Texture2D>.Get("UI/Commands/Breeder_Animal_on");
			public override Texture2D texture(Pawn pawn) => iconAccept;
			public override Texture2D cancel { get; } = iconCancel;

			public override Vector2 offset { get; } = new Vector2(0, 0);

			public override string desc(Pawn pawn) => !pawn.CanChangeDesignationPrisoner() ? "ForHeroRefuse2Desc" : "ForBreedingAnimalDesc";

			public override bool applicable(Pawn pawn) => pawn.CanDesignateBreedingAnimal() || pawn.IsDesignatedBreedingAnimal();
			public override bool applied(Pawn pawn) => pawn.IsDesignatedBreedingAnimal();
			public override void apply(Pawn pawn) => pawn.ToggleBreedingAnimal();
			public override void unapply(Pawn pawn) => pawn.ToggleBreedingAnimal();
		}
		[StaticConstructorOnStartup]
		public class Milking : SubIcon
		{
			static readonly Texture2D iconAccept = ContentFinder<Texture2D>.Get("UI/Commands/Milking_off");
			static readonly Texture2D iconCancel = ContentFinder<Texture2D>.Get("UI/Commands/Milking_on");
			public override Texture2D texture(Pawn pawn) => iconAccept;
			public override Texture2D cancel { get; } = iconCancel;

			public override Vector2 offset { get; } = new Vector2(IconGap + IconSize, IconSize + IconGap);

			public override string desc(Pawn pawn) => !pawn.CanChangeDesignationPrisoner() ? "ForHeroRefuse2Desc" : "ForMilkingDesc";

			public override bool applicable(Pawn pawn) => pawn.CanDesignateMilking() || pawn.IsDesignatedMilking();
			public override bool applied(Pawn pawn) => pawn.IsDesignatedMilking();
			public override void apply(Pawn pawn) => pawn.ToggleMilking();
			public override void unapply(Pawn pawn) => pawn.ToggleMilking();
		}
		[StaticConstructorOnStartup]
		public class Hero : SubIcon
		{
			static readonly Texture2D iconAccept = ContentFinder<Texture2D>.Get("UI/Commands/Hero_off");
			static readonly Texture2D iconCancel = ContentFinder<Texture2D>.Get("UI/Commands/Hero_on");
			public override Texture2D texture(Pawn pawn) => iconAccept;
			public override Texture2D cancel { get; } = iconCancel;

			public override Vector2 offset { get; } = new Vector2(IconGap + IconSize, IconSize + IconGap);

			//public override string desc(Pawn pawn) => pawn.CanDesignateHero() || (pawn.IsDesignatedHero() && pawn.IsHeroOwner()) ? "ForHeroDesc" : "Hero of " + pawn.GetPawnData().HeroOwner;
			public override string desc(Pawn pawn) => pawn.CanDesignateHero() ? "ForHeroDesc" :
														pawn.IsDesignatedHero() ? "Hero of " + pawn.GetRJWPawnData().HeroOwner : "ForHeroDesc";

			public override bool applicable(Pawn pawn) => pawn.CanDesignateHero() || pawn.IsDesignatedHero();
			public override bool applied(Pawn pawn) => pawn.IsDesignatedHero();
			public override void apply(Pawn pawn) => pawn.ToggleHero();
			public override void unapply(Pawn pawn) => pawn.ToggleHero();
		}
		[StaticConstructorOnStartup]
		public class HelpImStupid : SubIcon
		{
			// a question mark that shows when no other icons show; shows a hint to look at mod settings
			static readonly Texture2D myicon = ContentFinder<Texture2D>.Get("UI/Commands/HelpImStupid");
			public override Texture2D texture(Pawn pawn) => myicon;
			public override Texture2D cancel { get; } = myicon;

			static readonly Vector2 posComf = new Vector2(IconGap + IconSize, 0);
			public override Vector2 offset => posComf;

			public override string desc(Pawn pawn) => "GoToModSettings";

			public override bool applicable(Pawn pawn) => !(RJWSettings.rape_enabled && xxx.is_human(pawn)) && // comfort
														!(pawn.CanDesignateHero() || pawn.IsDesignatedHero()) && // hero
														//!(pawn.CanDesignateMilking() || pawn.IsDesignatedMilking()) && // milking
														!(pawn.CanDesignateBreedingAnimal() || pawn.IsDesignatedBreedingAnimal()) && // breeder
														!((pawn.CanDesignateBreeding() || pawn.IsDesignatedBreeding()) && xxx.is_animal(pawn)) && // breeding animal
														!(RJWSettings.bestiality_enabled && xxx.is_human(pawn));// && // breeding human
														//!(pawn.IsDesignatedService() && xxx.is_human(pawn)); // service

			public override bool applied(Pawn pawn) => true;
			public override void apply(Pawn pawn) { }
			public override void unapply(Pawn pawn) { }
		}
	}
}
