using LRT.Smith.Statistics;
using System;
using System.Collections.Generic;

namespace LRT.Smith.Equipments
{
	[Serializable]
	public class EquipmentData
	{
		public string id;
		public string rarityID;
		public string setID;

		public string name;
		public List<Statistic> statistics = new List<Statistic>();
		public List<EnumFlag> flags = new List<EnumFlag>();
	}

	[Serializable]
	public class EnumFlag
	{
		public int mask;
		public string @enum;
	}
}
