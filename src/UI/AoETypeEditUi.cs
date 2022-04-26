using ActionEffectRange.Actions;
using ActionEffectRange.Actions.Data.Template;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using System;
using System.Numerics;

namespace ActionEffectRange.UI
{
    public class AoETypeEditUi : ActionDataEditUi<AoETypeDataItem>
    {
        private int selectedAoEType = 0;
        private int selectedHarmfulness = 0;

        private static readonly string[] aoeTypeSelectionsDisplayed
            = Array.ConvertAll(
                ActionDataInterfacing.AoETypeSelections, t => $"{t}");
        private static readonly string[] harmfulnessSelectionsDisplayed
            = Array.ConvertAll(
                ActionDataInterfacing.ActionHarmfulnessesSelections, t => $"{t}");

        protected override Vector2 InitialUiSize => new(500, 400);
        protected override string UiName => "AoE Types";
        protected override DataTableModel DataTableViewModel { get; set; } = null!;

        public AoETypeEditUi()
        {
            BuildDataTableModel();
        }

        private void BuildDataTableModel()
        {
            var model = new DataTableModel(
                "ActionEffectRange_Tbl_AoETypeEdit",
                actionId => ActionData.RemoveFromAoETypeList(actionId));
            model.AddDataColumn("AoE Type", ImGuiTableColumnFlags.WidthFixed,
                false, 0, d => ActionDataInterfacing.GetAoETypeLabel(d.CastType));
            model.AddDataColumn("Harmful/\nBeneficial", ImGuiTableColumnFlags.WidthFixed,
                false, 0, d => $"{d.Harmfulness}");
            DataTableViewModel = model;
        }

        public override void DrawContents()
        {
            DrawIntro();
            DataTableViewModel.DrawTable(
                ActionData.GetCustomisedAoETypeListCopy());
            DrawActionSearchUi();
            DrawTypeEditInputUi();
        }

        private static void DrawIntro()
        {
            ImGuiExt.MultiTextWrapped(
                "Customising the AoE types of AoE actions.",
                "You can use this list to force an AoE action to be treated as another type.");
            ImGui.NewLine();
        }

        private void DrawTypeEditInputUi()
        {
            ImGui.PushID("AoETypeEditInput");

            if (selectedMatchedActionRow != null)
            {
                ImGui.Text("Editing for action: " +
                    ActionDataInterfacing.GetActionDescription(selectedMatchedActionRow));
                ImGui.Indent();
                ImGuiExt.ComboWithTooltip("AoE type: ", "##comboAoETypeInput", 
                    ref selectedAoEType, aoeTypeSelectionsDisplayed, 
                    aoeTypeSelectionsDisplayed.Length, 280, null);
                ImGuiExt.ComboWithTooltip("Harmful/Beneficial: ", "##comboHarmfulnessInput",
                    ref selectedHarmfulness, harmfulnessSelectionsDisplayed, 
                    harmfulnessSelectionsDisplayed.Length, 200, null);
                
                if (ImGuiExt.IconButton(1, FontAwesomeIcon.Plus, "Add to the list"))
                {
                    ActionData.AddToAoETypeList(selectedMatchedActionRow.RowId,
                        (byte)ActionDataInterfacing.AoETypeSelections[selectedAoEType],
                        ActionDataInterfacing.ActionHarmfulnessesSelections[selectedHarmfulness]);
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
