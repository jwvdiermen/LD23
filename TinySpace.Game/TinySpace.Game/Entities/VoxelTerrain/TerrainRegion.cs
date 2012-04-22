using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using FarseerPhysics.Collision;
using FarseerPhysics.Common;

using TinySpace.Scene;
using TinySpace.Core;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using TinySpace.Entity;

namespace TinySpace.Game.Entities
{
	/// <summary>
	/// Represents a terrain region which is independent with its own source, which is part of the complete terrain.
	/// Implements both the <see cref="IMovable" /> so it can be attached to the scene by the terrain and <see cref="IRenderable" /> 
	/// used by the terrain for rendering the final image.
	/// </summary>
	public class TerrainRegion : Movable, IUpdatable, IRenderable
	{
		#region Constructors

		internal TerrainRegion(Terrain terrain, float size, ITerrainRegionSource regionSource, int cellSize, string name = null)
			: base(name)
		{
			m_terrain = terrain;
			m_regionSource = regionSource;

			this.Size = size;
			this.PointsPerUnit = (int)Math.Ceiling((float)m_regionSource.Size / size);
			this.Iterations = 2;

			this.CellSize = cellSize;
			this.SubCellSize = cellSize / this.PointsPerUnit;

			m_xnum = (int)(m_regionSource.Size / this.CellSize);
			m_ynum = (int)(m_regionSource.Size / this.CellSize);

			m_verticesMap = new List<Vertices>[m_xnum, m_ynum];

			// Large AABB with max values so everything is dirty.
			m_dirtyArea = new AABB(Vector2.Zero, new Vector2(m_regionSource.Size - 1, m_regionSource.Size - 1));

			// Determine if we should enable physics.
			if (terrain.World is DynamicEntityWorld)
			{
				m_physicalWorld = ((DynamicEntityWorld)terrain.World).PhysicsWorld;
				m_bodyMap = new List<Body>[m_xnum, m_ynum];
			}

			// Create the rasterizer state.
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

		private Terrain m_terrain;
		private World m_physicalWorld;

		private ITerrainRegionSource m_regionSource;
		
		private int m_xnum;
		private int m_ynum;
		private AABB m_dirtyArea;

		private List<Vertices>[,] m_verticesMap;
		private List<Body>[,] m_bodyMap;

		private VertexBuffer m_vertexBuffer;
		private IndexBuffer m_indexBuffer;

		private BasicEffect m_basicEffect;

		#endregion

		#region Properties

		/// <summary>
		/// Gets the size.
		/// </summary>
		public float Size { get; private set; }

		/// <summary>
		/// Gets the number of points per unit.
		/// </summary>
		public int PointsPerUnit { get; private set; }

		/// <summary>
		/// Gets or sets the number of points per cell.
		/// </summary>
		public int CellSize { get; private set; }

		/// <summary>
		/// Gets or sets the number of points per sub cell.
		/// </summary>
		public int SubCellSize { get; private set; }

		/// <summary>
		/// Gets or sets the number of iterations to perform in the Marching Squares algorithm.
		/// </summary>
		public int Iterations { get; set; }

		#endregion

		#region Methods

		/// <summary>
		/// 
		/// </summary>
		/// <param name="center"></param>
		/// <param name="value"></param>
		public void DrawCircle(Vector2 center, float radius, sbyte value)
		{
			for (float by = -radius; by < radius; by += 0.1f)
			{
				for (float bx = -radius; bx < radius; bx += 0.1f)
				{
					if ((bx * bx) + (by * by) < radius * radius)
					{
						float ax = bx + center.X;
						float ay = by + center.Y;
						ModifyTerrain(new Vector2(ax, ay), value);
					}
				}
			}
		}

		/// <summary>
		/// Modify a single point in the terrain.
		/// </summary>
		/// <param name="location">World location to modify. Automatically clipped.</param>
		/// <param name="value">-1 = inside terrain, 1 = outside terrain</param>
		public void ModifyTerrain(Vector2 location, sbyte value)
		{
			// TODO: Clean this up, because it will cause terrible errors (I think) with multiple regions.

			// find local position
			// make position local to map space
			Vector2 p = location - m_terrain.SceneNode.Position + (m_terrain.TerrainSize / 2.0f);

			// find map position for each axis
			p.X = p.X * m_regionSource.Size / this.Size;
			p.Y = p.Y * m_regionSource.Size / this.Size;

			if (p.X >= 0 && p.X < m_regionSource.Size && p.Y >= 0 && p.Y < m_regionSource.Size)
			{
				m_regionSource.Data[(int)p.X, (int)p.Y] = value;

				// expand dirty area
				if (p.X < m_dirtyArea.LowerBound.X) m_dirtyArea.LowerBound.X = p.X;
				if (p.X > m_dirtyArea.UpperBound.X) m_dirtyArea.UpperBound.X = p.X;

				if (p.Y < m_dirtyArea.LowerBound.Y) m_dirtyArea.LowerBound.Y = p.Y;
				if (p.Y > m_dirtyArea.UpperBound.Y) m_dirtyArea.UpperBound.Y = p.Y;
			}
		}

		/// <summary>
		/// Regenerate the terrain.
		/// </summary>
		public void RegenerateTerrain()
		{
			// Iterate effected cells.
			var gx0 = (int)(m_dirtyArea.LowerBound.X / CellSize);
			var gx1 = (int)(m_dirtyArea.UpperBound.X / CellSize) + 1;
			if (gx0 < 0) gx0 = 0;
			if (gx1 > m_xnum) gx1 = m_xnum;
			var gy0 = (int)(m_dirtyArea.LowerBound.Y / CellSize);
			var gy1 = (int)(m_dirtyArea.UpperBound.Y / CellSize) + 1;
			if (gy0 < 0) gy0 = 0;
			if (gy1 > m_ynum) gy1 = m_ynum;

			bool regenerateBuffers = false;
			for (int gx = gx0; gx < gx1; gx++)
			{
				for (int gy = gy0; gy < gy1; gy++)
				{
					regenerateBuffers = true;

					// Remove old terrain object at grid cell
					if (m_bodyMap != null && m_bodyMap[gx, gy] != null)
					{
						for (int i = 0; i < m_bodyMap[gx, gy].Count; i++)
						{
							m_physicalWorld.RemoveBody(m_bodyMap[gx, gy][i]);
						}
						m_bodyMap[gx, gy] = null;
					}
					m_verticesMap[gx, gy] = null;

					// Regenerate the terrain.
					GenerateTerrain(gx, gy);
				}
			}

			// Regenerate the buffers.
			if (regenerateBuffers == true)
			{
				GenerateBuffers();
			}

			// Inverted AABB with max values so nothing is dirty.
			m_dirtyArea = new AABB(new Vector2(float.MaxValue, float.MaxValue), new Vector2(float.MinValue, float.MinValue));
		}

		private void ClearBuffers()
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
		}

		private void GenerateBuffers()
		{
			// Clear the buffers.
			ClearBuffers();

			// Generate the vertices and indices.
			var vertices = new List<VertexPositionColor>();
			var indices = new List<ushort>();

			foreach (var verticesList in m_verticesMap)
			{
				if (verticesList != null && verticesList.Count > 0)
				{
					foreach (var verticesObj in verticesList)
					{
						var vertexOffset = vertices.Count;
						vertices.AddRange(verticesObj.Select(e => new VertexPositionColor
						{
							Position = new Vector3(e, 0.0f),
							Color = new Color(124, 88, 30)
						}));

						int vertexCount = verticesObj.Count;
						for (int i = 1; i < vertexCount - 1; ++i)
						{
							indices.Add((ushort)vertexOffset);
							indices.Add((ushort)(vertexOffset + i));
							indices.Add((ushort)(vertexOffset + i + 1));
						}
					}
				}
			}

			// Create the buffers.
			if (vertices.Count > 0 && indices.Count > 0)
			{
				m_vertexBuffer = new VertexBuffer(m_graphics, VertexPositionColor.VertexDeclaration, vertices.Count, BufferUsage.None);
				m_vertexBuffer.SetData(vertices.ToArray());

				m_indexBuffer = new IndexBuffer(m_graphics, IndexElementSize.SixteenBits, indices.Count, BufferUsage.None);
				m_indexBuffer.SetData(indices.ToArray());
			}
		}

		private void GenerateTerrain(int gx, int gy)
		{
			int ax = gx * CellSize;
			int ay = gy * CellSize;

			var offset = new Vector2(m_regionSource.OffsetX + ax, m_regionSource.OffsetY + ay);
			var cellSize = new Vector2(CellSize, CellSize);

			var polys = MarchingSquares.DetectSquares(new AABB(offset, offset + cellSize), SubCellSize, SubCellSize, m_regionSource.Data, Iterations, true);
			if (polys.Count == 0) return;
			
			// Create the scale and translate vectors.
			var scale = new Vector2(1.0f / PointsPerUnit, 1.0f / PointsPerUnit);
			var translate = -m_terrain.TerrainSize / 2.0f;

			// Create the vertices list.
			var verticesList = new List<Vertices>();
			m_verticesMap[gx, gy] = verticesList;
			
			// Create the body list if necessary.
			List<Body> bodyList = null;
			if (m_physicalWorld != null)
			{
				bodyList = new List<Body>();
				m_bodyMap[gx, gy] = bodyList;
			}

			// create physics object for this grid cell
			foreach (var item in polys)
			{
				item.Scale(ref scale);
				item.Translate(ref translate);
				item.ForceCounterClockWise();
				var p = FarseerPhysics.Common.PolygonManipulation.SimplifyTools.CollinearSimplify(item);				
				var decompPolys = FarseerPhysics.Common.Decomposition.BayazitDecomposer.ConvexPartition(p);

				verticesList.AddRange(decompPolys);

				// Create physical bodies if necessary.
				if (m_physicalWorld != null)
				{
					foreach (Vertices poly in decompPolys)
					{
						if (poly.Count > 2)
						{
							bodyList.Add(BodyFactory.CreatePolygon(m_physicalWorld, poly, 1, this));
						}
					}
				}
			}
		}

		#endregion

		#region IUpdatable Members

		public void Update(GameTime time, ICamera camera)
		{
			// Sync the bodies with the scene node.
			if (m_bodyMap != null && this.SceneNode != null)
			{
				foreach (var bodyList in m_bodyMap)
				{
					if (bodyList != null)
					{
						foreach (var body in bodyList)
						{
							body.Position = this.SceneNode.Position;
							body.Rotation = this.SceneNode.Rotation;
						}
					}
				}
			}
		}

		#endregion

		#region IRenderable Members

		public void LoadContent(IServiceProvider services)
		{
			// Get the necessary services.
			m_graphics = ((IGraphicsDeviceService)services.GetService(typeof(IGraphicsDeviceService))).GraphicsDevice;

			// Generate the buffers if we can.
			GenerateBuffers();

			// Create the effect.
			m_basicEffect = new BasicEffect(m_graphics);
			m_basicEffect.DiffuseColor = Vector3.One;
			m_basicEffect.VertexColorEnabled = true;
		}

		public void UnloadContent()
		{
			// Clear the buffers.
			ClearBuffers();

			if (m_basicEffect != null)
			{
				m_basicEffect.Dispose();
				m_basicEffect = null;
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
				m_graphics.BlendState = BlendState.AlphaBlend;
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
