using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace rjw
{
	public class PawnRelationWorker_Child_Beast : PawnRelationWorker_Child
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (!xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isChildOf(other, me);
		}
	}

	public class PawnRelationWorker_Sibling_Beast : PawnRelationWorker_Sibling
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (!xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isSiblingOf(other, me);
		}
	}
	public class PawnRelationWorker_HalfSibling_Beast : PawnRelationWorker_HalfSibling
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (!xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isHalfSiblingOf(other, me);
		}
	}
	public class PawnRelationWorker_Grandparent_Beast : PawnRelationWorker_Grandparent
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (!xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isGrandchildOf(me, other);
		}
	}
	public class PawnRelationWorker_Grandchild_Beast : PawnRelationWorker_Grandchild
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (!xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isGrandparentOf(me, other);  //if other isGrandchildOf of me, me is their grandparent
		}
	}
	public class PawnRelationWorker_NephewOrNiece_Beast : PawnRelationWorker_NephewOrNiece
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (!xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isUncleOrAuntOf(me, other);
		}
	}
	public class PawnRelationWorker_UncleOrAunt_Beast : PawnRelationWorker_UncleOrAunt
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (!xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isNephewOrNieceOf(me, other);
		}
	}

	public class PawnRelationWorker_Cousin_Beast : PawnRelationWorker_Cousin
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (!xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isCousinOf(me, other);
		}
	}

	public class PawnRelationWorker_GreatGrandparent_Beast : PawnRelationWorker_GreatGrandparent
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (!xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isGreatGrandChildOf(me, other);
		}
	}

	public class PawnRelationWorker_GreatGrandchild_Beast : PawnRelationWorker_GreatGrandchild
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (!xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isGreatGrandparentOf(me, other);
		}
	}

	public class PawnRelationWorker_GranduncleOrGrandaunt_Beast : PawnRelationWorker_GranduncleOrGrandaunt
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (!xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isGrandnephewOrGrandnieceOf(me, other);
		}
	}

	public class PawnRelationWorker_GrandnephewOrGrandniece_Beast : PawnRelationWorker_GrandnephewOrGrandniece
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (!xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isGreatUncleOrAuntOf(me, other);
		}
	}

	public class PawnRelationWorker_CousinOnceRemoved_Beast : PawnRelationWorker_CousinOnceRemoved
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (!xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isCousinOnceRemovedOf(me, other);
		}
	}

	public class PawnRelationWorker_SecondCousin_Beast : PawnRelationWorker_SecondCousin
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (!xxx.is_animal(other) || me == other)
			{
				return false;
			}

			return RelationChecker.isSecondCousinOf(me, other);
		}
	}
	/*
	public class PawnRelationWorker_Kin_Beast : PawnRelationWorker_Kin
	{
		public override bool InRelation(Pawn me, Pawn other)
		{
			if (!xxx.is_animal(other) || me == other)
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
	*/
}
