using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using HarmonyLib;

namespace rjw
{
	public static class CodeExtensions
	{
		/// <summary>
		/// <para>Breaks apart a list of instructions at some specific point, identified by a
		/// predicate function.  The first instruction that the predicate matches will be
		/// included in the `before` list, while all remaining instructions will be
		/// in the `after` list.</para>
		/// <para>If no instructions matched the predicate, all instructions will be in
		/// the `before` list and the `after` list will be empty.</para>
		/// <para>This is useful for breaking apart the instructions into more manageable
		/// chunks to better identify the target of your patch.</para>
		/// </summary>
		/// <param name="instructions">This list of instructions.</param>
		/// <param name="predicate">The predicate to identify the break point.</param>
		/// <returns>A tuple, the first being the instructions up to and including the
		/// instruction that matched the predicate and the second being everything
		/// afterward.</returns>
		public static (List<CodeInstruction> before, List<CodeInstruction> after) SplitAfter(
			this List<CodeInstruction> instructions,
			Predicate<CodeInstruction> predicate)
		{
			if (instructions.FindIndex(predicate) is var idx and not -1)
			{
				var before = instructions.Take(idx + 1).ToList();
				var after = instructions.Skip(before.Count).ToList();
				return (before, after);
			}

			return (instructions, new());
		}

		/// <summary>
		/// <para>Checks the next instructions in this list of instructions to make sure
		/// they all match some expectation, one predicate per instruction.  If the
		/// predicates all match, it produces two new lists, one with the matched
		/// instructions and the other with those matched instructions removed.</para>
		/// <para>If any predicate fails to match, the `cut` list will be empty and
		/// the `remainder` list will be the same instance as `instructions`.</para>
		/// <para>Use this to validate that the code actually is the code you expect
		/// and isolate it from other instructions so you can tweak it safely.</para>
		/// </summary>
		/// <param name="instructions">This list of instructions.</param>
		/// <param name="predicates">One or more predicates to test each instruction.</param>
		/// <returns>A tuple, the first being the list of cut instructions and the second
		/// being the remaining instructions after the cut.</returns>
		public static (List<CodeInstruction> cut, List<CodeInstruction> remainder) CutMatchingSequence(
			this List<CodeInstruction> instructions,
			params Predicate<CodeInstruction>[] predicates)
		{
			if (predicates.Length == 0)
				return (new(), instructions);
			if (instructions.Count < predicates.Length)
				return (new(), instructions);
			if (instructions.Zip(predicates, (il, pred) => pred(il)).Contains(false))
				return (new(), instructions);

			var cut = instructions.Take(predicates.Length).ToList();
			var remainder = instructions.Skip(predicates.Length).ToList();
			return (cut, remainder);
		}

		/// <summary>
		/// <para>Determines if this instruction is some form of `stloc` that matches the
		/// given index in the locals table.</para>
		/// <para>Why is this not a part of Harmony's API?</para>
		/// </summary>
		/// <param name="il">This instruction.</param>
		/// <param name="index">The locals table index to check for.</param>
		/// <returns>Whether this is a `stloc` targeting the given index.</returns>
		public static bool IsStlocOf(this CodeInstruction il, int index) => index switch
		{
			0 when il.opcode == OpCodes.Stloc_0 => true,
			1 when il.opcode == OpCodes.Stloc_1 => true,
			2 when il.opcode == OpCodes.Stloc_2 => true,
			3 when il.opcode == OpCodes.Stloc_3 => true,
			// Most common thing we'll see...
			_ when il.IsStloc() && il.operand is LocalBuilder lb => lb.LocalIndex == index,
			// ...but these are also technically possible with `stloc_s` and `stloc`.
			_ when il.IsStloc() && il.operand is byte bIndex => bIndex == Convert.ToByte(index),
			_ when il.IsStloc() && il.operand is ushort uIndex => uIndex == Convert.ToUInt16(index),
			_ => false
		};

		/// <summary>
		/// Copies the labels from this instruction to its replacement.  If the replacement
		/// instruction also has labels, it ensures that duplicate labels are discarded.
		/// </summary>
		/// <param name="il">This instruction.</param>
		/// <param name="newIl">The instruction that will replace it.</param>
		/// <returns>The replacement instruction.</returns>
		public static CodeInstruction Replace(this CodeInstruction il, CodeInstruction newIl)
		{
			var copyLabels = newIl.labels.ToList();
			newIl.labels.Clear();
			newIl.labels.AddRange(copyLabels.Concat(il.labels).Distinct());
			return newIl;
		}
	}
}