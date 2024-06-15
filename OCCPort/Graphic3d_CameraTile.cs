using System;

namespace OCCPort
{
	public class Graphic3d_CameraTile
	{
		
		public Graphic3d_CameraTile()
		{
			TotalSize = new Graphic3d_Vec2i();
			TileSize = new Graphic3d_Vec2i();
			Offset = new Graphic3d_Vec2i();
		}

		public Graphic3d_Vec2i TotalSize; //!< total size of the View area, in pixels
		public Graphic3d_Vec2i TileSize;  //!< size of the Tile, in pixels
		public Graphic3d_Vec2i Offset;    //!< the lower-left corner of the Tile relative to the View area (or upper-left if IsTopDown is true), in pixels
		public  bool IsTopDown; //!< 
		public bool IsValid()
		{

			return TotalSize.x() > 0 && TotalSize.y() > 0
				&& TileSize.x() > 0 && TileSize.y() > 0;

		}

		public NCollection_Vec2i OffsetLowerLeft()
		{
			return new NCollection_Vec2i(Offset.x(),
						   !IsTopDown
						   ? Offset.y()
						   : TotalSize.y() - Offset.y() - 1);
		}

	}
}


