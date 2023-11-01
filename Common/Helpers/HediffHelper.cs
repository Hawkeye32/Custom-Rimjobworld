using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace rjw
{
	public class HediffHelper
	{
		/// <summary>
		/// Creates hediff with custom hediff class
		/// </summary>
		/// <typeparam name="T">hediff class. Should be class defined by hediff or subclass of that class</typeparam>
		/// <param name="def">hediffDef</param>
		/// <param name="pawn">owner of newly created hediff</param>
		/// <param name="partRecord">bodypart heduff assigned to</param>
		/// <returns>newly created hediff</returns>
		public static T MakeHediff<T>(HediffDef def, Pawn pawn, BodyPartRecord partRecord = null) where T : Hediff
		{
			if (!def.hediffClass.IsAssignableFrom(typeof(T)))
			{
				throw new InvalidOperationException($"trying to create hediff with incompatible class: {typeof(T).Name} is not a {def.hediffClass.Name} or its subclass");
			}

			T hediff = (T)Activator.CreateInstance(typeof(T));
			hediff.def = def;
			hediff.pawn = pawn;
			hediff.Part = partRecord;
			hediff.loadID = Find.UniqueIDsManager.GetNextHediffID();
			hediff.PostMake();

			return hediff;
		}

		/// <summary>
		/// Fill existing hediff class with data
		/// </summary>
		/// <typeparam name="T">hediff class. Should be class defined by hediff or subclass of that class</typeparam>
		/// <param name="def">hediffDef</param>
		/// <param name="hediff">empty hediff instance</Hediff>
		/// <param name="pawn">owner of newly created hediff</param>
		/// <param name="partRecord">bodypart heduff assigned to</param>
		/// <returns>newly created hediff</returns>
		public static T FillHediff<T>(HediffDef def, T hediff, Pawn pawn, BodyPartRecord partRecord = null) where T : Hediff
		{
			if (!def.hediffClass.IsAssignableFrom(typeof(T)))
			{
				throw new InvalidOperationException($"trying to create hediff with incompatible class: {typeof(T).Name} is not a {def.hediffClass.Name} or its subclass");
			}

			hediff.def = def;
			hediff.pawn = pawn;
			hediff.Part = partRecord;
			hediff.loadID = Find.UniqueIDsManager.GetNextHediffID();
			hediff.PostMake();

			return hediff;
		}
	}
}
