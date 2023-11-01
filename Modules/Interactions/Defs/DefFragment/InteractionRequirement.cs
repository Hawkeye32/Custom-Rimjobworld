using rjw.Modules.Interactions.Defs;
using rjw.Modules.Interactions.Enums;
using rjw.Modules.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Defs.DefFragment
{
	public class InteractionRequirement
	{
		//Default = false
		public bool hand = false;
		public bool tail = false;
		public bool foot = false;
		public bool mouth = false;
		public bool beak = false;
		public bool mouthORbeak = false;
		public bool oral = false;
		public bool tongue = false;

		///<summary>Default = 1</summary>
		public Nullable<int> minimumCount = 1;

		///<summary>Default = 1</summary>
		///@ed86 - nullables are good for you ! don't use default values :b
		public Nullable<float> minimumSeverity;

		//Default = empty
		public List<string> partProps = new List<string>();

		//Default = empty
		public List<GenitalTag> tags = new List<GenitalTag>();

		//Default = empty
		public List<GenitalFamily> families = new List<GenitalFamily>();

		//Default = PawnState.Healthy
		public List<PawnState> pawnStates = new List<PawnState>();

		//overrides - edging, spanking?
		//public bool canOrgasm = true; // true - orgasm satisfy need, false - ruin orgasm/ dont satisfy need
		//public bool canCum = true; // orgasm with cum effects -semen, pregnancy etc
		//public float Sensetivity = 1.0f;
	}
}
