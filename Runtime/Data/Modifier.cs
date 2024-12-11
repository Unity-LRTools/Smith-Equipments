using System;

namespace LRT.Smith.Equipments
{
	[Serializable]
	public class Modifier
	{
		Type modifierType;
		float modifier;

		public enum Type
		{
			Offset,
			Percentage,
		}
	}
}
