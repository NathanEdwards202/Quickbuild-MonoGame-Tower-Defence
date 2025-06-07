using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

#nullable enable

namespace Misc
{
    internal static class AssetManager
    {
        public static Dictionary<string, Texture2D> _textures = new();
        public static string? _currentMapTexture = null; // More stuff should've been handled like this, allows me to unload at the end of a scene / its use

        public static Dictionary<string, SpriteFont> _fonts = new();
        

#pragma warning disable CS8618 // VALUE SET AT THE START OF THE PROGRAM THROUGH SetContentManager(). THIS WILL NEVER BE NULL
        public static ContentManager _contentManager {  get; private set; }
        public static GraphicsDevice _graphicsDevice { get; private set; }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        // To be called once at the initialization of the game
        public static void SetContentManger(ContentManager contentManager, GraphicsDevice graphicsDevice)
        {
            _contentManager = contentManager;
            _graphicsDevice = graphicsDevice;
        }


        // Load a texture at a given path and add it to _textures, accessable via it's name (not path)
        public static void LoadAsset<T>(string textureName)
        {
            // Find string not including any '/'s
            string key = textureName;
            int finalSlash = textureName.LastIndexOf('/');
            if (finalSlash != -1) key = key[(finalSlash + 1)..];

            try
            {
                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.Object when typeof(T) == typeof(Texture2D):
                        _textures.Add(key, _contentManager.Load<Texture2D>(textureName));
                        break;
                    case TypeCode.Object when typeof(T) == typeof(SpriteFont):
                        _fonts.Add(key, _contentManager.Load<SpriteFont>(textureName));
                        break;
                    default:
                        throw new Exception("Unsupported type.");
                }

                Debug.WriteLine($"Texture {key} loaded");
            }
            catch
            {
                throw new Exception($"Failed to load texture: {key}");
            }
        }

        // Stolen (and then further edited) from Stackoverflow, that's why this is different to LoadAsset<T>
        public static void LoadAssetsFromFile<T>(string folderName)
        {
            //Load directory info, abort if none
            DirectoryInfo dir = new(_contentManager.RootDirectory + "\\" + folderName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException($"Failed to find directory: {folderName}");
            }

            //Load all files that matches the file filter
            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo file in files)
            {
                string key = Path.GetFileNameWithoutExtension(file.Name);
                
                switch (Type.GetTypeCode(typeof(T)))
                {
                    case TypeCode.Object when typeof(T) == typeof(Texture2D):
                        _textures.Add(key, _contentManager.Load<Texture2D>(folderName + "/" + key));
                        break;
                    case TypeCode.Object when typeof(T) == typeof(SpriteFont):
                        _fonts.Add(key, _contentManager.Load<SpriteFont>(folderName + "/" + key));
                        break;
                    default:
                        throw new Exception("Unsupported type.");
                }

                Debug.WriteLine($"Texture {key} loaded");
            }
        }

        public static Texture2D GetTexture(string textureName)
        {
            return _textures[textureName];
        }

        public static SpriteFont GetFont(string fontName)
        {
            return _fonts[fontName];
        }


        public static void OnLoadLevel(string mapName)
        {
            if (_currentMapTexture != null)
            {
                _textures.Remove(_currentMapTexture);
            }

            LoadAsset<Texture2D>("Overworld/BackgroundMaps/" + mapName);
            _currentMapTexture = mapName;
        }


        // Helper functions
        // THIS FUCKER
        // This uses so much memory
        // Someone tell me how to make this better I swear, the only thing I can think is to make them a bit smaller then stretch them
        // But dear god the memory usage
        // (Disregard the previous comments, they were written before I just uh... Parallelized it. Still not good, but not god-awful)
        public static Texture2D CreateCircleTexture(int radius)
        {
            Texture2D texture = new (_graphicsDevice, radius * 2, radius * 2);
            Color[] colorData = new Color[radius * 2 * radius * 2];

            System.Threading.Tasks.Parallel.For(0, radius * 2, y =>
            {
                for (int x = 0; x < radius * 2; x++)
                {
                    int dx = x - radius;
                    int dy = y - radius;

                    if (dx * dx + dy * dy <= radius * radius)
                    {
                        colorData[y * radius * 2 + x] = Color.White;
                    }
                    else
                    {
                        colorData[y * radius * 2 + x] = Color.Transparent;
                    }
                }
            });

            texture.SetData(colorData);
            return texture;
        }
    }
}