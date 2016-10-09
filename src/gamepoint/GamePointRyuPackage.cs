using Dargon.Robotics.DeviceRegistries;
using Dargon.Robotics.Devices;
using Dargon.Robotics.RollbackLogs;
using Dargon.Robotics.Subsystems.DriveTrain.Holonomic;
using Dargon.Ryu;
using Dargon.Ryu.Modules;
using System;
using System.Collections.Generic;
using Dargon.Robotics.Debugging;
using Dargon.Robotics.GamePoint.Commands;

namespace Dargon.Robotics.GamePoint {
   public class GamePointRyuPackage : RyuModule {
      public GamePointRyuPackage() {
         Optional.Singleton<IRobot>(CreateRobot);
         Optional.Singleton<GamePoint.Devices>(ConstructDevices);
         Optional.Singleton<HolonomicDriveTrain>(ryu => ryu.GetOrActivate<GamePoint.Devices>().DriveTrain);
         Optional.Singleton<IPositionTracker>(ryu => ryu.GetOrActivate<GamePoint.Devices>().PositionTracker);
         Optional.Singleton<IGyroscope>(ryu => ryu.GetOrActivate<GamePoint.Devices>().YawGyroscope);
      }

      private IRobot CreateRobot(IRyuContainer ryu) {
         var configuration = new IterativeRobotConfiguration(100);
         var subsystems = new Dictionary<int, ISubsystem>();
         var commands = new ICommand[] {
            ryu.GetOrActivate<TankDriveCommand>(),
            ryu.GetOrActivate<DriveToOffsetCommand>()
         };
         var debugRenderContext = ryu.GetOrActivate<IDebugRenderContext>();
         return SubsystemCommandBasedIterativeRobot.Create(configuration, subsystems, commands, debugRenderContext);
      }

      private GamePoint.Devices ConstructDevices(IRyuContainer ryu) {
         var deviceRegistry = ryu.GetOrActivate<IDeviceRegistry>();
         var frontLeftMotor = deviceRegistry.GetDevice<IMotor>("Drive.Motors.FrontLeft");
         var frontRightMotor = deviceRegistry.GetDevice<IMotor>("Drive.Motors.FrontRight");
         var rearLeftMotor = deviceRegistry.GetDevice<IMotor>("Drive.Motors.RearLeft");
         var rearRightMotor = deviceRegistry.GetDevice<IMotor>("Drive.Motors.RearRight");

         var frontLeftIncrementalRotaryEncoder = deviceRegistry.GetDevice<IIncrementalRotaryEncoder>("Drive.Motors.FrontLeft.Encoder");
         var frontRightIncrementalRotaryEncoder = deviceRegistry.GetDevice<IIncrementalRotaryEncoder>("Drive.Motors.FrontRight.Encoder");
         var yawGyro = deviceRegistry.GetDevice<IGyroscope>("Drive.Gyroscopes.Yaw");

         var positionTracker = new TankDriveShaftEncodersAndYawGyroscopeBasedPositionTracker("Drive.PositionTracker", yawGyro, frontLeftIncrementalRotaryEncoder, frontRightIncrementalRotaryEncoder, 5.0f * 0.0254f);
         positionTracker.Initialize();
         deviceRegistry.AddDevice(positionTracker.Name, positionTracker);

         var motionLog = new MotionStateSnapshotLog(positionTracker, yawGyro, TimeSpan.FromSeconds(2));
         var driveTrain = new HolonomicDriveTrain(frontLeftMotor, frontRightMotor, rearLeftMotor, rearRightMotor);
         return new GamePoint.Devices(driveTrain, yawGyro, positionTracker, motionLog);
      }
   }
}