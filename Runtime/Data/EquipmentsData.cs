
using System.Collections.Generic;

namespace LRT.Smith.Equipments
{
	public class EquipmentsData : LazySingletonScriptableObject<EquipmentsData>
	{
		private static string GetPath() => "Assets/Smith/Equipments/Resources"; //Called by reflection

		public List<EquipmentData> equipments = new List<EquipmentData>();
	}
}
