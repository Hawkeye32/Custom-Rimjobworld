using rjw.Modules.Interactions.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace rjw.Modules.Interactions
{
	public interface ILewdInteractionValidatorService
	{
		bool IsValid(InteractionDef interaction, Pawn dominant, Pawn submissive);
	}
}
