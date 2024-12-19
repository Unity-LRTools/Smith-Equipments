using System.Collections.Generic;
using System.Linq;

namespace LRT.Smith.Equipments
{
	public class EquipmentsData : LazySingletonScriptableObject<EquipmentsData>
	{
		private static string GetPath() => "Assets/Smith/Equipments/Resources"; //Called by reflection

		public List<EquipmentData> equipments = new List<EquipmentData>();
		public List<RarityData> rarities = new List<RarityData>();
		public List<SetData> sets = new List<SetData>();

		public void AddEquipmentToSet(EquipmentData equipment)
		{
			if (equipment.HasSet)
			{
				SetData set = sets.FirstOrDefault(s => s.id == equipment.setID);
				if (set != null)
					set.equipmentsID.Add(equipment.id);
			}
		}

		public void AddSetToEquipment(SetData set)
		{
			if(set.equipmentsID.Count > 0)
			{
				foreach(string id in set.equipmentsID)
				{
					EquipmentData equipment = equipments.FirstOrDefault(e => e.id == id);
					if (equipment != null)
						equipment.setID = set.id;
				}
			}
		}

		public void SynchronizeSetToEquipment()
		{
			foreach(SetData set in sets)
			{
				foreach(string id in set.equipmentsID)
				{
					GetEquipment(id).setID = set.id;
				}
			}
		}

		public void SynchronizeEquipmentToSet()
		{
			foreach (SetData set in sets)
			{
				set.equipmentsID.Clear();
			}

			foreach (EquipmentData equipment in equipments)
			{
				if (equipment.HasSet)
					GetSet(equipment.setID).equipmentsID.Add(equipment.id);
			}
		}

		public EquipmentData GetEquipment(string id)
		{
			return equipments.FirstOrDefault(e => e.id == id);
		}

		public RarityData GetRarity(string id)
		{
			return rarities.FirstOrDefault(r => r.id == id);
		}

		public SetData GetSet(string id)
		{
			return sets.FirstOrDefault(r => r.id == id);
		}
	}
}
