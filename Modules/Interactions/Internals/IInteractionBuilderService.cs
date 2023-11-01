using rjw.Modules.Interactions.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Internals
{
	public interface IInteractionBuilderService
	{
		/// <summary>
		/// Fill the output interaction with all the good stuff !
		/// </summary>
		void Build(InteractionContext context);
	}
}
