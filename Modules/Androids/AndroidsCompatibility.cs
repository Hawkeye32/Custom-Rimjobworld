using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace rjw
{
	/// <summary>
	/// Helper class for ChJees's Androids mod.
	/// </summary>
	[StaticConstructorOnStartup]
	public static class AndroidsCompatibility
	{
		public static Type androidCompatType;
		public static readonly string typeName = "Androids.SexualizeAndroidRJW";
		private static bool foundType;

		static AndroidsCompatibility()
		{
			try
			{
				androidCompatType = Type.GetType(typeName);
				foundType = true;
				//Log.Message("Found Type: Androids.SexualizeAndroidRJW");
			}
			catch
			{
				foundType = false;
				//Log.Message("Did NOT find Type: Androids.SexualizeAndroidRJW");
			}
		}

		/*private static bool TestPredicate(DefModExtension extension)
		{
			if (extension == null)
				return false;

			Log.Message($"Predicate: {extension} : {extension.GetType()?.FullName}");
			return extension.GetType().FullName == typeName;
		}*/

		public static bool IsAndroid(ThingDef def)
		{
			if (def == null || !foundType)
			{
				return false;
			}

			return def.modExtensions != null && def.modExtensions.Any(extension => extension.GetType().FullName == typeName);
		}

		public static bool IsAndroid(Thing thing)
		{
			return IsAndroid(thing.def);
		}

		public static bool AndroidPenisFertility(Pawn pawn)
		{
			//androids only fertile with archotech parts
			BodyPartRecord Part = Genital_Helper.get_genitalsBPR(pawn);
			return (pawn.health.hediffSet.hediffs.Any((Hediff hed) =>
				(hed.Part == Part) &&
				(hed.def == Genital_Helper.archotech_penis)
				));
		}

		public static bool AndroidVaginaFertility(Pawn pawn)
		{
			//androids only fertile with archotech parts
			BodyPartRecord Part = Genital_Helper.get_genitalsBPR(pawn);
			return (pawn.health.hediffSet.hediffs.Any((Hediff hed) =>
				(hed.Part == Part) &&
				(hed.def == Genital_Helper.archotech_vagina)
				));
		}

	}
}
