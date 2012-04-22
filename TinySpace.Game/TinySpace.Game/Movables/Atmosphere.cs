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
using Microsoft.Xna.Framework.Content;
using TinySpace.Core;

namespace TinySpace.Game.Movables
{
	/// <summary>
	/// Renders a simple atmosphere.
	/// </summary>
	public class Atmosphere : Movable, IRenderable
	{
		#region Constructors

		public Atmosphere(float radius, string name = null)
			: base(name)
		{
			this.Radius = radius;
			m_rasterizerState = new RasterizerState
			{
				CullMode = CullMode.None,
				FillMode = FillMode.Solid
			};
		}

		#endregion

		#region Fields

		private GraphicsDevice m_graphics;
		private RasterizerState m_rasterizerState;

		private BasicEffect m_basicEffect;
		private Texture2D m_gradient;

		private VertexBuffer m_vertexBuffer;
		private IndexBuffer m_indexBuffer;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the radius.
		/// </summary>
		public float Radius
		{
			get;
			private set;
		}

		#endregion

		#region IRenderable Members

		public void LoadContent(IServiceProvider services)
		{
			// Get the necessary services.
			m_graphics = ((IGraphicsDeviceService)services.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;
			var content = (ContentManager)services.GetService(typeof(ContentManager));

			// Load the gradient texture.
			m_gradient = content.Load<Texture2D>(@"Textures\Atmosphere");

			// Create the effect.
			m_basicEffect = new BasicEffect(m_graphics);
			m_basicEffect.DiffuseColor = Vector3.One;
			m_basicEffect.Texture = m_gradient;
			m_basicEffect.TextureEnabled = true;

			// Create the buffers.
			var vertices = new List<VertexPositionTexture>();
			var indices = new List<ushort>();

			int sectionCount = 64;

			vertices.Add(new VertexPositionTexture
			{
				Position = Vector3.Zero,
				TextureCoordinate = Vector2.Zero
			});

			for (int i = 0; i < sectionCount; ++i)
			{
				float step = 360.0f / (float)sectionCount * i;

				vertices.Add(new VertexPositionTexture
				{
					Position = new Vector3(MathCore.Cos(step) * this.Radius, MathCore.Sin(step) * this.Radius, 0.0f),
					TextureCoordinate = Vector2.One * 0.99f
				});
			}

			for (int i = 1; i < sectionCount - 1; ++i)
			{
				indices.Add(0);
				indices.Add((ushort)i);
				indices.Add((ushort)(i + 1));
			}

			indices.Add(0);
			indices.Add((ushort)(sectionCount - 1));
			indices.Add((ushort)1);

			if (vertices.Count > 0 && indices.Count > 0)
			{
				m_vertexBuffer = new VertexBuffer(m_graphics, VertexPositionTexture.VertexDeclaration, vertices.Count, BufferUsage.None);
				m_vertexBuffer.SetData(vertices.ToArray());

				m_indexBuffer = new IndexBuffer(m_graphics, IndexElementSize.SixteenBits, indices.Count, BufferUsage.None);
				m_indexBuffer.SetData(indices.ToArray());
			}
		}

		public void UnloadContent()
		{
			if (m_vertexBuffer != null)
			{
				m_vertexBuffer.Dispose();
				m_vertexBuffer = null;
			}
			if (m_indexBuffer != null)
			{
				m_indexBuffer.Dispose();
				m_indexBuffer = null;
			}

			m_basicEffect.Dispose();
			m_gradient.Dispose();
		}

		public void Render(GameTime gameTime, ICamera camera)
		{
			if (m_vertexBuffer != null && m_indexBuffer != null && m_basicEffect != null)
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
				m_graphics.SetVertexBuffer(m_vertexBuffer);
				m_graphics.Indices = m_indexBuffer;

				foreach (var effectPass in m_basicEffect.CurrentTechnique.Passes)
				{
					effectPass.Apply();
					m_graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_vertexBuffer.VertexCount, 0, m_indexBuffer.IndexCount / 3);
				}
			}
		}

		#endregion
	}
}
