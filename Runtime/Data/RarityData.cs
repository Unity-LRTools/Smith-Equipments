using System;
using System.Collections.Generic;
using UnityEngine;

namespace LRT.Smith.Equipments
{
	[Serializable]
	public class RarityData
	{
		public string id;
		public string name;
		public Color color = Color.white;
		public List<Modifier> modifier = new List<Modifier>();
	}

	[Serializable]
	public class Modifier
	{
		public Type modifierType;
		public float modifier;

		public enum Type
		{
			Offset,
			Percentage,
		}

		public override string ToString()
		{
			return $"{modifierType}:{modifier}";
		}
	}
}
