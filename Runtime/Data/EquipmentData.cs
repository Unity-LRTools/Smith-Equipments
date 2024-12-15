using LRT.Smith.Statistics;
using System;
using System.Collections.Generic;
using System.Linq;

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

		public bool HasSet => setID != null;

		public EquipmentData() { }

		public EquipmentData(EquipmentData origin)
		{
			id = origin.id;
			rarityID = origin.rarityID;
			setID = origin.setID;
			name = origin.name;
			statistics = origin.statistics.ConvertAll(s => s.Clone() as Statistic);
			flags = origin.flags.ConvertAll(f => f.Clone() as EnumFlag);
		}
	}

	[Serializable]
	public class EnumFlag : ICloneable
	{
		public int mask;
		public string @enum;

		public object Clone()
		{
			return new EnumFlag()
			{
				mask = mask,
				@enum = @enum,
			};
		}
	}
}
