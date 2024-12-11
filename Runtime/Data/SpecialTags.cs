using LRT.Utility;
using System;
using System.Collections.Generic;

namespace LRT.Smith.Equipments
{
	[Serializable]
	public class SpecialTags : Tags
	{
		public string id;
		protected override IEnumerable<string> GetOptions() => throw new NotImplementedException(); //SpecialTagsData.Instance.Get(id);
	}
}
