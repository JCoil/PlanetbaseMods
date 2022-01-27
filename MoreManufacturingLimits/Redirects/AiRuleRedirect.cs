using Planetbase;
using Redirection;
using System;
using System.Collections.Generic; 
using System.Text; 
using UnityEngine;

namespace ImprovedManufacturingLimits
{
	public abstract class AiRuleRedirect : AiRule
	{
		/// <summary>
		/// Vegetable pads are classed as buildable to repair, so we can stop colonists working them here if they are above the limit
		/// </summary>
        [RedirectFrom(typeof(AiRule))]
        public static new Buildable findBuildableToRepair(Character character)
        {
            Buildable buildable = null;
            ConstructionComponent constructionComponent = FindDamagedNew(character, character.getSpecialization());

            if (constructionComponent != null)
            {
                Construction parentConstruction = constructionComponent.getParentConstruction();
                if (parentConstruction.isOperational() && parentConstruction.isSurvivable(character))
                {
                    buildable = constructionComponent;
                }
            }
            if (buildable == null && Singleton<SecurityManager>.getInstance().isGoingOutsideAllowed())
            {
                buildable = Construction.findDamaged(character, character.getSpecialization());
            }
            return buildable;
        }

		private static ConstructionComponent FindDamagedNew(Character character, Specialization specialization)
		{
			float num = float.MaxValue;
			ConstructionComponent result = null;
			int count = ConstructionComponent.mComponents.Count;
			for (int i = 0; i < count; i++)
			{
				ConstructionComponent constructionComponent = ConstructionComponent.mComponents[i];
				if (constructionComponent.isDamaged(specialization) 
					&& constructionComponent.isBuilt() 
					&& constructionComponent.getPotentialUserCount(character) == 0
					&& ManufactureLimitsHelper.isUnderLimit(constructionComponent))
				{
					float num2 = (constructionComponent.getPosition() - character.getPosition()).magnitude;
					if (constructionComponent.isHighPriority())
					{
						num2 -= 100f;
					}
					if (constructionComponent.mConditionIndicator.isVeryLow())
					{
						num2 -= 10f;
					}
					else if (constructionComponent.mConditionIndicator.isExtremelyLow())
					{
						num2 -= 20f;
					}
					if (num2 < num)
					{
						num = num2;
						result = constructionComponent;
					}
				}
			}
			return result;
		}
	}
}
