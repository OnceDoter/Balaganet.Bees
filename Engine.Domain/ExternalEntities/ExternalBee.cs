using Engine.Field.Common;
using Engine.Field.InternalEntities;
using Engine.Field.Map;

namespace Engine.Field.ExternalEntities;

public abstract class ExternalBee(ExternalContext context, DimensionPart dimensionPart, InternalBee bee)
   : ExternalEntity(context)
{
   public void TurnRight()
   {
      switch (bee.Direction)
      {
         case Direction.North:
            bee.Turn(Direction.East);
            break;
         case Direction.South:
            bee.Turn(Direction.West);
            break;
         case Direction.East:
            bee.Turn(Direction.South);
            break;
         case Direction.West:
            bee.Turn(Direction.North);
            break;
         default:
            throw new ArgumentOutOfRangeException();
      }
   }

   public void TurnLeft()
   {
      switch (bee.Direction)
      {
         case Direction.North:
            bee.Turn(Direction.West);
            break;
         case Direction.South:
            bee.Turn(Direction.East);
            break;
         case Direction.East:
            bee.Turn(Direction.North);
            break;
         case Direction.West:
            bee.Turn(Direction.South);
            break;
         default:
            throw new ArgumentOutOfRangeException();
      }
   }

   public void MoveForward()
   {
      switch(bee.Direction)
      {
         case Direction.North:
            bee.UpdateCoordinates((short)(bee.TileX + 1), (short)bee.TileY);
            break;
         case Direction.South:
            bee.UpdateCoordinates((short)(bee.TileX - 1), (short)bee.TileY);
            break;
         case Direction.East:
            bee.UpdateCoordinates((short)bee.TileX, (short)(bee.TileY + 1));
            break;
         case Direction.West:
            bee.UpdateCoordinates((short)bee.TileX, (short)(bee.TileY - 1));
            break;
         default:
            throw new ArgumentOutOfRangeException();
      }
   }
}