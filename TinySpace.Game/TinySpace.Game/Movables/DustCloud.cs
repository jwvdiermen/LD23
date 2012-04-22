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
using Microsoft.Xna.Framework.Graphics;

using TinySpace.Scene;
using TinySpace.Core;

namespace TinySpace.Game.Movables
{
	/// <summary>
	/// A dust cloud entity for making the player's movement more visible.
	/// </summary>
	public class DustCloud : Movable, IUpdatable, IRenderable
	{
		#region Nested types

		private struct Particle
		{
			public Vector2 Position;
			public Color Color;
		}

		#endregion

		#region Constructors

		public DustCloud(int size = 100, string name = "DustCloud")
			: base(name)
		{
			m_particles = new Particle[size];
			m_particleVertices = new VertexPositionColor[size * 3];

			m_rasterizerState = new RasterizerState
			{
				CullMode = CullMode.None,
				FillMode = FillMode.Solid
			};
		}

		#endregion

		#region Fields

		private FastRandom m_random = new FastRandom();

		private GraphicsDevice m_graphics;
		private BasicEffect m_basicEffect;
		private RasterizerState m_rasterizerState;

		private float m_particleSize;
		private float m_halfParticleSize;

		private Vector2 m_lastCameraPosition;

		private bool m_hasInitialized = false;

		private Particle[] m_particles;
		private VertexPositionColor[] m_particleVertices;

		#endregion

		#region Methods

		private void Initialize(ICamera camera)
		{
			// Retrieve the necessary data.
			var cameraPosition = camera.SceneNode.Position;
			var halfScreenSize = camera.ScreenSize / 2.0f;

			m_lastCameraPosition = cameraPosition;

			// Use the viewport's and screen's height to determine our particle size since it's contstant.
			// Particle size = 2 pixels.
			m_particleSize = camera.ScreenSize.Y / (float)camera.Viewport.Height * 2.0f;
			m_halfParticleSize = m_particleSize / 2.0f;

			// Initialize the particles.
			for (int i = 0; i < m_particles.Length; ++i)
			{
				var particle = new Particle
				{
					Position = new Vector2(
						cameraPosition.X + halfScreenSize.X * m_random.NextRangedFloat(),
						cameraPosition.Y + halfScreenSize.Y * m_random.NextRangedFloat()),
					Color = new Color(
						1.0f - m_random.NextFloat() * 0.2f,
						1.0f - m_random.NextFloat() * 0.2f,
						1.0f - m_random.NextFloat() * 0.2f,
						1.0f - m_random.NextFloat() * 0.2f)
				};

				m_particles[i] = particle;
				RebuildPartice(i, ref particle);
			}
			m_hasInitialized = true;
		}

		private void RebuildPartice(int index, ref Particle particle)
		{
			index = index * 3;

			m_particleVertices[index + 0] = new VertexPositionColor
			{
				Position = new Vector3(particle.Position.X, particle.Position.Y - m_halfParticleSize, 0.0f),
				Color = particle.Color
			};
			m_particleVertices[index + 1] = new VertexPositionColor
			{
				Position = new Vector3(particle.Position.X - m_halfParticleSize, particle.Position.Y + m_halfParticleSize, 0.0f),
				Color = particle.Color
			};
			m_particleVertices[index + 2] = new VertexPositionColor
			{
				Position = new Vector3(particle.Position.X + m_halfParticleSize, particle.Position.Y + m_halfParticleSize, 0.0f),
				Color = particle.Color
			};
		}

		#endregion

		#region IUpdatable Members

		public void Update(GameTime time, ICamera camera)
		{
			// Check if we need to initialize.
			if (m_hasInitialized == false)
			{
				Initialize(camera);
			}

			// Retrieve the necessary data.
			var cameraPosition = camera.SceneNode.Position;
			var halfScreenSize = camera.ScreenSize / 2.0f;

			var cameraDirection = cameraPosition - m_lastCameraPosition;

			var topLeft = cameraPosition - halfScreenSize;
			var bottomRight = cameraPosition + halfScreenSize;

			// Find any particles that moved of the screen.
			for (int i = 0; i < m_particles.Length; ++i)
			{
				var particle = m_particles[i];
				var position = particle.Position;
				
				// Check if and how the particle left the viewport.
				bool moved = false;
				if (position.X < topLeft.X && cameraDirection.X > 0.0f)
				{
					position = new Vector2(
						cameraPosition.X + halfScreenSize.X + (m_random.NextFloat() * 20.0f),
						cameraPosition.Y + (halfScreenSize.Y + 40.0f) * m_random.NextRangedFloat());

					moved = true;
				}
				else if (position.X > bottomRight.X && cameraDirection.X < 0.0f)
				{
					position = new Vector2(
						cameraPosition.X - halfScreenSize.X - (m_random.NextFloat() * 20.0f),
						cameraPosition.Y + (halfScreenSize.Y + 40.0f) * m_random.NextRangedFloat());

					moved = true;
				}

				if (position.Y < topLeft.Y && cameraDirection.Y > 0.0f)
				{
					position = new Vector2(
						cameraPosition.X + (halfScreenSize.X + 40.0f) * m_random.NextRangedFloat(),
						cameraPosition.Y + halfScreenSize.Y + (m_random.NextFloat() * 20.0f));

					moved = true;
				}
				else if (position.Y > bottomRight.Y && cameraDirection.Y < 0.0f)
				{
					position = new Vector2(
						cameraPosition.X + (halfScreenSize.X + 40.0f) * m_random.NextRangedFloat(),
						cameraPosition.Y - halfScreenSize.Y - (m_random.NextFloat() * 20.0f));

					moved = true;
				}

				if (moved)
				{
					particle = new Particle
					{
						Position = position,
						Color = new Color(
							1.0f - m_random.NextFloat() * 0.2f,
							1.0f - m_random.NextFloat() * 0.2f,
							1.0f - m_random.NextFloat() * 0.2f,
							1.0f - m_random.NextFloat() * 0.2f)
					};
					m_particles[i] = particle;

					RebuildPartice(i, ref particle);
				}
			} 
			
			m_lastCameraPosition = cameraPosition;
		}

		#endregion

		#region IRenderable Members

		public void LoadContent(IServiceProvider services)
		{
			// Get the necessary services.
			m_graphics = ((IGraphicsDeviceService)services.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;

			// Create the effect.
			m_basicEffect = new BasicEffect(m_graphics);
			m_basicEffect.DiffuseColor = Vector3.One;
			m_basicEffect.VertexColorEnabled = true;
		}

		public void UnloadContent()
		{
			if (m_basicEffect != null)
			{
				m_basicEffect.Dispose();
				m_basicEffect = null;
			}
		}

		public void Render(GameTime gameTime, ICamera camera)
		{
			if (m_basicEffect != null)
			{
				// Set the effect's parameters.
				Matrix projectionMatrix, viewMatrix;
				camera.GetMatrices(out projectionMatrix, out viewMatrix);

				m_basicEffect.Projection = projectionMatrix;
				m_basicEffect.View = viewMatrix;
				m_basicEffect.World =
					Matrix.CreateRotationZ(MathHelper.ToRadians(this.SceneNode.Rotation)) *
					Matrix.CreateTranslation(new Vector3(this.SceneNode.Position, this.SceneNode.Depth)) *
					Matrix.CreateScale(new Vector3(this.SceneNode.Scale, 1.0f));

				// Set the rasterizer state.
				m_graphics.BlendState = BlendState.AlphaBlend;
				m_graphics.RasterizerState = m_rasterizerState;

				// Draw the vertex buffer.
				foreach (var effectPass in m_basicEffect.CurrentTechnique.Passes)
				{
					effectPass.Apply();
					m_graphics.DrawUserPrimitives(PrimitiveType.TriangleList, m_particleVertices, 0, m_particles.Length, VertexPositionColor.VertexDeclaration);
				}
			}
		}

		#endregion
	}
}
