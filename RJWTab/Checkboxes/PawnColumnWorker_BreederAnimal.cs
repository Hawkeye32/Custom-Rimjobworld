using RimWorld;
using UnityEngine;
using Verse;

namespace rjw.MainTab.Checkbox
{
	[StaticConstructorOnStartup]
	public class PawnColumnWorker_BreederAnimal : PawnColumnWorker_Checkbox
	{
		public static readonly Texture2D CheckboxOnTex = ContentFinder<Texture2D>.Get("UI/Commands/Breeding_Pawn_on");
		public static readonly Texture2D CheckboxOffTex = ContentFinder<Texture2D>.Get("UI/Commands/Breeding_Pawn_off");
		public static readonly Texture2D CheckboxDisabledTex = ContentFinder<Texture2D>.Get("UI/Commands/Breeding_Pawn_Refuse");
		protected override bool HasCheckbox(Pawn pawn)
		{
			return pawn.CanDesignateBreedingAnimal();
		}
		protected bool GetDisabled(Pawn pawn)
		{
			return !pawn.CanDesignateBreedingAnimal();
		}

		protected override bool GetValue(Pawn pawn)
		{
			return pawn.IsDesignatedBreedingAnimal() && xxx.is_animal(pawn);
		}

		protected override void SetValue(Pawn pawn, bool value, PawnTable table)
		{
			if (value == this.GetValue(pawn)) return;
			pawn.ToggleBreedingAnimal();
		}
	}
}