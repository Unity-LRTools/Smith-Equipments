using LRT.Smith.Statistics;
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
		public string visualID;

		public string name;
		public Sprite Icon;
		public List<SpecialTags> tags;
		public List<IStatistic> statistics;
	}
}
