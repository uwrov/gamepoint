using Dargon.Robotics.Devices;
using Dargon.Robotics.Subsystems.DriveTrain.Holonomic;

namespace Dargon.Robotics.Demo {
   public class Devices {
      public Devices(HolonomicDriveTrain driveTrain, IPositionTracker positionTracker) {
         DriveTrain = driveTrain;
         PositionTracker = positionTracker;
      }

      public HolonomicDriveTrain DriveTrain { get; }
      public IPositionTracker PositionTracker { get; set; }
   }
}