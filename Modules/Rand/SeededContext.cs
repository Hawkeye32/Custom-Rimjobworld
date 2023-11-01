using Verse;

namespace rjw.Modules.Rand
{
	/// <summary>
	/// <para>When used with a `using` statement, creates a seeded RNG context.</para>
	/// <para>Once the block is exited, the original RNG state is restored.</para>
	/// <example>
	/// <para>How to create a seeded context:</para>
	/// <code>
	/// // Pull in the `Seeded` type at the top of the C# file.
	/// using Seeded = rjw.Modules.Rand.Seeded;
	/// 
	/// string RandomYepNah(Pawn pawn)
	/// {
	///   // Put it in a `using` with a new seed or a good seed source (like this
	///   // pawn here), and then do your random stuff inside.  There's no need to
	///   // bind it to a variable.
	///   using (Seeded.With(pawn))
	///   {
	///     var someChance = Verse.Rand.Chance(0.5f);
	///     return someChance ? "Yep!" : "Nah...";
	///   }
	/// }
	/// </code>
	/// <para>When the block is exited, the ref-struct is disposed, which pops the
	/// RNG state for you.  It even does this if there's an exception thrown in the
	/// using block, so the RNG state won't be borked.</para>
	/// <para>It should be safe to layer multiple RNG contexts.  They should pop
	/// in the correct order.</para>
	/// </example>
	/// </summary>
	public readonly ref struct Seeded
	{
		public readonly int seed;

		private Seeded(int replacementSeed)
		{
			seed = replacementSeed;
			Verse.Rand.PushState(replacementSeed);
		}

		public void Dispose()
		{
			Verse.Rand.PopState();
		}

		/// <summary>
		/// <para>When used with a `using` statement, creates an isolated RNG context.</para>
		/// <para>Once the block is exited, the original RNG state is restored.</para>
		/// <para>This version will use the given seed.</para>
		/// </summary>
		/// <param name="replacementSeed">The seed to use.</param>
		/// <returns>An RNG context.</returns>
		public static Seeded With(int replacementSeed) =>
			new(replacementSeed);
		
		/// <summary>
		/// <para>When used with a `using` statement, creates an isolated RNG context.</para>
		/// <para>Once the block is exited, the original RNG state is restored.</para>
		/// <para>This version will use a seed based on the thing's ID and so the
		/// randomness will be reproducible for that thing each time the block is
		/// entered.  As long as you always do the same operations, it will always
		/// produce the same RNG.</para>
		/// </summary>
		/// <param name="thing">The thing to use as a seed source.</param>
		/// <returns>An RNG context.</returns>
		public static Seeded With(Thing thing) =>
			new(thing.HashOffset());
		
		/// <summary>
		/// <para>When used with a `using` statement, creates an isolated RNG context.</para>
		/// <para>Once the block is exited, the original RNG state is restored.</para>
		/// <para>This is similar to <see cref="With"/>, but it will use a special seed
		/// associated with the given thing that will be stable for the current in-game
		/// hour.</para>
		/// <para>You can get the seed from the context's struct, if needed.  Just
		/// assign the context to a variable.</para>
		/// </summary>
		/// <param name="thing">The thing to use as a seed source.</param>
		/// <param name="salt">Salt that can be added to munge the seed further.</param>
		/// <returns>An RNG context.</returns>
		public static Seeded ForHour(Thing thing, int salt = 42) =>
			new(Verse.Rand.RandSeedForHour(thing, salt));

		/// <summary>
		/// <para>When used with a `using` statement, creates an isolated RNG context.</para>
		/// <para>Once the block is exited, the original RNG state is restored.</para>
		/// <para>This is similar to <see cref="With"/>, but it will use a random seed
		/// based on the last RNG state.  This should be stable for multiplayer.</para>
		/// <para>You can get the seed from the context's struct, if needed.  Just
		/// assign the context to a variable.</para>
		/// </summary>
		/// <returns>An RNG context.</returns>
		public static Seeded Randomly() =>
			new(Verse.Rand.Int);
	}
}