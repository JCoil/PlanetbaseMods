using Planetbase;
using PlanetbaseModUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

namespace StorageGuru
{
    internal static class ContentManager
    {
        static string EnableIconFilePath = @"Mods\StorageGuruData\Textures\StorageEnable.png";
        static string DisableIconFilePath = @"Mods\StorageGuruData\Textures\StorageDisable.png";
        public static Texture2D EnableAllIcon { get; private set; }
        public static Texture2D DisableAllIcon { get; private set; }

        private const float greyscaleBrightness = 180f / 255f;

        public static Dictionary<string, Texture2D> GreyscaleTextures; 

        public static void Init()
        {
            StringUtils.RegisterString("tooltip_manage_storage", "Manage Storage");
            GreyscaleTextures = new Dictionary<string, Texture2D>(); 
            LoadContent();
        }

        internal static void LoadContent()
        {
            EnableIconFilePath = Path.Combine(Util.getFilesFolder(), EnableIconFilePath);
            DisableIconFilePath = Path.Combine(Util.getFilesFolder(), DisableIconFilePath);

            EnableAllIcon = LoadTexture(EnableIconFilePath);
            DisableAllIcon = LoadTexture(DisableIconFilePath);

            if(EnableAllIcon == null || DisableAllIcon == null)
            {
                Debug.Log("[StorageGuru] Failed to load icons");
            }
        }

        private static Texture2D LoadTexture(string filepath)
        {
            if (File.Exists(filepath))
            {
                byte[] iconBytes = File.ReadAllBytes(filepath);
                Texture2D tex = new Texture2D(0, 0);
                tex.LoadImage(iconBytes);
                return Util.applyColor(tex);
            }

            return new Texture2D(1, 1);
        }

        public static void CreateAlternativeIcons(List<ResourceType> resourceTypes)
        {
            GreyscaleTextures = new Dictionary<string, Texture2D>(); 

            foreach (var resourceType in resourceTypes)
            {
                GreyscaleTextures.Add(resourceType.getName(), ApplyGreyscaleColorFix(resourceType.getIcon())); 
            }
        }

        private static Texture2D ApplyGreyscaleColorFix(Texture2D texture)
        {
            var pixels = texture.GetPixels();
            var greyscalePixels = pixels.Select(p => new Color(greyscaleBrightness, greyscaleBrightness, greyscaleBrightness, p.a)).ToArray();
            texture.SetPixels(greyscalePixels);
            return Util.applyColor(texture); // Apply standard Gui color
        }
    }
}
