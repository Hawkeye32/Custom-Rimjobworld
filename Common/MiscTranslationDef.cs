using System;
using Verse;

namespace rjw
{
	public sealed class MiscTranslationDef : Def
	{
		public Type targetClass;
		public string stringA = null;
		public string stringB = null;
		public string stringC = null;

		private void Assert(bool check, string errorMessage)
		{
			if (!check)
			{
				ModLog.Error($"Invalid data in MiscTranslationDef {defName}: {errorMessage}");
			}
		}

		public override void PostLoad()
		{
			Assert(targetClass != null, "targetClass field must be set");
		}

		public override void ResolveReferences()
		{
			base.ResolveReferences();
		}
	}
}