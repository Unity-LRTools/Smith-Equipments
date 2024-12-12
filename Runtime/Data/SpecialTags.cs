using LRT.Utility;
using System;
using System.Collections.Generic;

namespace LRT.Smith.Equipments
{
	[Serializable]
	public class SpecialTags : Tags
	{
		public string id;
		protected override IEnumerable<string> GetOptions() => SpecialTagsData.Instance.GetSpecialTagsOptions(this);

		public SpecialTags(string id)
		{
			this.id = id;
		}
	}
}
