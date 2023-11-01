using rjw.Modules.Genitals.Enums;
using rjw.Modules.Interactions.Contexts;
using rjw.Modules.Shared.Logs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rjw.Modules.Interactions.Internals.Implementation
{
	public class ReverseDetectorService : IReverseDetectorService
	{
		private static ILog _log = LogManager.GetLogger<ReverseDetectorService, InteractionLogProvider>();

		public static IReverseDetectorService Instance { get; private set; }

		static ReverseDetectorService()
		{
			Instance = new ReverseDetectorService();

			_random = new Random();
		}

		private static readonly Random _random;

		/// <summary>
		/// Do not instantiate, use <see cref="Instance"/>
		/// </summary>
		private ReverseDetectorService() { }

		private const float ReverseRapeChanceForFemale = 90 / 100f;
		private const float ReverseRapeChanceForMale = 10 / 100f;
		private const float ReverseRapeChanceForFuta = (ReverseRapeChanceForFemale + ReverseRapeChanceForMale) / 2f;

		private const float ReverseConsensualChanceForFemale = 75 / 100f;
		private const float ReverseConsensualChanceForMale = 25 / 100f;
		private const float ReverseConsensualChanceForFuta = (ReverseConsensualChanceForFemale + ReverseConsensualChanceForMale) / 2f;

		private const float ReverseBestialityChanceForFemale = 90 / 100f;
		private const float ReverseBestialityChanceForMale = 10 / 100f;
		private const float ReverseBestialityChanceForFuta = (ReverseBestialityChanceForFemale + ReverseBestialityChanceForMale) / 2f;

		private const float ReverseAnimalChanceForFemale = 90 / 100f;
		private const float ReverseAnimalChanceForMale = 10 / 100f;
		private const float ReverseAnimalChanceForFuta = (ReverseAnimalChanceForFemale + ReverseAnimalChanceForMale) / 2f;

		private const float ReverseWhoringChanceForFemale = 90 / 100f;
		private const float ReverseWhoringChanceForMale = 10 / 100f;
		private const float ReverseWhoringChanceForFuta = (ReverseWhoringChanceForFemale + ReverseWhoringChanceForMale) / 2f;

		public bool IsReverse(InteractionContext context)
		{
			//Necrophilia
			if (context.Internals.InteractionType == Enums.InteractionType.Necrophilia)
			{
				context.Internals.IsReverse = false;
			}
			//MechImplant
			if (context.Internals.InteractionType == Enums.InteractionType.Mechanoid)
			{
				context.Internals.IsReverse = false;
			}
			//Masturbation
			if (context.Internals.InteractionType == Enums.InteractionType.Masturbation)
			{
				context.Internals.IsReverse = false;
			}
			
			Gender initiatorGender = context.Internals.Dominant.Gender;
			Gender partnerGender = context.Internals.Submissive.Gender;

			float roll = (float)_random.NextDouble();

			bool result;
			
			if (context.Outputs.Generated.InteractionType == Enums.InteractionType.Consensual)
			{
				result = IsReverseRape(initiatorGender, partnerGender, roll, ReverseConsensualChanceForFemale, ReverseConsensualChanceForMale, ReverseConsensualChanceForFuta);
			}
			else
			if (context.Outputs.Generated.InteractionType == Enums.InteractionType.Animal)
			{
				result = IsReverseRape(initiatorGender, partnerGender, roll, ReverseAnimalChanceForFemale, ReverseAnimalChanceForMale, ReverseAnimalChanceForFuta);
			}
			else
			if (context.Outputs.Generated.InteractionType == Enums.InteractionType.Whoring)
			{
				result = IsReverseRape(initiatorGender, partnerGender, roll, ReverseWhoringChanceForFemale, ReverseWhoringChanceForMale, ReverseWhoringChanceForFuta);
			}
			else
			if (context.Outputs.Generated.InteractionType == Enums.InteractionType.Bestiality)
			{
				result = IsReverseRape(initiatorGender, partnerGender, roll, ReverseBestialityChanceForFemale, ReverseBestialityChanceForMale, ReverseBestialityChanceForFuta);
			}
			else
			if (context.Outputs.Generated.InteractionType == Enums.InteractionType.Rape)
			{
				result = IsReverseRape(initiatorGender, partnerGender, roll, ReverseRapeChanceForFemale, ReverseRapeChanceForMale, ReverseRapeChanceForFuta);
			}
			else
			{
				result = false;
			}
			
			return result;
		}
		
		private bool IsReverseRape(Gender initiator, Gender partner, float roll, float femaleChance, float maleChance, float futaChance)
		{
			_log.Debug($"{initiator}/{partner} - {roll} -> [f{femaleChance},m{maleChance},fu{futaChance}]");

			switch (initiator)
			{
				case Gender.Male:
					return roll < maleChance;
				case Gender.Female:
					return roll < femaleChance;
				case Gender.Trap:
					return roll < maleChance;
				case Gender.Futa:
				case Gender.FemaleOvi:
					return roll < futaChance;
				case Gender.MaleOvi:
					return roll < maleChance;
				case Gender.Unknown:
					break;
			}

			return false;
		}
	}
}
