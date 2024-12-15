using System;
using System.Collections.Generic;

namespace LRT.Smith.Equipments
{
	[Serializable]
	public class SetData
	{
		public string id;
		public string name;
		public List<string> equipmentsID = new List<string>();
	}
}
