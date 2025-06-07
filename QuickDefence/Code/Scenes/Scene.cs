using Controllers.Inputs;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Misc;
using Scenes.Objects;
using Scenes.Objects.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Scenes
{
    internal class Scene
    {
        public Scene()
        {
            _toAddNextFrame = new(); // Do not edit main list of objects mid-loop

            SetupDelegates();

            if(this.GetType() == typeof(Scene)) Debug.WriteLine("Scene initialized.");
        }

        ~Scene()
        {
            RemoveDelegates();
        }

        // This whole thing should totally just be handled within the ObjectManager
        // But eh, I changed the structure mid-programming and that would be effort to move
        // This works, just an unintuitive location for handling this
        void SetupDelegates()
        {
            ObjectManager.onGameObjectCreated += OnGameObjectCreatedExternally;
        }

        void RemoveDelegates()
        {
            ObjectManager.onGameObjectCreated -= OnGameObjectCreatedExternally;
        }

        public Queue<GameObject> _toAddNextFrame { get; protected set; }

        int frameNum = 0; // Used in a hack to manually clean memory that wasn't being picked up
                          // Turns out, this was caused by the range preview in tower previews which is intensive to generate and didn't properly get garbage collected without manual calling
                          // But I didn't find this out until days after finishing this and haven't gone through refactoring everything yet


        // This should be in every scene no matter what... For obvious reasons
        public virtual void GetInputs(GameTime gameTime)
        {
            MouseController.Update(deltaTime: gameTime.ElapsedGameTime.TotalSeconds);
            KeyboardController.Update(deltaTime: gameTime.ElapsedGameTime.TotalSeconds);
        }


        public virtual void Update(GameTime gameTime)
        {
            // Update Objects
            foreach(GameObject obj in ObjectManager._objects)
            {
                obj.Update(gameTime);
            }

            // Add new objects to scene
            while (_toAddNextFrame.Count > 0)
            {
                AddToScene(_toAddNextFrame.Dequeue());
            }
            _toAddNextFrame.Clear();
            frameNum++;
            if(frameNum % 3600 == 0) GC.Collect(); // Hack as fuck

            // Reorder objects for rendering
            if (Misc.ObjectManager._dirtyLayer) ObjectManager.OnDirtyLayer(); // To be honest, I have no idea if this gets called when it should, I made
                                                                              // _dirtyLayer in day 1 and didn't need it for ages, most of the time when
                                                                              // layers update I don't think I call it
        }

        // Add / remove Gameobjects from the scene
        public static void AddToScene(GameObject g)
        {
            ObjectManager._objects.Add(g);
            Misc.ObjectManager._dirtyLayer = true;
        }
        // Created to accomodate the MainGameUICreator.cs file
        public static void AddToScene(List<UIElement> UIElements)
        {
            foreach(UIElement element in UIElements)
            {
                AddToScene(element);
            }
        }
        public static void RemoveFromScene(GameObject g)
        {
            ObjectManager._objects.Remove(g);
        }

        // Should probably be in ObjectManager.cs
        private void OnGameObjectCreatedExternally(object sender, ObjectManager.OnGameObjectCreatedEventArgs e)
        {
            _toAddNextFrame.Enqueue(e.obj);
        }

        public virtual void Render(ref SpriteBatch sb, GameWindow window)
        {
            foreach(GameObject obj in ObjectManager._objects)
            {
                obj.Render(ref sb, window);
            }
        }
    }
}
