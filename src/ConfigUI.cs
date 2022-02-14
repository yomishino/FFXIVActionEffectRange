using ActionEffectRange.Actions;
using Dalamud.Interface;
using Dalamud.Interface.Components;
using ImGuiNET;
using System.Collections.Generic;
using System.Linq;

namespace ActionEffectRange
{
    public static partial class ConfigUi
    {
        public static void Draw()
        {
            if (!Plugin.InConfig) return;

            DrawMainConfigUi();
            DrawActionBlacklistConfigUi();

            Plugin.RefreshConfig();
        }

        private static void DrawMainConfigUi()
        {
            ImGui.SetNextWindowSize(new(500, 400), ImGuiCond.FirstUseEver);
            if (ImGui.Begin("ActionEffectRange: Configuration"))
            {
                ImGui.TreePush();
                ImGui.Checkbox("Enable plugin", ref Plugin.Config.Enabled);
                ImGui.TreePop();

                if (Plugin.Config.Enabled)
                {
                    ImGui.NewLine();
                    ImGui.TreePush();
                    ImGui.Checkbox("Enable in PvP zones", ref Plugin.Config.EnabledPvP);
                    ImGui.TreePop();

                    ImGui.NewLine();
                    ImGui.Separator();
                    ImGui.NewLine();

                    ImGui.Text("Drawing Options");
                    ImGui.NewLine();
                    ImGui.TreePush();
                    ImGui.Columns(2, "DrawingOptions", false);
                    CheckboxWithTooltip("Enable for beneficial actions", ref Plugin.Config.DrawBeneficial,
                        "If enabled, will draw effect range for beneficial actions." +
                        "\nBeneficial actions are generally actions used in favour of player/allys such as healing and buffing actions." +
                        "\n\n(For actions with both beneficial and harmful effects, enabling this will allow drawing its beneficial effect range.)");
                    if (Plugin.Config.DrawBeneficial)
                    {
                        ImGui.Indent();
                        ImGui.Text("Colour: ");
                        ImGui.ColorEdit4("##BeneficialColour", ref Plugin.Config.BeneficialColour);
                        ImGui.Unindent();
                    }
                    ImGui.NextColumn();
                    CheckboxWithTooltip("Enable for harmful actions", ref Plugin.Config.DrawHarmful,
                        "If enabled, will draw effect range for harmful actions." +
                        "\nHarmful actions are generally actions used against enemies such as attacking debuffing actions." +
                        "\n\n(For actions with both beneficial and harmful effects, enabling this will allow drawing its harmful effect range.)");
                    if (Plugin.Config.DrawHarmful)
                    {
                        ImGui.Indent();
                        ImGui.Text("Colour: ");
                        ImGui.ColorEdit4("##HarmfulColour", ref Plugin.Config.HarmfulColour);
                        ImGui.Unindent();
                    }
                    ImGui.Columns(1);
                    ImGui.NewLine();
                    CheckboxWithTooltip("Enable drawing for your own pet's AoE actions", ref Plugin.Config.DrawOwnPets,
                        "If enabled, will also draw effect range for actions used by your own pet." +
                        "\nThis only works for Summoner/Scholar.");
                    CheckboxWithTooltip("Enable drawing for ground-targeted actions", ref Plugin.Config.DrawGT,
                        "If enabled, will also draw effect range for ground-targeted actions.");
                    ImGui.NewLine();

                    ImGui.Text("Actions with large effect range: ");
                    ImGui.Indent();
                    ImGui.Combo("##LargeDrawOpt", ref Plugin.Config.LargeDrawOpt, Configuration.LargeDrawOptions, Configuration.LargeDrawOptions.Length);
                    SetTooltip($"If set to any option other than \"{Configuration.LargeDrawOptions[0]}\", " +
                        "AoEs whose effect range is at least as large as the number specified below will be drawn (or not drawn at all) according to the set option." +
                        "\n\nThis only applies to Circle or Donut AoEs (including Ground-targeted ones). " +
                        "Other types of AoEs are not affected by this setting.");
                    ImGui.Unindent();
                    if (Plugin.Config.LargeDrawOpt > 0)
                    {
                        InputIntWithTooltip("Apply to actions with effect range >= ", ref Plugin.Config.LargeThreshold, 1, 1, 0, 80,
                            "The setting will be applied to actions with at least the specified effect range." +
                            "\nFor example, if set to 15, AoE such as Medica and Medica II will be affected by the setting, but not Cure III.");
                        if (Plugin.Config.LargeThreshold < 0) Plugin.Config.LargeThreshold = 0;
                        if (Plugin.Config.LargeThreshold > 55) Plugin.Config.LargeThreshold = 55;
                    }
                    ImGui.TreePop();

                    ImGui.NewLine();
                    ImGui.Separator();
                    ImGui.NewLine();

                    ImGui.Text("Style options");
                    ImGui.NewLine();
                    ImGui.TreePush();
                    ImGui.Columns(2, "StyleOptions", false);
                    ImGui.Checkbox("Draw outline (outer ring)", ref Plugin.Config.OuterRing);
                    if (Plugin.Config.OuterRing)
                    {
                        DragIntWithTooltip("Thickness: ", ref Plugin.Config.Thickness, 1, 1, 50, 60, null);
                        if (Plugin.Config.Thickness < 1) Plugin.Config.Thickness = 1;
                        if (Plugin.Config.Thickness > 50) Plugin.Config.Thickness = 50;
                    }
                    ImGui.NextColumn();
                    ImGui.Checkbox("Fill colour", ref Plugin.Config.Filled);
                    if (Plugin.Config.Filled)
                    {
                        DragFloatWithTooltip("Opacity: ", ref Plugin.Config.FillAlpha, .01f, 0, 1, "%.2f", 60, null);
                        if (Plugin.Config.FillAlpha < 0) Plugin.Config.FillAlpha = 0;
                        if (Plugin.Config.FillAlpha > 1) Plugin.Config.FillAlpha = 1;
                    }
                    ImGui.Columns(1);
                    ImGui.NewLine();
                    DragIntWithTooltip("Smoothness: ", ref Plugin.Config.NumSegments, 10, 40, 500, 100,
                        "The larger number, the smoothier");
                    if (Plugin.Config.NumSegments < 40) Plugin.Config.NumSegments = 40;
                    if (Plugin.Config.NumSegments > 500) Plugin.Config.NumSegments = 500;
                    ImGui.TreePop();

                    ImGui.NewLine();
                    ImGui.Separator();
                    ImGui.NewLine();

                    ImGui.TreePush();
                    DragFloatWithTooltip("Delay before drawing (sec): ", ref Plugin.Config.DrawDelay, .1f, 0, 2, "%.3f", 80,
                        "Delay (in seconds) to wait immediately after using an action before drawing the effect range.");
                    DragFloatWithTooltip("Remove drawing after time (sec): ", ref Plugin.Config.PersistSeconds, .1f, .1f, 5, "%.3f", 80,
                        "Allow the effect range drawn to last for the given time (in seconds) before erased from screen.");
                    ImGui.TreePop();

                    ImGui.NewLine();
                    ImGui.Separator();
                    ImGui.NewLine();

                    ImGui.TreePush();
                    if (ImGui.Button("Edit Action Blacklist"))
                        isActionBlacklistConfigUiActive = true;
                    ImGui.TreePop();

                    ImGui.NewLine();
                    ImGui.Separator();
                    ImGui.NewLine();

                    ImGui.TreePush();
                    ImGui.Checkbox($"[DEBUG] Log debug info to Dalamud Console", ref Plugin.Config.LogDebug);
                    ImGui.TreePop();
                }

                ImGui.NewLine();
                ImGui.Separator();
                ImGui.NewLine();

                if (ImGui.Button("Save & Close"))
                {
                    Plugin.Config.Save();
                    Plugin.InConfig = false;
                }

                ImGui.End();
            }
        }


        private static string GetActionDescription(Lumina.Excel.GeneratedSheets.Action row)
        {
            var classjobRow = row.ClassJob.Value;
            var classjob = classjobRow != null && classjobRow.RowId > 0
                ? $" [{classjobRow.Abbreviation}]" : string.Empty;
            var pvp = row.IsPvP ? " [PvP]" : string.Empty;
            return $"#{row.RowId} {row.Name}{classjob}{pvp}";
        }


        private static void SetTooltip(string tooltip)
        {
            if (ImGui.IsItemHovered())
                ImGui.SetTooltip(tooltip);
        }

        private static void CheckboxWithTooltip(string label, ref bool v, string? tooltip)
        {
            ImGui.Checkbox(label, ref v);
            if (tooltip != null) SetTooltip(tooltip);
        }

        private static void InputIntWithTooltip(string label, ref int v, int step, int stepFast, ImGuiInputTextFlags flags, float itemWidth, string? tooltip)
        {
            ImGui.Text(label);
            if (tooltip != null) SetTooltip(tooltip);
            ImGui.SameLine();
            ImGui.SetNextItemWidth(itemWidth);
            ImGui.InputInt("##" + label, ref v, step, stepFast, flags);
            if (tooltip != null) SetTooltip(tooltip);
        }

        private static void DragIntWithTooltip(string label, ref int v, int spd, int min, int max, float itemWidth, string? tooltip)
        {
            ImGui.Text(label);
            if (tooltip != null) SetTooltip(tooltip);
            ImGui.SameLine();
            ImGui.SetNextItemWidth(itemWidth);
            ImGui.DragInt("##" + label, ref v, spd, min, max);
            if (tooltip != null) SetTooltip(tooltip);
        }


        private static void DragFloatWithTooltip(string label, ref float v, float spd, float min, float max, string format, float itemWidth, string? tooltip)
        {
            ImGui.Text(label);
            if (tooltip != null) SetTooltip(tooltip);
            ImGui.SameLine();
            ImGui.SetNextItemWidth(itemWidth);
            ImGui.DragFloat("##" + label, ref v, spd, min, max, format);
            if (tooltip != null) SetTooltip(tooltip);
        }

    }


    public static partial class ConfigUi
    {
        private const string actionBlacklistConfigUiName
            = "ActionEffectRange: Configuration - Action Blacklist";

        private static bool isActionBlacklistConfigUiActive;
        private static bool isActionBlacklistEdited;
        private static bool shouldShowActionBlacklistMatches;
        private static string actionBlacklistInput = string.Empty;
        private static List<Lumina.Excel.GeneratedSheets.Action>? actionBlacklistMatchedActions = null;
        private static Lumina.Excel.GeneratedSheets.Action? selectedActionBlacklistActionRow = null;


        private static void DrawActionBlacklistConfigUi()
        {
            if (isActionBlacklistConfigUiActive)
                ImGui.OpenPopup(actionBlacklistConfigUiName);
            else
            {
                ClearActionBlacklistInput();
                return;
            }
            ImGui.SetNextWindowSize(ImGuiHelpers.ScaledVector2(400, 400), ImGuiCond.FirstUseEver);
            if (ImGui.BeginPopupModal(actionBlacklistConfigUiName, ref isActionBlacklistConfigUiActive))
            {
                ImGui.TextWrapped("Action in this blacklist will not be drawn.");
                ImGui.TextWrapped("You can use this blacklist to prevent a particular action from being drawn regardless of other settings.");
                ImGui.NewLine();

                if (ImGui.BeginTable("ActionEffectRange_Tbl_ActionBlacklistConfig", 4, ImGuiTableFlags.BordersH | ImGuiTableFlags.SortMulti))
                {
                    ImGui.TableSetupColumn("Action ID", ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("Action", ImGuiTableColumnFlags.WidthStretch);
                    ImGui.TableSetupColumn("ClassJob", ImGuiTableColumnFlags.DefaultSort | ImGuiTableColumnFlags.WidthFixed);
                    ImGui.TableSetupColumn("##Remove", ImGuiTableColumnFlags.NoSort | ImGuiTableColumnFlags.WidthFixed, 
                        UiBuilder.IconFont.FontSize * ImGuiHelpers.GlobalScale * 2);
                    ImGui.TableHeadersRow();

                    foreach (var actionId in Plugin.Config.ActionBlacklist)
                    {
                        var excelRow = ActionData.GetActionExcelRow(actionId);
                        ImGui.TableNextRow();
                        ImGui.TableSetColumnIndex(0);
                        ImGui.Text(actionId.ToString());
                        ImGui.TableNextColumn();
                        ImGui.Text(excelRow?.Name ?? string.Empty);
                        ImGui.TableNextColumn();
                        ImGui.Text(excelRow?.ClassJob.Value?.Name ?? string.Empty);
                        ImGui.TableNextColumn();
                        if (ImGuiComponents.IconButton((int)actionId, FontAwesomeIcon.Minus))
                        {
                            SetTooltip("Unblacklist");
                            ActionData.ActionBlacklist.Remove(actionId);
                            isActionBlacklistEdited = true;
                        }
                    }
                    ImGui.TableNextRow();   // dummy
                    ImGui.EndTable();
                }
                ImGui.NewLine();

                ImGui.PushID("ActionBlacklistInputAction");
                ImGui.Text("Search an action to add");
                ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                var input = actionBlacklistInput;
                ImGui.InputTextWithHint("##actionBlacklistRawInput",
                    "Search by Action ID / Action Name...", ref input, 64);
                if (ImGui.IsItemActivated() && !string.IsNullOrWhiteSpace(input))
                    shouldShowActionBlacklistMatches = true;
                if (ImGui.IsAnyMouseDown() && !ImGui.IsAnyItemHovered()) shouldShowActionBlacklistMatches = false;
                if (actionBlacklistInput != input)
                {
                    actionBlacklistInput = input;
                    actionBlacklistMatchedActions = string.IsNullOrWhiteSpace(input) ? null
                        : ActionData.GetAllPartialMatchActionExcelRows(
                            actionBlacklistInput, true, int.MaxValue,
                            a => a != null && a.IsPlayerAction && ActionData.IsPlayerCombatAction(a))?
                            .ToList();
                    shouldShowActionBlacklistMatches = actionBlacklistMatchedActions != null && actionBlacklistMatchedActions.Any();
                }
                if (shouldShowActionBlacklistMatches)
                {
                    if (ImGui.BeginChildFrame(1, new(ImGui.GetContentRegionAvail().X, ImGui.GetTextLineHeight() * 10),
                        ImGuiWindowFlags.NoFocusOnAppearing))
                    {
                        if (actionBlacklistMatchedActions != null)
                        {
                            foreach (var row in actionBlacklistMatchedActions)
                            {
                                if (ImGui.Selectable(GetActionDescription(row)))
                                {
                                    selectedActionBlacklistActionRow = row;
                                    shouldShowActionBlacklistMatches = false;
                                }
                            }
                        }
                        ImGui.EndChildFrame();
                    }
                }
                ImGui.NewLine();

                if (selectedActionBlacklistActionRow != null)
                {
                    ImGui.Text($"Add this action to Blacklist?");
                    ImGui.Indent();
                    ImGui.Text(GetActionDescription(selectedActionBlacklistActionRow));
                    ImGui.Unindent();
                    if (ImGuiComponents.IconButton(FontAwesomeIcon.Plus))
                    {
                        ActionData.ActionBlacklist.Add(selectedActionBlacklistActionRow.RowId);
                        ClearActionBlacklistInput();
                        isActionBlacklistEdited = true;
                    }
                    SetTooltip("Add new action");
                    ImGui.SameLine();
                    if (ImGuiComponents.IconButton(FontAwesomeIcon.Times))
                            ClearActionBlacklistInput();
                    SetTooltip("Clear new action");
                }
                else 
                    ImGuiComponents.DisabledButton(FontAwesomeIcon.Plus);

                ImGui.PopID();

                ImGui.NewLine();
                if (ImGui.Button("Close"))
                {
                    isActionBlacklistConfigUiActive = false;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }

            if (isActionBlacklistEdited)
            {
                ActionData.ActionBlacklist.Save();
                isActionBlacklistEdited = false;
            }
        }

        private static void ClearActionBlacklistInput()
        {
            actionBlacklistInput = string.Empty;
            shouldShowActionBlacklistMatches = false;
            actionBlacklistMatchedActions = null;
            selectedActionBlacklistActionRow = null;
        }
    }
}
