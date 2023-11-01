using System.Collections.Generic;
using RimWorld;
using Verse;

namespace rjw
{
	public class Recipe_MakeFuta : rjw_CORE_EXPOSED.Recipe_AddBodyPart
	{
		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			GenderHelper.Sex before = GenderHelper.GetSex(pawn);

			base.ApplyOnPawn(pawn, part, billDoer, ingredients, bill);

			GenderHelper.Sex after = GenderHelper.GetSex(pawn);

			GenderHelper.ChangeSex(pawn, before, after);
		}
	}

	//Female to futa
	public class Recipe_MakeFutaF : Recipe_MakeFuta
	{
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn p, RecipeDef r)
		{
			var parts = p.GetGenitalsList();

			bool blocked = Genital_Helper.genitals_blocked(p) || xxx.is_slime(p); //|| xxx.is_demon(p);
			bool has_vag = Genital_Helper.has_vagina(p, parts);
			bool has_cock = Genital_Helper.has_male_bits(p, parts);

			foreach (BodyPartRecord part in base.GetPartsToApplyOn(p, r))
				if (r.appliedOnFixedBodyParts.Contains(part.def) && !blocked && (has_vag && !has_cock))
					yield return part;
		}
	}

	//Male to futa
	public class Recipe_MakeFutaM : Recipe_MakeFuta
	{
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn p, RecipeDef r)
		{
			var parts = p.GetGenitalsList();

			bool blocked = Genital_Helper.genitals_blocked(p) || xxx.is_slime(p); //|| xxx.is_demon(p);
			bool has_vag = Genital_Helper.has_vagina(p, parts);
			bool has_cock = Genital_Helper.has_male_bits(p, parts);

			foreach (BodyPartRecord part in base.GetPartsToApplyOn(p, r))
				if (r.appliedOnFixedBodyParts.Contains(part.def) && !blocked && (!has_vag && has_cock))
					yield return part;
		}
	}

	//TODO: maybe merge with above to make single recipe, but meh for now just copy-paste recipes or some filter to disable multiparts if normal part doesn't exist yet

	//add multi parts
	public class Recipe_AddMultiPart : Recipe_MakeFuta
	{
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn p, RecipeDef r)
		{
			//don't add artificial - peg, hydraulics, bionics, archo, ovi
			if (r.addsHediff.addedPartProps?.solid ?? false)
				yield break;

			//don't add if artificial parts present
			if (p.health.hediffSet.hediffs.Any((Hediff hed) =>
				(hed.Part != null) && 
				r.appliedOnFixedBodyParts.Contains(hed.Part.def) && 
				(hed.def.addedPartProps?.solid ?? false)))
				yield break;

			var parts = p.GetGenitalsList();

			//don't add if no ovi
			//if (Genital_Helper.has_ovipositorF(p, parts) || Genital_Helper.has_ovipositorM(p, parts))
			//	yield break;

			//don't add if same part type not present yet
			if (!Genital_Helper.has_vagina(p, parts) && r.defName.ToLower().Contains("vagina"))
				yield break;
			if (!Genital_Helper.has_penis_fertile(p, parts) && r.defName.ToLower().Contains("penis"))
				yield break;

			//cant install parts when part blocked, on slimes, on demons
			bool blocked = (xxx.is_slime(p) //|| xxx.is_demon(p)
				|| (Genital_Helper.genitals_blocked(p) && r.appliedOnFixedBodyParts.Contains(xxx.genitalsDef))
				|| (Genital_Helper.anus_blocked(p) && r.appliedOnFixedBodyParts.Contains(xxx.anusDef))
				|| (Genital_Helper.breasts_blocked(p) && r.appliedOnFixedBodyParts.Contains(xxx.breastsDef)));

			foreach (BodyPartRecord part in base.GetPartsToApplyOn(p, r))
				if (r.appliedOnFixedBodyParts.Contains(part.def) && !blocked)
					yield return part;
		}
	}

	public class Recipe_AddSinglePart : Recipe_MakeFuta
	{
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn p, RecipeDef r)
		{
			//cant install parts when part blocked, on slimes, or when already has one
			bool blocked = xxx.is_slime(p) || p.health.hediffSet.HasHediff(r.addsHediff);

			foreach (BodyPartRecord part in base.GetPartsToApplyOn(p, r))
				if (r.appliedOnFixedBodyParts.Contains(part.def) && !blocked)
					yield return part;
		}
	}

	public class Recipe_RemoveSinglePart : Recipe_MakeFuta
	{
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn p, RecipeDef r)
		{
			//cant install parts when part blocked, on slimes, or when already has one
			bool blocked = xxx.is_slime(p) || !p.health.hediffSet.HasHediff(r.removesHediff);

			foreach (BodyPartRecord part in base.GetPartsToApplyOn(p, r))
				if (r.appliedOnFixedBodyParts.Contains(part.def) && !blocked)
					yield return part;
		}
	}
}
