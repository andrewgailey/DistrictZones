using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ColossalFramework.UI;
using System.Collections;
using ICities;

namespace DistrictZones
{
    class DistrictZonesUI
    {

        private const String BUTTON_NAME = "DistrictZonesButton";
        private const String PANEL_NAME = "DistrictZonesPanel";

        public AppMode appMode { get; set; }
        UIMultiStateButton button;
        UIPanel panel;
        UIView view;
        GameObject newGO;
        Vector3 buttonPos = new Vector3(-38, 0);
        Vector3 buttonSize = new Vector3(36, 36);
        Vector3 panelPos = new Vector3(340, 947);
        float panelWidth = 40;

        public DistrictZonesUI()
        {
        }

        public void CreateUI()
        {

            // Delete the old button, panel, and object if they're there.
            UIMultiStateButton oldButton = UIUtils.Find<UIMultiStateButton>(BUTTON_NAME);
            if (oldButton != null)
            {
                GameObject.Destroy(oldButton);
            }
            UIPanel oldPanel = UIUtils.Find<UIPanel>(PANEL_NAME);
            if (oldPanel != null)
            {
                GameObject.Destroy(oldPanel);
            }

            UIComponent roadsOptionPanel = UIUtils.Find<UIComponent>("DistrictOptionPanel");
            if (roadsOptionPanel == null)
            {
                // Listen for districts tab if it hasn't been added yet
                UITabstrip mainToolStrip = UIUtils.Find<UITabstrip>("MainToolstrip");
                if (mainToolStrip == null)
                {
                    CitiesConsole.Warning("Could not find MainToolstrip");
                    return;
                }
                mainToolStrip.eventSelectedIndexChanged += mainToolStrip_eventSelectedIndexChanged;
            }
            else
            {
                CreateButton(roadsOptionPanel);
            }
        }        

        private void mainToolStrip_eventSelectedIndexChanged(UIComponent component, int value)
        {
            // tab[2] is the districts tab
            if (value == 2)
            {
                // delay until districts tab is added
                component.StartCoroutine(CreateButtonCoroutine((UITabstrip)component));
            }
        }

        private IEnumerator CreateButtonCoroutine(UITabstrip tabstrip)
        {
            // This will delay the execution of this coroutine.
            yield return null;

            if (CreateButton(null))
            {
                tabstrip.eventSelectedIndexChanged -= mainToolStrip_eventSelectedIndexChanged;
            }
        }

        private bool CreateButton(UIComponent districtOptionPanel)
        {
            String[] iconNames = {
                "RoadArrowIcon",
                "Base",
                "BaseFocused",
                "BaseHovered",
                "BasePressed",
                "BaseDisabled",
                                 };

            // Create Hierarchy
            view = UIView.GetAView();
            panel = (UIPanel)view.AddUIComponent(typeof(UIPanel));
            newGO = new GameObject(BUTTON_NAME);
            button = newGO.AddComponent<UIMultiStateButton>();
            panel.AttachUIComponent(newGO);

            // Panel attributes
            panel.relativePosition = panelPos;
            panel.width = panelWidth;
            panel.name = PANEL_NAME;
            panel.isVisible = false;

            // Button attributes
            button.relativePosition = buttonPos;
            button.size = buttonSize;
            button.playAudioEvents = true;
            button.name = BUTTON_NAME;
            button.tooltip = "Display Zones";
            button.isTooltipLocalized = false;
            button.spritePadding = new RectOffset();
            button.activeStateIndex = 1;

            button.atlas = CreateTextureAtlas("icons.png", BUTTON_NAME + "Atlas", panel.atlas.material, 36, 36, iconNames);

            UIMultiStateButton.SpriteSet backgroundSpriteSet0 = button.backgroundSprites[0];
            backgroundSpriteSet0.normal = "Base";
            backgroundSpriteSet0.disabled = "Base";
            backgroundSpriteSet0.hovered = "BaseHovered";
            backgroundSpriteSet0.pressed = "Base";
            backgroundSpriteSet0.focused = "Base";

            button.backgroundSprites.AddState();
            UIMultiStateButton.SpriteSet backgroundSpriteSet1 = button.backgroundSprites[1];
            backgroundSpriteSet1.normal = "BaseFocused";
            backgroundSpriteSet1.disabled = "BaseFocused";
            backgroundSpriteSet1.hovered = "BaseFocused";
            backgroundSpriteSet1.pressed = "BaseFocused";
            backgroundSpriteSet1.focused = "BaseFocused";

            UIMultiStateButton.SpriteSet foregroundSpriteSet0 = button.foregroundSprites[0];
            foregroundSpriteSet0.normal = "RoadArrowIcon";
            foregroundSpriteSet0.disabled = "RoadArrowIcon";
            foregroundSpriteSet0.hovered = "RoadArrowIcon";
            foregroundSpriteSet0.pressed = "RoadArrowIcon";
            foregroundSpriteSet0.focused = "RoadArrowIcon";

            button.foregroundSprites.AddState();
            UIMultiStateButton.SpriteSet foregroundSpriteSet1 = button.foregroundSprites[1];
            foregroundSpriteSet1.normal = "RoadArrowIcon";
            foregroundSpriteSet1.disabled = "RoadArrowIcon";
            foregroundSpriteSet1.hovered = "RoadArrowIcon";
            foregroundSpriteSet1.pressed = "RoadArrowIcon";
            foregroundSpriteSet1.focused = "RoadArrowIcon";

            // Districts Panel Listener
            districtOptionPanel.eventVisibilityChanged += districtOptionPanel_eventVisibilityChanged;

            // Button Listener
            button.eventActiveStateIndexChanged += button_eventActiveStateIndexChanged;

            return true;
        }

        private void button_eventActiveStateIndexChanged(UIComponent component, int value)
        {
            if (value == 1)
            {
                TerrainManager.instance.RenderZones = true;
            }
            else
            {
                TerrainManager.instance.RenderZones = false;
            }
        }

        private void districtOptionPanel_eventVisibilityChanged(UIComponent component, bool value) {
            panel.isVisible = value;
            if (value)
            {
                panel.isVisible = true;
                if (button.activeStateIndex > 0)
                {
                    TerrainManager.instance.RenderZones = true;
                }
            }
            else
            {
                panel.isVisible = false;
                TerrainManager.instance.RenderZones = false;
            }
        }

        private static UITextureAtlas CreateTextureAtlas(string textureFile, string atlasName, Material baseMaterial, int spriteWidth, int spriteHeight, string[] spriteNames)
        {

            Texture2D texture = new Texture2D(spriteWidth * spriteNames.Length, spriteHeight, TextureFormat.ARGB32, false);
            texture.filterMode = FilterMode.Bilinear;

            { // LoadTexture
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                System.IO.Stream textureStream = assembly.GetManifestResourceStream("DistrictZones." + textureFile);

                byte[] buf = new byte[textureStream.Length];  //declare arraysize
                textureStream.Read(buf, 0, buf.Length); // read from stream to byte array

                texture.LoadImage(buf);

                texture.Apply(true, true);
            }

            UITextureAtlas atlas = ScriptableObject.CreateInstance<UITextureAtlas>();

            { // Setup atlas
                Material material = (Material)Material.Instantiate(baseMaterial);
                material.mainTexture = texture;

                atlas.material = material;
                atlas.name = atlasName;
            }

            // Add sprites
            for (int i = 0; i < spriteNames.Length; ++i)
            {
                float uw = 1.0f / spriteNames.Length;

                var spriteInfo = new UITextureAtlas.SpriteInfo()
                {
                    name = spriteNames[i],
                    texture = texture,
                    region = new Rect(i * uw, 0, uw, 1),
                };

                atlas.AddSprite(spriteInfo);
            }

            return atlas;
        }
    }
}
