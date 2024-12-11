using LRT.Smith.Statistics;
using LRT.Utility;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace LRT.Smith.Equipments
{
	[Serializable]
	public class EquipmentData
	{
		public string id;
		public string rarityID;
		public string setID;

		public string name;
		public Sprite Icon;
		public List<SpecialTags> tags;
		public List<Statistic> statistics;
	}

	[Serializable]
	public class SpecialTags : Tags
	{
		public string id;
		protected override IEnumerable<string> GetOptions() => throw new NotImplementedException(); //SpecialTagsData.Instance.Get(id);
	}

	[Serializable]
	public class RarityData
	{
		string id;
		string name;
		List<Modifier> modifier;
	}

	[Serializable]
	public class SetData
	{
		string id;
		string name;
		List<string> equipmentID;
	}

	[Serializable]
	public class Modifier
	{
		ModifierType modifierType;
		float modifier;
	}

	public enum ModifierType
	{
		Offset,
		Percentage,
	}
}
