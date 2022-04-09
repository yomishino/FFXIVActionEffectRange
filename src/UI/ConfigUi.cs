using ActionEffectRange.Actions;
using ImGuiNET;
using System.Diagnostics;

namespace ActionEffectRange.UI
{
    public static class ConfigUi
    {
        private static readonly ActionBlacklistEditUI actionBlacklistEditUI = new();
        private static readonly ConeAoEAngleEditUI coneAoEAngleEditUI = new();

        public static void Draw()
        {
            if (!Plugin.InConfig) return;

            DrawMainConfigUi();

            DrawSubUIs();

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
                    ImGuiExt.CheckboxWithTooltip("Enable for beneficial actions", 
                        ref Plugin.Config.DrawBeneficial,
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
                    ImGuiExt.CheckboxWithTooltip("Enable for harmful actions", 
                        ref Plugin.Config.DrawHarmful,
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
                    ImGuiExt.CheckboxWithTooltip("Enable drawing for your own pet's AoE actions", 
                        ref Plugin.Config.DrawOwnPets,
                        "If enabled, will also draw effect range for actions used by your own pet." +
                        "\nThis only affects Summoner/Scholar pet actions.");
                    ImGuiExt.CheckboxWithTooltip("Enable drawing for ground-targeted actions", 
                        ref Plugin.Config.DrawGT,
                        "If enabled, will also draw effect range for ground-targeted actions.");
                    ImGuiExt.CheckboxWithTooltip("Enable drawing for Special/Artillery actions", 
                        ref Plugin.Config.DrawEx,
                        "If enabled, will also draw effect range for actions of category " +
                        $"\"{ActionData.GetActionCategoryName(Actions.Enums.ActionCategory.Special)}\" or " +
                        $"\"{ActionData.GetActionCategoryName(Actions.Enums.ActionCategory.Artillery)}\"." +
                        $"\n\nActions of these categories are generally available in certain contents/duties, " +
                        $"\nafter you mount something or transformed into something, etc." +
                        $"\n\nPlease note however, that effect range drawing for these actions may be very inaccurate.");
                    ImGui.NewLine();

                    ImGui.Text("Actions with large effect range: ");
                    ImGui.Indent();
                    ImGui.Combo("##LargeDrawOpt", ref Plugin.Config.LargeDrawOpt, Configuration.LargeDrawOptions, Configuration.LargeDrawOptions.Length);
                    ImGuiExt.SetTooltipIfHovered($"If set to any option other than \"{Configuration.LargeDrawOptions[0]}\", " +
                        "AoEs whose effect range is at least as large as the number specified below will be drawn (or not drawn at all) according to the set option." +
                        "\n\nThis only applies to Circle or Donut AoEs (including Ground-targeted ones). " +
                        "Other types of AoEs are not affected by this setting.");
                    ImGui.Unindent();
                    if (Plugin.Config.LargeDrawOpt > 0)
                    {
                        ImGuiExt.InputIntWithTooltip("Apply to actions with effect range >= ", 
                            ref Plugin.Config.LargeThreshold, 1, 1, 5, 55, 0, 80,
                            "The setting will be applied to actions with at least the specified effect range." +
                            "\nFor example, if set to 15, AoE such as Medica and Medica II will be affected by the setting, but not Cure III.");
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
                        ImGuiExt.DragIntWithTooltip("Thickness: ", 
                            ref Plugin.Config.Thickness, 1, 1, 50, 60, null);
                        if (Plugin.Config.Thickness < 1) Plugin.Config.Thickness = 1;
                        if (Plugin.Config.Thickness > 50) Plugin.Config.Thickness = 50;
                    }
                    ImGui.NextColumn();
                    ImGui.Checkbox("Fill colour", ref Plugin.Config.Filled);
                    if (Plugin.Config.Filled)
                    {
                        ImGuiExt.DragFloatWithTooltip("Opacity: ", 
                            ref Plugin.Config.FillAlpha, .01f, 0, 1, "%.2f", 60, null);
                        if (Plugin.Config.FillAlpha < 0) Plugin.Config.FillAlpha = 0;
                        if (Plugin.Config.FillAlpha > 1) Plugin.Config.FillAlpha = 1;
                    }
                    ImGui.Columns(1);
                    ImGui.NewLine();
                    ImGuiExt.DragIntWithTooltip("Smoothness: ", 
                        ref Plugin.Config.NumSegments, 10, 40, 500, 100,
                        "The larger number, the smoothier");
                    if (Plugin.Config.NumSegments < 40) Plugin.Config.NumSegments = 40;
                    if (Plugin.Config.NumSegments > 500) Plugin.Config.NumSegments = 500;
                    ImGui.TreePop();

                    ImGui.NewLine();
                    ImGui.Separator();
                    ImGui.NewLine();

                    ImGui.TreePush();
                    ImGuiExt.DragFloatWithTooltip("Delay before drawing (sec): ", 
                        ref Plugin.Config.DrawDelay, .1f, 0, 2, "%.3f", 80,
                        "Delay (in seconds) to wait immediately after using an action before drawing the effect range.");
                    ImGuiExt.DragFloatWithTooltip("Remove drawing after time (sec): ", 
                        ref Plugin.Config.PersistSeconds, .1f, .1f, 5, "%.3f", 80,
                        "Allow the effect range drawn to last for the given time (in seconds) before erased from screen.");
                    ImGui.TreePop();

                    ImGui.NewLine();
                    ImGui.Separator();
                    ImGui.NewLine();

                    ImGui.TreePush();
                    if (ImGui.Button("Edit Action Blacklist"))
                        actionBlacklistEditUI.OpenUI();
                    if (ImGui.Button("Customise Cone AoE Drawing"))
                        coneAoEAngleEditUI.OpenUI();
                    ImGui.TreePop();

                    ImGui.NewLine();
                    ImGui.Separator();
                    ImGui.NewLine();

                    ImGui.TreePush();
                    ImGui.Checkbox($"[DEBUG] Log debug info to Dalamud Console", ref Plugin.Config.LogDebug);
                    ImGui.NewLine();
                    ImGui.Checkbox("Show Sponsor/Support button", ref Plugin.Config.showSponsor);
                    if (Plugin.Config.showSponsor)
                    {
                        ImGui.Indent();
                        ImGui.PushStyleColor(ImGuiCol.Button, 0xFF000000 | 0x005E5BFF);
                        ImGui.PushStyleColor(ImGuiCol.ButtonActive, 0xDD000000 | 0x005E5BFF);
                        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, 0xAA000000 | 0x005E5BFF);
                        if (ImGuiExt.Button("Buy Yomishino a coffee", 
                            "You can support me and buy me a coffee if you want.\n" +
                            "(Will open external link to Ko-fi in your browser)"))
                        {
                            Process.Start(new ProcessStartInfo
                            {
                                FileName = "https://ko-fi.com/yomishino",
                                UseShellExecute = true
                            });
                        }
                        ImGui.PopStyleColor(3);
                        ImGui.Unindent();
                    }
                    ImGui.TreePop();
                }

                ImGui.NewLine();
                ImGui.Separator();
                ImGui.NewLine();

                if (ImGui.Button("Save & Close"))
                {
                    CloseSubUIs();
                    ActionData.SaveCustomisedData();
                    Plugin.Config.Save();
                    Plugin.InConfig = false;
                }

                ImGui.End();
            }
        }

        private static void DrawSubUIs()
        {
            actionBlacklistEditUI.Draw();
            coneAoEAngleEditUI.Draw();
        }

        private static void CloseSubUIs()
        {
            actionBlacklistEditUI.CloseUI();
            coneAoEAngleEditUI.CloseUI();
        }

    }
}
