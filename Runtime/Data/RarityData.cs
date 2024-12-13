using System;
using System.Collections.Generic;

namespace LRT.Smith.Equipments
{
	[Serializable]
	public class RarityData
	{
		string id;
		string name;
		List<Modifier> modifier;
	}

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
