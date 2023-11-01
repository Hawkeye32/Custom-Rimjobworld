using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace rjw
{
	/// <summary>
	/// Checks for relations, workaround for relation checks that rely on other relation checks, since the vanilla inRelation checks have been prefixed.
	/// 
	/// Return true if first pawn is specified relation of the second pawn
	/// 
	///  If "me" isRelationOf "other" return true
	/// </summary>
	/// 
	public static class RelationChecker
	{
		public static bool isChildOf(Pawn me, Pawn other)
		{
			if (me == null || me == other)
			{
				return false;
			}
			if (me.GetMother() != other)
			{
				return me.GetFather() == other;
			}
			//else "other" is mother
			return true;
		}

		public static bool isSiblingOf(Pawn me, Pawn other)
		{
			if (me == null || me == other)
			{
				return false;
			}
			if (me.GetMother() != null && me.GetFather() != null && me.GetMother() == other.GetMother() && me.GetFather() == other.GetFather())
			{
				return true;
			}
			return false;
		}

		public static bool isHalfSiblingOf(Pawn me, Pawn other)
		{
			if (me == null || me == other)
			{
				return false;
			}
			if (isSiblingOf(me, other))
			{
				return false;
			}
			return ((me.GetMother() != null && me.GetMother() == other.GetMother()) || (me.GetFather() != null && me.GetFather() == other.GetFather()));
		}

		public static bool isAnySiblingOf(Pawn me, Pawn other)
		{
			if (me == null || me == other)
			{
				return false;
			}
			if ((me.GetMother() != null && me.GetMother() == other.GetMother()) || (me.GetFather() != null && me.GetFather() == other.GetFather()))
			{
				return true;
			}
			return false;
		}

		public static bool isGrandchildOf(Pawn me, Pawn other)
		{
			if (me == null || me == other)
			{
				return false;
			}
			if ((me.GetMother() != null && isChildOf(me.GetMother(), other)) || (me.GetFather() != null && isChildOf(me.GetFather(), other)))
			{
				return true;
			}
			return false;
		}

		public static bool isGrandparentOf(Pawn me, Pawn other)
		{
			if (me == null || me == other)
			{
				return false;
			}
			if (isGrandchildOf(other, me))
			{
				return true;
			}
			return false;
		}

		public static bool isNephewOrNieceOf(Pawn me, Pawn other)
		{
			if (me == null || me == other)
			{
				return false;
			}
			if ((me.GetMother() != null && (isAnySiblingOf(other, me.GetMother()))) || (me.GetFather() != null && (isAnySiblingOf(other, me.GetFather()))))
			{
				return true;
			}
			return false;
		}

		public static bool isUncleOrAuntOf(Pawn me, Pawn other)
		{
			if (me == null || me == other)
			{
				return false;
			}
			if (isNephewOrNieceOf(other, me))
			{
				return true;
			}
			return false;
		}

		public static bool isCousinOf(Pawn me, Pawn other)
		{
			if (me == null || me == other)
			{
				return false;
			}

			if ((other.GetMother() != null && isNephewOrNieceOf(me, other.GetMother())) || (other.GetFather() != null && isNephewOrNieceOf(me, other.GetFather())))
			{
				return true;
			}
			return false;
		}

		public static bool isGreatGrandparentOf(Pawn me, Pawn other)
		{
			if (me == null || me == other)
			{
				return false;
			}

			return isGreatGrandChildOf(other, me);
		}

		public static bool isGreatGrandChildOf(Pawn me, Pawn other)
		{
			if (me == null || me == other)
			{
				return false;
			}

			if ((me.GetMother() != null && isGrandchildOf(me.GetMother(), other)) || (me.GetFather() != null && isGrandchildOf(me.GetFather(), other)))
			{
				return true;
			}
			return false;
		}

		public static bool isGreatUncleOrAuntOf(Pawn me, Pawn other)
		{
			if (me == null || me == other)
			{
				return false;
			}

			return isGrandnephewOrGrandnieceOf(other, me);
		}

		public static bool isGrandnephewOrGrandnieceOf(Pawn me, Pawn other)
		{
			if (me == null || me == other)
			{
				return false;
			}

			if ((me.GetMother() != null && isUncleOrAuntOf(other, me.GetMother())) || (me.GetFather() != null && isUncleOrAuntOf(other, me.GetFather())))
			{
				return true;
			}
			return false;
		}

		public static bool isCousinOnceRemovedOf(Pawn me, Pawn other)
		{
			if (me == null || me == other)
			{
				return false;
			}
			if ((other.GetMother() != null && isCousinOf(me, other.GetMother())) || (other.GetFather() != null && isCousinOf(me, other.GetFather())))
			{
				return true;
			}
			if ((other.GetMother() != null && isGrandnephewOrGrandnieceOf(me, other.GetMother())) || (other.GetFather() != null && isGrandnephewOrGrandnieceOf(me, other.GetFather())))
			{
				return true;
			}
			return false;
		}

		public static bool isSecondCousinOf(Pawn me, Pawn other)
		{
			if (me == null || me == other)
			{
				return false;
			}
			PawnRelationWorker worker = PawnRelationDefOf.GranduncleOrGrandaunt.Worker;
			Pawn mother = other.GetMother();
			if (mother != null && ((mother.GetMother() != null && isGrandnephewOrGrandnieceOf(me, mother.GetMother())) || (mother.GetFather() != null && isGrandnephewOrGrandnieceOf(me, mother.GetFather()))))
			{
				return true;
			}
			Pawn father = other.GetFather();
			if (father != null && ((father.GetMother() != null && isGrandnephewOrGrandnieceOf(me, father.GetMother())) || (father.GetFather() != null && isGrandnephewOrGrandnieceOf(me, father.GetFather()))))
			{
				return true;
			}
			return false;
		}
	}
}
