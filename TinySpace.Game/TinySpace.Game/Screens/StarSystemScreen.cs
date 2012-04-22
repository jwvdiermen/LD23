/*
 *  TinySpace by Crealuz - TinySpace.Game
 * 
 *  @original authors: jwvdiermen
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using TinySpace.State;
using TinySpace.Sprite;
using TinySpace.Scene;
using TinySpace.Entity;
using FarseerPhysics.DebugViews;

namespace TinySpace.Game.Screens
{
	/// <summary>
	/// This screen shows the level.
	/// </summary>
	public class StarSystemScreen : IScreen
	{
		#region Constructors

		/// <summary>
		/// Public constructor.
		/// </summary>
		public StarSystemScreen(XnaGame game)
		{
			m_game = game;
			m_debug = new Debug(m_game);
			m_content = m_game.Content;
			m_graphics = m_game.GraphicsDevice;
		}

		#endregion

		#region Fields

		private XnaGame m_game;
		private Debug m_debug;
		private ContentManager m_content;
		private GraphicsDevice m_graphics;

		private ICamera m_camera;
		private ISceneNode m_cameraNode;

		private Vector2 m_worldSize = new Vector2(5000.0f, 5000.0f);
		private DynamicEntityWorld m_world;
		private DebugViewXNA m_debugView;

		private DefaultLayer m_playerLayer;
		private Entities.Player m_player;

		private DefaultLayer m_backgroundLayer;
		private ISceneNode m_deepSpaceNode;
		private ISceneNode m_lessDeepSpaceNode;

		private ISceneNode m_dustCloudNode;

		private DefaultLayer m_terrainLayer;
		private Entities.Terrain m_planet;

		private DefaultLayer m_miscLayer;

		#endregion

		#region IScreen Members

		public bool IsPopup
		{
			get { return false; }
		}

		public void LoadContent()
		{
			m_debug.LoadContent();

			var pp = m_graphics.PresentationParameters;

			// Create our entity world.
			m_world = new DynamicEntityWorld(m_game.Services);

			// Create the debug view.
			m_debugView = new DebugViewXNA(m_world.PhysicsWorld);
			m_debugView.LoadContent(m_graphics, m_content);			
			
			// Create the camera.
			m_camera = new DefaultCamera(null, new Viewport(0, 0, pp.BackBufferWidth, pp.BackBufferHeight));
			m_cameraNode = m_world.Scene.Root.CreateChild("Camera");
			m_cameraNode.Attach(m_camera);

			// Create the player.
			m_player = new Entities.Player(
				new StaticSprite(Vector2.One * 10.0f, m_content.Load<Texture2D>(@"Sprites\Ship01"), "Player"));

			m_player.Attach(new Controllers.PlayerMovement(m_camera));
			m_world.Add(m_player);

			m_player.Position = new Vector2(0.0f, 0.0f);

			// Create the layers.
			m_playerLayer = new DefaultLayer(typeof(ISprite), "Player", 0.5f) { Scene = m_world.Scene };
			m_backgroundLayer = new DefaultLayer(0.0f) { Scene = m_world.Scene };

			// Create the background.
			var deepSpaceSprite = new StaticSprite(Vector2.One * 200.0f, m_content.Load<Texture2D>(@"Textures\Background01"), null) { BlendState = BlendState.Additive };
			var lessDeepSpaceSprite = new StaticSprite(Vector2.One * 400.0f, m_content.Load<Texture2D>(@"Textures\Background02"), null) { BlendState = BlendState.Additive };

			m_deepSpaceNode = m_cameraNode.CreateChild("Background_DeepSpace");
			m_deepSpaceNode.Depth = -1.0f; // Move the background into the back.
			m_deepSpaceNode.Attach(deepSpaceSprite);

			m_lessDeepSpaceNode = m_cameraNode.CreateChild("Background_LessDeepSpace");
			m_lessDeepSpaceNode.Depth = -0.9f; // Move the background into the back, but in front of the other background.
			m_lessDeepSpaceNode.Attach(lessDeepSpaceSprite);

			m_backgroundLayer.AddRenderable(deepSpaceSprite);
			m_backgroundLayer.AddRenderable(lessDeepSpaceSprite);

			// Create the dust cloud.
			var dustCloud = new Movables.DustCloud(100);

			m_dustCloudNode = m_world.Scene.Root.CreateChild("DustCloud");
			m_dustCloudNode.Depth = -0.8f;
			m_dustCloudNode.Attach(dustCloud);

			m_backgroundLayer.AddRenderable(dustCloud);

			// Create the planet.
			m_terrainLayer = new DefaultLayer(typeof(Entities.TerrainRegion), null, 0.0f) { Scene = m_world.Scene };
			m_miscLayer = new DefaultLayer(typeof(ISprite), "MISC_", -0.5f) { Scene = m_world.Scene };

			CreatePlanet();
		}

		private void CreatePlanet()
		{
			var seed = 42601423;

			var fastNoise01 = new LibNoise.FastNoise(seed);
			fastNoise01.Frequency = 0.1;
			fastNoise01.OctaveCount = 2;

			var fastNoise02 = new LibNoise.FastNoise(seed);
			fastNoise02.Frequency = 0.25;
			fastNoise02.OctaveCount = 4;

			var noiseModule =
				new LibNoise.Modifiers.Add(
					new LibNoise.Modifiers.ScaleOutput(fastNoise01, 0.3),
					new LibNoise.Modifiers.ScaleOutput(fastNoise02, 0.05));

			// Create a new planet.
			int terrainWidth = 1000;
			int terrainHeight = 1000;

			var terrainData = new sbyte[terrainWidth + 1, terrainHeight + 1];

			for (int x = 0; x < terrainWidth; ++x)
			{
				for (int y = 0; y < terrainHeight; ++y)
				{
					var position = new Vector2(x, y);
					var center = new Vector2(terrainWidth / 2, terrainHeight / 2);
					var distance = (position - center).Length();

					var noise = (float)noiseModule.GetValue((double)(position.X / 5.0f), (double)(position.Y / 5.0f), 10.0);
					var density = (1.2f / 500.0f * distance) - 1.0f - ((noise + 1.5f) * 0.1f);

					terrainData[x, y] =  (sbyte)(density > 0.0f ? 1 : -1);
				}
			}

			m_planet = new Entities.Terrain(Vector2.One * 100.0f, new Entities.MemoryTerrainSource(terrainData, 1000), "Terrain");
			m_planet.Position = new Vector2(0.0f, -100.0f);

			m_world.Add(m_planet);

			var atmosphere = new Movables.Atmosphere(70.0f);
			m_planet.SceneNode.Attach(atmosphere);
			m_miscLayer.AddRenderable(atmosphere);
		}

		public void UnloadContent()
		{
			m_debug.UnloadContent();

			m_debugView.Dispose();

			m_playerLayer.Dispose();
			m_terrainLayer.Dispose();
			m_backgroundLayer.Dispose();
			m_miscLayer.Dispose();

			m_world.Dispose();
		}

		public void Update(GameTime gameTime, bool isActive, bool isOverlayed)
		{
			m_world.Update(gameTime, m_camera);

			// Track our player.
			var cameraPosition = m_player.Position;

			var halfWorldSize = m_worldSize / 2.0f;

			var halfScreenSize = m_camera.ScreenSize / 2.0f;
			halfScreenSize = new Vector2(halfScreenSize.X / m_cameraNode.Scale.X, halfScreenSize.Y / m_cameraNode.Scale.Y);

			var topLeft = new Vector2(-halfWorldSize.X + halfScreenSize.X, -halfWorldSize.Y + halfScreenSize.Y);
			var bottomRight = new Vector2(halfWorldSize.X - halfScreenSize.X, halfWorldSize.Y - halfScreenSize.X);

			if (cameraPosition.X < topLeft.X)
			{
				cameraPosition.X = topLeft.X;
			}
			else if (cameraPosition.X > bottomRight.X)
			{
				cameraPosition.X = bottomRight.X;
			}

			if (cameraPosition.Y < topLeft.Y)
			{
				cameraPosition.Y = topLeft.Y;
			}
			else if (cameraPosition.Y > bottomRight.Y)
			{
				cameraPosition.Y = bottomRight.Y;
			}

			m_cameraNode.Position = cameraPosition;

			// Move the background based on the camera's position.
			m_deepSpaceNode.LocalPosition = (new Vector2(-100.0f, -100.0f) + halfScreenSize) / halfWorldSize * cameraPosition;
			m_lessDeepSpaceNode.LocalPosition = (new Vector2(-200.0f, -200.0f) + halfScreenSize) / halfWorldSize * cameraPosition;
		}

		public void Draw(GameTime gameTime)
		{
			// Clear our screen texture.
			m_camera.Apply(m_graphics);
			m_graphics.Clear(Color.Black);

			// TODO: rework the layer system, takes to much work now.
			// Draw the background layer.
			foreach (var renderable in m_backgroundLayer.Renderables)
			{
				renderable.Render(gameTime, m_camera);
			}

			// Draw the misc layer.
			foreach (var renderable in m_miscLayer.Renderables)
			{
				renderable.Render(gameTime, m_camera);
			}

			// Draw the terrain layer.
			foreach (var renderable in m_terrainLayer.Renderables)
			{
				renderable.Render(gameTime, m_camera);
			}

			// Draw the player layer.
			foreach (var renderable in m_playerLayer.Renderables)
			{
				renderable.Render(gameTime, m_camera);
			}

			//Matrix projectionMatrix, viewMatrix;
			//m_camera.GetMatrices(out projectionMatrix, out viewMatrix);
			//m_debugView.RenderDebugData(ref projectionMatrix, ref viewMatrix);

			// Draw the debug information.
			//m_debug.Draw();
		}

		#endregion
	}
}
