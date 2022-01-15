using ImGuiNET;

namespace ActionEffectRange
{
    public class ConfigUi
    {
        public static void Draw()
        {
            if (!Plugin.InConfig) return;

            ImGui.SetNextWindowSize(new(500, 400));
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
                    ImGui.NewLine();

                    ImGui.Text("Actions with large effect range: ");
                    ImGui.Indent();
                    ImGui.Combo("##LargeDrawOpt", ref Plugin.Config.LargeDrawOpt, Configuration.LargeDrawOptions, Configuration.LargeDrawOptions.Length);
                    SetTooltip($"If set to any option other than \"{Configuration.LargeDrawOptions[0]}\", " +
                        $"AoEs whose effect range is at least as large as the number specified below will be drawn (or not drawn at all) according to the set option." +
                        $"\n\nThis only applies to Circle or Donut AoEs (including Ground-targeted ones). " +
                        $"Other types of AoEs are not affected by this setting.");
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
                        DragIntWithTooltip("Thickness: ", ref Plugin.Config.Thickness, 1, 1, 50, 40, null);
                        if (Plugin.Config.Thickness < 1) Plugin.Config.Thickness = 1;
                        if (Plugin.Config.Thickness > 50) Plugin.Config.Thickness = 50;
                    }
                    ImGui.NextColumn();
                    ImGui.Checkbox("Fill colour", ref Plugin.Config.Filled);
                    if (Plugin.Config.Filled)
                    {
                        DragFloatWithTooltip("Opacity: ", ref Plugin.Config.FillAlpha, .01f, 0, 1, "%.2f", 40, null);
                        if (Plugin.Config.FillAlpha < 0) Plugin.Config.FillAlpha = 0;
                        if (Plugin.Config.FillAlpha > 1) Plugin.Config.FillAlpha = 1;
                    }
                    ImGui.Columns(1);
                    ImGui.NewLine();
                    DragIntWithTooltip("Smoothness: ", ref Plugin.Config.NumSegments, 10, 10, 1000, 100,
                        "The larger number, the smoothier");
                    if (Plugin.Config.NumSegments < 10) Plugin.Config.NumSegments = 10;
                    ImGui.TreePop();

                    ImGui.NewLine();
                    ImGui.Separator();
                    ImGui.NewLine();

                    ImGui.TreePush();
                    DragFloatWithTooltip("Delay before drawing (sec): ", ref Plugin.Config.DrawDelay, .1f, 0, 2, "%.3f", 50,
                        "Delay (in seconds) to wait immediately after using an action before drawing the effect range.");
                    DragFloatWithTooltip("Remove drawing after time (sec): ", ref Plugin.Config.PersistSeconds, .1f, .1f, 5, "%.3f", 50,
                        "Allow the effect range drawn to last for the given time (in seconds) before erased from screen.");
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

                Plugin.RefreshConfig();
            }
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
}
