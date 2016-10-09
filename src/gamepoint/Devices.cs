using Dargon.Robotics.Devices;
using Dargon.Robotics.Devices.DriveTrain.Holonomic;
using Dargon.Robotics.RollbackLogs;

namespace Dargon.Robotics.GamePoint {
   public class Devices {
      public Devices(HolonomicDriveTrain driveTrain, IGyroscope yawGyroscope, IPositionTracker positionTracker, IMotionStateSnapshotLog motionLog) {
         DriveTrain = driveTrain;
         YawGyroscope = yawGyroscope;
         PositionTracker = positionTracker;
         MotionLog = motionLog;
      }

      public HolonomicDriveTrain DriveTrain { get; }
      public IGyroscope YawGyroscope { get; set; }
      public IPositionTracker PositionTracker { get; set; }
      public IMotionStateSnapshotLog MotionLog { get; set; }
   }
}