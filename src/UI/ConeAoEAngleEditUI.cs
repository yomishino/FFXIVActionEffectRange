using ActionEffectRange.Actions;
using ActionEffectRange.Actions.Data.Template;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using System.Numerics;

namespace ActionEffectRange.UI
{
    public class ConeAoEAngleEditUI : ActionDataEditUi<ConeAoEAngleDataItem>
    {
        private int centralAngleDegInput;
        private int rotationOffsetDegInput;

        protected override Vector2 InitialUiSize => new(500, 400);
        protected override string UiName => "Cone AoE";

        protected override DataTableModel DataTableViewModel { get; set; } = null!;


        public ConeAoEAngleEditUI()
        {
            BuildDataTableModel();
        }

        public override void DrawContents()
        {
            DrawIntro();
            DataTableViewModel.DrawTable(
                ActionData.GetCustomisedConeAoEAngleListCopy());
            DrawActionSearchUi();
            DrawAngleEditInputUi();
        }

        private void BuildDataTableModel()
        {
            var model = new DataTableModel(
                "ActionEffectRange_Tbl_ConeAoEAngleEdit",
                actionId => ActionData.RemoveFromConeAoEAngleList(actionId));
            model.AddDataColumn("Central\n Angle", ImGuiTableColumnFlags.WidthFixed, 
                false, 0, d => $"{ActionDataInterfacing.CycleToDeg(d.CentralAngleCycles):0}");
            model.AddDataColumn("Rotation\n Offset", ImGuiTableColumnFlags.WidthFixed,
                false, 0, d => $"{ActionDataInterfacing.RadToDeg(d.RotationOffset):0}");
            DataTableViewModel = model;
        }

        private static void DrawIntro()
        {
            ImGuiExt.MultiTextWrapped(
                "Customising the drawing of the cone AoEs here.",
                "You can use this list to temporarily fix these errors " +
                "regarding cone-shaped AoEs, should they exist:");
            ImGuiExt.BulletTextWrappedWithHelpMarker("Central Angle",
                "Central Angle: Sets the central angle of the sector drawn for the Cone AoE.\n\n" +
                "For example, set the Central Angle for an action to 90 " +
                "if you want the plugin to draw a sector of 90 degrees when you used that action.");
            ImGuiExt.BulletTextWrappedWithHelpMarker("Rotation Offset",
                "Rotation Offset: Adds certain degrees to the calculated rotation (that is, direction) of the sector.\n\n" +
                "For example, set it to 180 so that the sector will be drawn to the opposite direction.\n" +
                "(You probably won't need this though.)");
            ImGuiExt.MultiTextWrapped(
                "Both Central Angle and Rotation Offset are specified in degrees.\n" +
                "E.g., a value of 90 means 90 degrees");
            ImGuiExt.MultiTextWrapped(
                "Note: settings here only apply to AoE actions that are regarded as Cone AoE by this plugin.");
            ImGui.NewLine();
        }

        private void DrawAngleEditInputUi()
        {
            ImGui.PushID("ConeAoEAngleEditInput");

            if (selectedMatchedActionRow != null)
            {
                ImGui.Text("Editing for action: " +
                    ActionDataInterfacing.GetActionDescription(selectedMatchedActionRow));
                ImGui.Indent();
                ImGuiExt.InputIntWithTooltip("Central angle: ", ref centralAngleDegInput, 
                    1, 10, 0, 360, ImGuiInputTextFlags.None, 100, null);
                ImGuiExt.InputIntWithTooltip("Rotation offset: ", ref rotationOffsetDegInput, 
                    1, 10, 0, 360, ImGuiInputTextFlags.None, 100, null);

                if (ImGuiExt.IconButton(1, FontAwesomeIcon.Plus, "Add to the list"))
                {
                    ActionData.AddToConeAoEAngleList(selectedMatchedActionRow.RowId,
                        ActionDataInterfacing.DegToCycle(centralAngleDegInput),
                        ActionDataInterfacing.DegToRad(rotationOffsetDegInput));
                    ClearEditInput();
                }
                ImGui.SameLine();
                if (ImGuiExt.IconButton(2, FontAwesomeIcon.Times, "Clear Input"))
                    ClearEditInput();
                ImGui.Unindent();
            }
            else
                ImGuiComponents.DisabledButton(FontAwesomeIcon.Plus);

            ImGui.PopID();
            ImGui.NewLine();
        }

    }
}
