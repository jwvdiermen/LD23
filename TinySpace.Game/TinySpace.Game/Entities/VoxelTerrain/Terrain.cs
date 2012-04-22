/*
 *  TinySpace by Crealuz - TinySpace.Game
 * 
 *  @original authors: jwvdiermen
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using TinySpace.Entity;
using TinySpace.Scene;

namespace TinySpace.Game.Entities
{
	/// <summary>
	/// A destructable terrain entity using Voxels. The terrain is split up into a grid of individual renderable regions
	/// which contain voxel data.
	/// </summary>
	public class Terrain : EntityBase
	{
		#region Constructors

		/// <summary>
		/// Public constructor.
		/// </summary>
		/// <param name="terrainSize">The size of the terrain.</param>
		/// <param name="source">The data source.</param>
		/// <param name="name">The name of the entity.</param>
		public Terrain(Vector2 terrainSize, ITerrainSource source, string name = "Terrain")
			: base(name)
		{
			m_terrainSize = terrainSize;
			m_terrainSource = source;

			var ratio = (float)source.RegionCountY / (float)source.RegionCountX;
			if (m_terrainSize.X * ratio != m_terrainSize.Y)
			{
				throw new ArgumentException("The ratio of the terrain size should be equal to the ratio of the number of regions in both dimensions of the terrain source.");
			}
			m_regionSize = m_terrainSize.X / (float)source.RegionCountX;

			m_regions = new TerrainRegion[source.RegionCountX, source.RegionCountY];
		}

		#endregion

		#region Fields

		private Vector2 m_terrainSize;
		private ITerrainSource m_terrainSource;
		private float m_regionSize;

		private TerrainRegion[,] m_regions;

		private List<IDisposable> m_contentList = new List<IDisposable>();

		#endregion

		#region Properties

		/// <summary>
		/// Gets the terrain size.
		/// </summary>
		public Vector2 TerrainSize
		{
			get { return m_terrainSize; }
		}

		#endregion

		#region Methods

		private void ClearAll()
		{
			// Destroy the scene node.
			if (this.SceneNode != null)
			{
				this.SceneNode.Dispose();
				this.SceneNode = null;
			}

			// Clear the regions.
			int width = m_regions.GetUpperBound(0) + 1;
			int height = m_regions.GetUpperBound(1) + 1;

			for (int x = 0; x < width; ++x)
			{
				for (int y = 0; y < height; ++y)
				{
					var region = m_regions[x, y];
					if (region != null)
					{
						region.Dispose();
						m_regions[x, y] = null;
					}
				}
			}
		}

		protected override void OnWorldChanged(IEntityWorld world)
		{
			// Clear the terrain.
			ClearAll();

			// Create our scene node.
			this.SceneNode = world.Scene.Root.CreateChild(this.Name);

			// Generate the new regions if necessary.
			if (world != null)
			{
				// TODO: optimize regions based on visible camera view / active physic islands.
				int width = m_regions.GetUpperBound(0) + 1;
				int height = m_regions.GetUpperBound(1) + 1;

				for (int x = 0; x < width; ++x)
				{
					for (int y = 0; y < height; ++y)
					{
						// Create the region.
						var region = new TerrainRegion(this, m_regionSize, m_terrainSource.GetRegionSource(x, y), 50, this.Name + "_Region(" + x + "," + y + ")");

						// Attach to the scene node.
						this.SceneNode.Attach(region);

						// Generate data and store.
						region.RegenerateTerrain();
						m_regions[x, y] = region;
					}
				}
			}
		}

		public override void LoadContent(IServiceProvider services)
		{			
		}

		public override void UnloadContent()
		{
			// Dispose our content.
			foreach (var asset in m_contentList)
			{
				asset.Dispose();
			}
			m_contentList.Clear();
		}		

		#endregion
	}
}
