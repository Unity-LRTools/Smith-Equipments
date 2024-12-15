using LRT.Smith.Statistics;
using LRT.Smith.Statistics.Editor;
using LRT.Utility;
using LRT.Utility.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace LRT.Smith.Equipments.Editor
{
	public class EquipmentsWindow : EditorWindow
	{
		private Dictionary<WindowPanelType, WindowPanel> panels;
		private WindowPanelType state = WindowPanelType.Rarity;
		private WindowPanelView view = WindowPanelView.Create;

		private Dictionary<WindowPanelType, bool> foldouts = new Dictionary<WindowPanelType, bool>()
			{
				{ WindowPanelType.Equipment, true },
				{ WindowPanelType.Rarity, true },
				{ WindowPanelType.Set, true },
			};

		private const int LEFT_SIZEBAR = 150;
		private const float PADDING = 5;

		[MenuItem("Smith/Equipments")]
		public static void ShowWindow()
		{
			EquipmentsWindow wizard = GetWindow<EquipmentsWindow>("Equipments", typeof(StatisticSettingsWizard));
			wizard.titleContent = new GUIContent("Equipments", EditorGUIUtility.IconContent("CustomTool@2x").image);
		}

		void OnEnable()
		{
			if (panels == null)
			{
				panels = new Dictionary<WindowPanelType, WindowPanel>
				{
					{  WindowPanelType.Equipment, new EquipmentPanel(this) },
					{  WindowPanelType.Rarity, new RarityPanel(this) },
					{  WindowPanelType.Set, new SetPanel(this) },
				};
				state = WindowPanelType.Equipment;
			}
		}

		void OnGUI()
		{
			Rect start = position.SetPos(PADDING, PADDING);
			Rect left = start.SetWidth(LEFT_SIZEBAR);
			Rect middle = start.SetWidth(2).MoveX(LEFT_SIZEBAR + 10);
			Rect right = start.MoveX(LEFT_SIZEBAR + 20).ChangeWidth(-LEFT_SIZEBAR - 30 - PADDING);

			DrawMenu(left);
			EditorGUI.DrawRect(middle, Color.gray);
			panels[state].OnGUI(right);
		}

		private void DrawMenu(Rect rect)
		{
			WindowPanelType[] values = (WindowPanelType[])Enum.GetValues(typeof(WindowPanelType));
			string[] names = Enum.GetNames(typeof(WindowPanelType));

			rect = rect.SetHeight(EditorGUIUtility.singleLineHeight);
			for (int i = 0; i < values.Length; i++)
			{
				if (state == values[i])
					EditorGUI.DrawRect(rect, Color.black * 0.35f);

				foldouts[values[i]] = EditorGUI.Foldout(rect, foldouts[values[i]], names[i]);

				rect = rect.MoveY(EditorGUIUtility.singleLineHeight);

				if (foldouts[values[i]])
				{
					rect = rect.MoveX(20).ChangeWidth(-20);
					WindowPanelView[] valuesView = (WindowPanelView[])Enum.GetValues(typeof(WindowPanelView));
					string[] namesView = Enum.GetNames(typeof(WindowPanelView));

					for (int j = 0; j < valuesView.Length; j++)
					{
						if (state == values[i] && view == valuesView[j])
							EditorGUI.DrawRect(rect, Color.white * 0.5f);

						if (GUI.Button(rect, namesView[j], GUI.skin.label))
						{
							state = values[i];
							view = valuesView[j];
						}

						rect = rect.MoveY(EditorGUIUtility.singleLineHeight);
					}
					rect = rect.MoveX(-20).ChangeWidth(20);
				}
			}
		}

		#region Panels
		public abstract class WindowPanel
		{
			protected EquipmentsWindow window;

			public WindowPanel(EquipmentsWindow wizard)
			{
				this.window = wizard;
			}

			public void OnGUI(Rect rect)
			{
				GUILayout.BeginArea(rect);

				switch (window.view)
				{
					case WindowPanelView.Create:
						OnGUICreate();
						break;
					case WindowPanelView.Update:
						OnGUIUpdate();
						break;
				}

				GUILayout.EndArea();
			}

			public abstract void OnGUICreate();

			public abstract void OnGUIUpdate();
		}

		#region Equipment
		public class EquipmentPanel : WindowPanel
		{
			public EquipmentData equipment = new EquipmentData();

			private EnumFlagListDrawer enumFlagDrawer;
			private StatisticListDrawer statisticDrawer;

			public EquipmentPanel(EquipmentsWindow window) : base(window)
			{
				enumFlagDrawer = new EnumFlagListDrawer(equipment.flags);
				statisticDrawer = new StatisticListDrawer(equipment.statistics);
			}

			public override void OnGUICreate()
			{
				EditorGUILayout.LabelField("New Equipment", EditorStyles.boldLabel);

				equipment.id = EditorGUILayout.TextField("ID", equipment.id);
				equipment.name = EditorGUILayout.TextField("name", equipment.name);

				#region Rarity
				if (EquipmentsData.Instance.rarities.Count > 0)
					equipment.rarityID = CustomGUIUtility.Popup("Rarity", equipment.rarityID, EquipmentsData.Instance.rarities, r => r.id, r => $"{r.name} ({r.id})");
				else
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("No Rarity Available", GUILayout.Width(EditorGUIUtility.labelWidth - 1));
					if (GUILayout.Button("Create Rarity"))
					{
						window.state = WindowPanelType.Rarity;
						window.view = WindowPanelView.Create;
					}
					EditorGUILayout.EndHorizontal();
				}
				#endregion

				#region Set
				if (EquipmentsData.Instance.sets.Count > 0)
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("Set", GUILayout.Width(EditorGUIUtility.labelWidth - 1));

					if (GUILayout.Button(equipment.setID == null ? "Enable Set" : "Disable Set", GUILayout.Width(100)))
						equipment.setID = equipment.setID == null ? EquipmentsData.Instance.sets[0].id : null;

					if (equipment.setID != null)
						equipment.setID = CustomGUIUtility.Popup("", equipment.setID, EquipmentsData.Instance.sets, s => s.id, s => $"{s.name} ({s.id})");

					EditorGUILayout.EndHorizontal();
				}
				else
				{
					EditorGUILayout.BeginHorizontal();
					EditorGUILayout.LabelField("No Set Available", GUILayout.Width(EditorGUIUtility.labelWidth - 1));
					if (GUILayout.Button("Create Set"))
					{
						window.state = WindowPanelType.Set;
						window.view = WindowPanelView.Create;
					}
					EditorGUILayout.EndHorizontal();
				}
				#endregion

				EditorListDrawer<Statistic>.Draw("Statistics", statisticDrawer);
				EditorListDrawer<EnumFlag>.Draw("Flags", enumFlagDrawer);

				List<string> errors = CheckErrors();
				List<string> warnings = CheckWarnings();

				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				using (new EditorGUI.DisabledGroupScope(errors.Count > 0))
				{
					if (GUILayout.Button("Create", GUILayout.MinWidth(100), GUILayout.MinHeight(35)))
					{
						EquipmentsData.Instance.equipments.Add(new EquipmentData(equipment));
						EquipmentsData.Instance.AddEquipmentToSet(equipment);
						equipment = new EquipmentData();
						enumFlagDrawer = new EnumFlagListDrawer(equipment.flags);
						statisticDrawer = new StatisticListDrawer(equipment.statistics);
						EditorUtility.SetDirty(EquipmentsData.Instance);
					}
				}
				EditorGUILayout.EndHorizontal();

				Errors(errors, warnings);
			}

			private List<string> CheckErrors()
			{
				List<string> errors = new List<string>();

				if (equipment.id == null)
					errors.Add("ID can't be null.");

				if (equipment.id != null && EquipmentsData.Instance.equipments.Select(e => e.id).Any(id => id == equipment.id))
					errors.Add("There is another equipment with the same ID.");

				if (equipment.rarityID == null)
					errors.Add("Equipment should always have a rarity");

				return errors;
			}

			private List<string> CheckWarnings()
			{
				List<string> warnings = new List<string>();

				if (equipment.statistics.Count == 0)
					warnings.Add("There is no statistic set.");

				if (equipment.flags.Count == 0)
					warnings.Add("There is no flags set.");

				foreach (EnumFlag flags in equipment.flags)
				{
					if (flags.mask == 0)
						warnings.Add("You've added an empty flag, maybe you want to add value.");
				}

				return warnings;
			}

			private void Errors(List<string> errors, List<string> warnings)
			{
				foreach (string error in errors)
				{
					EditorGUILayout.HelpBox(error, MessageType.Error);
				}

				foreach (string warning in warnings)
				{
					EditorGUILayout.HelpBox(warning, MessageType.Warning);
				}
			}

			public override void OnGUIUpdate()
			{

			}

			public class EnumFlagListDrawer : EditorListDrawer<EnumFlag>.ListDrawer
			{
				public override bool DrawHeader => false;
				public override string LabelAddButton => "Add Flag";

				string typeName;
				string fullTypeName;

				public EnumFlagListDrawer(List<EnumFlag> flags) : base(flags) { }

				public override void DrawItem(EnumFlag item, int index)
				{
					GUILayout.BeginHorizontal();

					Type enumType = Type.GetType(item.@enum);
					Enum e = (Enum)Enum.ToObject(enumType, item.mask);

					item.mask = (int)(object)EditorGUILayout.EnumFlagsField("Flags", e);

					DrawUtilities(index);

					GUILayout.EndHorizontal();
				}

				public override EnumFlag OnCreate()
				{
					EnumFlag flag = new EnumFlag()
					{
						@enum = fullTypeName,
						mask = 0,
					};

					return flag;
				}

				public override void DrawBeforeAddButton()
				{
					List<Type> types = GetEnumsWithAttribute<EquipmentFlags>(GetUnityRuntimeAssembly());
					string[] typesNames = types.Select(t => t.FullName).ToArray();
					int index = Mathf.Max(0, Array.IndexOf(typesNames, typeName));

					typeName = typesNames[EditorGUILayout.Popup("Pick a flag:", index, typesNames)];
					fullTypeName = types[index].AssemblyQualifiedName;
				}

				#region Helpers
				public Assembly[] GetUnityRuntimeAssembly()
				{
					// Get all loaded assemblies in the current AppDomain
					Assembly[] loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

					// Find Unity runtime and first-pass assemblies
					//Assembly firstPassAssembly = loadedAssemblies.FirstOrDefault(a => a.GetName().Name == "Assembly-CSharp-firstpass");
					Assembly runtimeAssembly = loadedAssemblies.FirstOrDefault(a => a.GetName().Name == "Assembly-CSharp");

					return new Assembly[1] { runtimeAssembly };
				}

				public static List<Type> GetEnumsWithAttribute<TAttribute>(params Assembly[] assemblies) where TAttribute : Attribute
				{
					List<Type> enums = new List<Type>();
					for (int i = 0; i < assemblies.Length; i++)
					{
						enums.AddRange(assemblies[i].GetTypes()
						.Where(type => type.IsEnum && type.GetCustomAttribute<TAttribute>() != null)
						.ToList());
					}
					return enums;
				}
				#endregion
			}

			public class StatisticListDrawer : EditorListDrawer<Statistic>.ListDrawer
			{
				public override bool DrawHeader => false;
				public override string LabelAddButton => "Add Statistic";

				public StatisticListDrawer(List<Statistic> items) : base(items) { }

				public override void DrawItem(Statistic item, int index)
				{
					GUILayout.BeginHorizontal();

					StatisticGUILayout.StatisticField("Statistic", item);

					DrawUtilities(index);

					GUILayout.EndHorizontal();
				}

				public override void DrawBeforeAddButton()
				{
					GUILayout.FlexibleSpace();
				}

				public override Statistic OnCreate()
				{
					return new Statistic();
				}
			}
		}
		#endregion

		#region Rarity
		public class RarityPanel : WindowPanel
		{
			RarityData rarity;

			ModifierListDrawer modifierDrawer;

			public RarityPanel(EquipmentsWindow window) : base(window)
			{
				rarity = new RarityData();
				modifierDrawer = new ModifierListDrawer(rarity.modifier);
			}

			public override void OnGUICreate()
			{
				EditorGUILayout.LabelField("New Rarity", EditorStyles.boldLabel);

				rarity.id = EditorGUILayout.TextField("ID", rarity.id);
				rarity.name = EditorGUILayout.TextField("Name", rarity.name);
				rarity.color = EditorGUILayout.ColorField("Color", rarity.color);
				EditorListDrawer<Modifier>.Draw("Modifiers", modifierDrawer);

				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Create", GUILayout.MinWidth(100), GUILayout.MinHeight(35)))
				{
					EquipmentsData.Instance.rarities.Add(rarity);
					rarity = new RarityData();
					EditorUtility.SetDirty(EquipmentsData.Instance);
				}
				EditorGUILayout.EndHorizontal();
			}

			public override void OnGUIUpdate()
			{

			}

			private class ModifierListDrawer : EditorListDrawer<Modifier>.ListDrawer
			{
				public override bool DrawHeader { get => false; }

				public override string LabelAddButton => "Add Modifier";

				public ModifierListDrawer(List<Modifier> items) : base(items) { }

				public override void DrawItem(Modifier item, int index)
				{
					EditorGUILayout.BeginHorizontal();

					item.modifierType = (Modifier.Type)EditorGUILayout.EnumPopup("Type", item.modifierType);

					if (item.modifierType == Modifier.Type.Offset)
					{
						item.modifier = EditorGUILayout.FloatField("Modifier", item.modifier);
					}
					else
					{
						item.modifier = EditorGUILayout.FloatField(item.modifier);
						EditorGUILayout.LabelField($"{item.modifier * 100}%", GUILayout.Width(60));
					}

					DrawUtilities(index);

					EditorGUILayout.EndHorizontal();
				}

				public override void DrawBeforeAddButton()
				{
					GUILayout.FlexibleSpace();
				}

				public override Modifier OnCreate()
				{
					return new Modifier();
				}
			}
		}
		#endregion

		#region Set
		public class SetPanel : WindowPanel
		{
			SetData set = new SetData();

			EquipmentListDrawer equipmentDrawer;

			public SetPanel(EquipmentsWindow window) : base(window)
			{
				equipmentDrawer = new EquipmentListDrawer(set.equipmentsID, set);
			}

			public override void OnGUICreate()
			{
				EditorGUILayout.LabelField("New Set", EditorStyles.boldLabel);

				set.id = EditorGUILayout.TextField("ID", set.id);
				set.name = EditorGUILayout.TextField("Name", set.name);
				EditorListDrawer<string>.Draw("Equipments", equipmentDrawer);

				EditorGUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button("Create", GUILayout.MinWidth(100), GUILayout.MinHeight(35)))
				{
					EquipmentsData.Instance.sets.Add(set);
					EquipmentsData.Instance.AddSetToEquipment(set);
					set = new SetData();
					EditorUtility.SetDirty(EquipmentsData.Instance);
				}
				EditorGUILayout.EndHorizontal();
			}

			public override void OnGUIUpdate()
			{

			}

			private class EquipmentListDrawer : EditorListDrawer<string>.ListDrawer
			{
				public override bool DrawHeader { get => false; }

				public override string LabelAddButton => "Add Equipment";

				SetData set;

				public EquipmentListDrawer(List<string> items, SetData set) : base(items)
				{
					this.set = set;
				}

				public override void DrawItem(string item, int index)
				{
					EditorGUILayout.BeginHorizontal();

					set.equipmentsID[index] = CustomGUIUtility.Popup($"n°{index + 1}", set.equipmentsID[index], EquipmentsData.Instance.equipments, e => e.id, e => $"{e.name} ({e.id})");

					DrawUtilities(index);

					EditorGUILayout.EndHorizontal();
				}

				public override void DrawBeforeAddButton()
				{
					GUILayout.FlexibleSpace();
				}
			}
		}
		#endregion
		#endregion

		private enum WindowPanelType
		{
			Equipment,
			Rarity,
			Set,
		}

		private enum WindowPanelView
		{
			Update,
			Create,
		}
	}

	public static class RectExtension
	{
		public static Rect NextLine(ref this Rect rect)
		{
			rect = rect.MoveY(EditorGUIUtility.singleLineHeight);
			return rect;
		}
	}
}

