/*
 *  TinySpace by Crealuz - TinySpace.Sprite
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

namespace TinySpace.Sprite
{
	/// <summary>
	/// This class implements the <see cref="IStaticSprite" /> and provides default functionality.
	/// </summary>
	public class StaticSprite : Movable, IStaticSprite
	{
		#region Constructors

		/// <summary>
		/// Creates a new static sprite with the pivot in the center.
		/// </summary>
		/// <param name="size">The size of the sprite.</param>
		/// <param name="texture">The texture.</param>
		/// <param name="name">The name.</param>
		/// <param name="source">The source.</param>
		public StaticSprite(Vector2 size, Texture2D texture, string name = null, Rectangle? source = null)
			: this(size, size / 2.0f, texture, name, source)
		{			
		}

		/// <summary>
		/// Creates a new static sprite.
		/// </summary>
		/// <param name="size">The size of the sprite.</param>
		/// <param name="pivot">The pivot of the sprite.</param>
		/// <param name="texture">The texture.</param>
		/// <param name="name">The name.</param>
		/// <param name="source">The source.</param>
		public StaticSprite(Vector2 size, Vector2 pivot, Texture2D texture, string name = null, Rectangle? source = null)
			: base(name)
		{
			this.Size = size;
			this.Pivot = pivot;
			this.Texture = texture;
			this.Source = source != null ? (Rectangle)source : new Rectangle(0, 0, texture.Width, texture.Height);

			m_rasterizerState = new RasterizerState
			{
				CullMode = CullMode.None,
				FillMode = FillMode.Solid
			};

			this.BlendState = BlendState.AlphaBlend;
		}

		#endregion

		#region ISprite Members

		public SpriteEffects Effects
		{
			get;
			set;
		}

		public BlendState BlendState
		{
			get;
			set;
		}

		#endregion

		#region Fields

		private GraphicsDevice m_graphics;
		private RasterizerState m_rasterizerState;

		private VertexBuffer m_vertexBuffer;
		private IndexBuffer m_indexBuffer;

		private BasicEffect m_basicEffect;

		#endregion

		#region IStaticSprite Members

		public Vector2 Size
		{
			get;
			private set;
		}

		public Vector2 Pivot
		{
			get;
			private set;
		}

		public Texture2D Texture
		{
			get;
			private set;
		}

		public Rectangle Source
		{
			get;
			private set;
		}

		#endregion

		#region IUpdatable Members

		public void Update(GameTime time, ICamera camera)
		{
		}

		#endregion

		#region IRenderable Members

		public void LoadContent(IServiceProvider services)
		{
			// Get the necessary services.
			m_graphics = ((IGraphicsDeviceService)services.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;

			// Determine the texture coordinates.
			var sx = 1.0f / this.Texture.Width;
			var sy = 1.0f / this.Texture.Height;

			var uvTopLeft = new Vector2(sx * this.Source.Left, sy * this.Source.Top);
			var uvBottomRight = new Vector2(sx * this.Source.Right, sy * this.Source.Bottom);

			// Generate the vertex buffer.
			var topLeft = -this.Pivot;
			var vertices = new VertexPositionColorTexture[]
			{
				new VertexPositionColorTexture { Position = new Vector3(topLeft.X, topLeft.Y, 0.0f), TextureCoordinate = new Vector2(uvTopLeft.X, uvTopLeft.Y), Color = Color.White },
				new VertexPositionColorTexture { Position = new Vector3(topLeft.X + this.Size.X, topLeft.Y, 0.0f), TextureCoordinate = new Vector2(uvBottomRight.X, uvTopLeft.Y), Color = Color.White },
				new VertexPositionColorTexture { Position = new Vector3(topLeft.X + this.Size.X, topLeft.Y + this.Size.Y, 0.0f), TextureCoordinate = new Vector2(uvBottomRight.X, uvBottomRight.Y), Color = Color.White },
				new VertexPositionColorTexture { Position = new Vector3(topLeft.X, topLeft.Y + this.Size.Y, 0.0f), TextureCoordinate = new Vector2(uvTopLeft.X, uvBottomRight.Y), Color = Color.White }
			};

			m_vertexBuffer = new VertexBuffer(m_graphics, VertexPositionColorTexture.VertexDeclaration, vertices.Length, BufferUsage.None);
			m_vertexBuffer.SetData(vertices);

			// Generate the index buffer.
			var indices = new ushort[] { 0, 3, 1, 1, 3, 2 };

			m_indexBuffer = new IndexBuffer(m_graphics, IndexElementSize.SixteenBits, indices.Length, BufferUsage.None);
			m_indexBuffer.SetData(indices);

			// Create the effect.
			m_basicEffect = new BasicEffect(m_graphics);
			m_basicEffect.Texture = this.Texture;
			m_basicEffect.TextureEnabled = true;
			m_basicEffect.DiffuseColor = Vector3.One;
		}

		public void UnloadContent()
		{
			if (m_basicEffect != null)
			{
				m_basicEffect.Dispose();
				m_basicEffect = null;
			}
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
				m_graphics.BlendState = this.BlendState;
				m_graphics.RasterizerState = m_rasterizerState;

				// Draw the vertex buffer.
				foreach (var effectPass in m_basicEffect.CurrentTechnique.Passes)
				{
					effectPass.Apply();

					m_graphics.SetVertexBuffer(m_vertexBuffer);
					m_graphics.Indices = m_indexBuffer;

					m_graphics.DrawIndexedPrimitives(PrimitiveType.TriangleList, 0, 0, m_vertexBuffer.VertexCount, 0, m_indexBuffer.IndexCount / 3);
				}
			}
		}

		#endregion
	}
}
