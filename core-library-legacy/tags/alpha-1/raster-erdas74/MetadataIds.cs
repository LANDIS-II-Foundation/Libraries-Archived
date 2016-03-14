
namespace Landis.Raster.Erdas74
{
    /// <summary>
    /// The Metadata constants used by this driver's implementation
    /// </summary>
    class MetadataIds
    {
        // Metadata ID's
        
        
        public const string RASTER_ULX  = "Raster Upper Left X";
            // Int32 representing the x pixel number of the upper left corner
            
        public const string RASTER_ULY  = "Raster Upper Left Y";
            // Int32 representing the y pixel number of the upper left corner
            
        public const string WORLD_ULX   = "World Upper Left X";
            // float representing the x world coord of the upper left corner
            
        public const string WORLD_ULY   = "World Upper Left Y";
            // float representing the y world coord of the upper left corner
            
        public const string X_SCALE     = "X Scale";
            // float representing the number of units per cell in the x direction
            
        public const string Y_SCALE     = "Y Scale";
            // float representing the number of units per cell in the y direction
            
        public const string SCALE_UNITS = "Scale Units";
            // string representing the real world units of the X & Y scales
            
        public const string PROJECTION  = "Projection";
            // string representing the name of the projection
        
    }
}
