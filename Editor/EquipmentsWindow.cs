using LRT.Utility;
using LRT.Utility.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
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
			EquipmentsWindow wizard = GetWindow<EquipmentsWindow>("Equipments");
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
			Rect right = start.MoveX(LEFT_SIZEBAR + 20).ChangeWidth(-LEFT_SIZEBAR - 20 - PADDING);

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

			public abstract void OnGUI(Rect rect);
		}

		public class EquipmentPanel : WindowPanel
		{
			public EquipmentPanel(EquipmentsWindow window) : base(window) { }

			public override void OnGUI(Rect rect)
			{
				EditorGUI.DrawRect(rect, Color.red);
			}
		}

		public class RarityPanel : WindowPanel
		{
			public RarityPanel(EquipmentsWindow window) : base(window) { }

			public override void OnGUI(Rect rect)
			{
				EditorGUI.DrawRect(rect, Color.green);
			}
		}

		public class SetPanel : WindowPanel
		{
			public SetPanel(EquipmentsWindow window) : base(window) { }

			public override void OnGUI(Rect rect)
			{
				EditorGUI.DrawRect(rect, Color.blue);
			}
		}
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
}

