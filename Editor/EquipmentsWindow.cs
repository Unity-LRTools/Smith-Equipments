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
		private WindowPanelType state;
		private WindowPanelView view;
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

				EditorGUI.Foldout(rect, foldouts[values[i]], names[i]);

				if (GUI.Button(rect, "", GUI.skin.label))
				{
					state = values[i];
					foldouts[values[i]] = !foldouts[values[i]];
				}

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
				switch(window.view)
				{
					case WindowPanelView.Create:
						OnGUICreate(rect);
						break;
					case WindowPanelView.Update:
						OnGUIUpdate(rect);
						break;
				}
			}

			public abstract void OnGUICreate(Rect rect);

			public abstract void OnGUIUpdate(Rect rect);
		}

		#region Equipment
		public class EquipmentPanel : WindowPanel
		{
			public EquipmentData equipment;

			private EnumFlagListDrawer enumFlagDrawer;
			private StatisticListDrawer statisticDrawer;

			public EquipmentPanel(EquipmentsWindow window) : base(window) 
			{
				equipment = SpecialTagsData.Instance.equipment;
				enumFlagDrawer = new EnumFlagListDrawer(equipment.flags);
				statisticDrawer = new StatisticListDrawer(equipment.statistics);
			}

			public override void OnGUICreate(Rect rect)
			{
				GUILayout.BeginArea(rect);

				equipment.id = EditorGUILayout.TextField("id", equipment.id);
				equipment.rarityID = EditorGUILayout.TextField("rarityID", equipment.rarityID);
				equipment.setID = EditorGUILayout.TextField("setID", equipment.setID);
				equipment.name = EditorGUILayout.TextField("name", equipment.name);

				equipment.statistics = EditorListDrawer<Statistic>.Draw("Statistics", statisticDrawer);

				//equipment.flags.Clear();
				EditorListDrawer<EnumFlag>.Draw("Flags", enumFlagDrawer);

				GUILayout.EndArea();
			}

			public override void OnGUIUpdate(Rect rect)
			{
				
			}

			public class EnumFlagListDrawer : EditorListDrawer<EnumFlag>.ListDrawer
			{
				public override bool DrawHeader { get => false; }

				string typeName;
				string fullTypeName;

				public EnumFlagListDrawer(List<EnumFlag> flags) : base(flags) { }

				public override void DrawItem(EnumFlag item, int index)
				{
					GUILayout.BeginHorizontal();

					Type enumType = Type.GetType(item.@enum);
					Enum e = (Enum)Enum.ToObject(enumType, item.mask);

					item.mask = (int)(object)EditorGUILayout.EnumFlagsField("Flags", e);

					EditorListDrawer<EnumFlag>.DrawUtilities(items, index, Orderable);

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
			}

			public class StatisticListDrawer : EditorListDrawer<Statistic>.ListDrawer
			{
				public override bool DrawHeader { get => false; }

				public StatisticListDrawer(List<Statistic> items) : base(items) { }

				public override void DrawItem(Statistic item, int index)
				{
					GUILayout.BeginHorizontal();

					StatisticGUILayout.StatisticField("Statistic", item);

					EditorListDrawer<Statistic>.DrawUtilities(items, index, Orderable);

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
			public RarityPanel(EquipmentsWindow window) : base(window) { }

			public override void OnGUICreate(Rect rect)
			{
				EditorGUI.DrawRect(rect, Color.green);
			}

			public override void OnGUIUpdate(Rect rect)
			{
				throw new NotImplementedException();
			}
		}
		#endregion

		#region Set
		public class SetPanel : WindowPanel
		{
			public SetPanel(EquipmentsWindow window) : base(window) { }

			public override void OnGUICreate(Rect rect)
			{
				EditorGUI.DrawRect(rect, Color.blue);
			}

			public override void OnGUIUpdate(Rect rect)
			{
				throw new NotImplementedException();
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

