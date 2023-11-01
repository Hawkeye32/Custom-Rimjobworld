using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace rjw
{
	public class PawnRelationWorker_Child_Humanlike : PawnRelationWorker_Child
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (xxx.is_animal(other) || me == other)
			{
				return false;
			}

			if (base.InRelation(me, other) == true)
			{
				return true;
			}
			return false;
		}
	}

	public class PawnRelationWorker_Sibling_Humanlike : PawnRelationWorker_Sibling
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (xxx.is_animal(other) || me == other)
			{
				return false;
			}

			if (base.InRelation(me, other) == true)
			{
				return true;
			}
			return false;
		}
	}
	public class PawnRelationWorker_HalfSibling_Humanlike : PawnRelationWorker_HalfSibling
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isHalfSiblingOf(other, me);
		}
	}
	public class PawnRelationWorker_Grandparent_Humanlike : PawnRelationWorker_Grandparent
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isGrandchildOf(me, other);
		}
	}
	public class PawnRelationWorker_Grandchild_Humanlike : PawnRelationWorker_Grandchild
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isGrandparentOf(me, other);
		}
	}
	public class PawnRelationWorker_NephewOrNiece_Humanlike : PawnRelationWorker_NephewOrNiece
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isUncleOrAuntOf(me, other);
		}
	}
	public class PawnRelationWorker_UncleOrAunt_Humanlike : PawnRelationWorker_UncleOrAunt
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isNephewOrNieceOf(me, other);
		}
	}

	public class PawnRelationWorker_Cousin_Humanlike : PawnRelationWorker_Cousin
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isCousinOf(me, other);
		}
	}

	public class PawnRelationWorker_GreatGrandparent_Humanlike : PawnRelationWorker_GreatGrandparent
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isGreatGrandChildOf(me, other);
		}
	}

	public class PawnRelationWorker_GreatGrandchild_Humanlike : PawnRelationWorker_GreatGrandchild
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isGreatGrandparentOf(me, other);
		}
	}

	public class PawnRelationWorker_GranduncleOrGrandaunt_Humanlike : PawnRelationWorker_GranduncleOrGrandaunt
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isGrandnephewOrGrandnieceOf(me, other);
		}
	}

	public class PawnRelationWorker_GrandnephewOrGrandniece_Humanlike : PawnRelationWorker_GrandnephewOrGrandniece
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isGreatUncleOrAuntOf(me, other);
		}
	}

	public class PawnRelationWorker_CousinOnceRemoved_Humanlike : PawnRelationWorker_CousinOnceRemoved
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isCousinOnceRemovedOf(me, other);
		}
	}

	public class PawnRelationWorker_SecondCousin_Humanlike : PawnRelationWorker_SecondCousin
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isSecondCousinOf(me, other);
		}
	}

	public class PawnRelationWorker_Kin_Humanlike : PawnRelationWorker_Kin
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (xxx.is_animal(other) || me == other)
			{
				return false;
			}

			if (base.InRelation(me, other) == true)
			{
				return true;
			}
			return false;
		}
	}
}
