using Dargon.Robotics.Devices;
using Dargon.Robotics.RollbackLogs;
using Dargon.Robotics.Subsystems.DriveTrain.Holonomic;
using Dargon.Robotics.Subsystems.DriveTrain.Tank;

namespace Dargon.Robotics.GamePoint {
   public class Devices {
      public Devices(TankDriveTrain driveTrain, IGyroscope yawGyroscope, IPositionTracker positionTracker, IMotionStateSnapshotLog motionLog) {
         DriveTrain = driveTrain;
         YawGyroscope = yawGyroscope;
         PositionTracker = positionTracker;
         MotionLog = motionLog;
      }

      public TankDriveTrain DriveTrain { get; }
      public IGyroscope YawGyroscope { get; set; }
      public IPositionTracker PositionTracker { get; set; }
      public IMotionStateSnapshotLog MotionLog { get; set; }
   }
}