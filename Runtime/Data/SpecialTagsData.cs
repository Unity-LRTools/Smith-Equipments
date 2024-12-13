using System;
using System.Collections.Generic;
using System.Linq;

namespace LRT.Smith.Equipments
{
	[Serializable]
	public class SpecialTagsData : LazySingletonScriptableObject<SpecialTagsData>
	{
		private static string GetPath() => "Assets/Smith/Equipments/Resources"; //Called by reflection

		public EquipmentData equipment = new EquipmentData();

		public List<KeyValue> specialTags = new List<KeyValue>();

		public List<string> GetSpecialTagsOptions(SpecialTags tag)
		{
			KeyValue keyValue = specialTags.FirstOrDefault(kv => kv.id == tag.id);

			if (keyValue != null)
				return keyValue.options;

			specialTags.Add(new KeyValue(tag.id));
			
			return specialTags[^1].options;
		} 
	}

	[Serializable]
	public class KeyValue
	{
		public string id;
		public List<string> options;

		public KeyValue(string id)
		{
			this.id = id;
			options = new List<string>();
		}
	}
}

