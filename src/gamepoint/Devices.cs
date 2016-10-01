using Dargon.Robotics.Subsystems.DriveTrain.Holonomic;

namespace Dargon.Robotics.Demo {
   public class Devices {
      public Devices(HolonomicDriveTrain driveTrain) {
         DriveTrain = driveTrain;
      }

      public HolonomicDriveTrain DriveTrain { get; }
   }
}