using Dargon.Robotics.DeviceRegistries;
using Dargon.Robotics.Devices;
using Dargon.Robotics.RollbackLogs;
using Dargon.Ryu;
using Dargon.Ryu.Modules;
using System;
using System.Collections.Generic;
using Dargon.Robotics.Debug;
using Dargon.Robotics.Devices.DriveTrain.Holonomic;
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
         var commands = new ICommand[] {
            ryu.GetOrActivate<TankDriveCommand>(),
            ryu.GetOrActivate<DriveToOffsetCommand>()
         };
         var debugRenderContext = ryu.GetOrActivate<IDebugRenderContext>();
         var deviceRegistry = ryu.GetOrActivate<IDeviceRegistry>();
         return SubsystemCommandBasedIterativeRobot.Create(configuration, commands, debugRenderContext, deviceRegistry);
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
         var positionTracker = deviceRegistry.GetDevice<IPositionTracker>("Drive.PositionTracker");

         var motionLog = new MotionStateSnapshotLog(positionTracker, yawGyro, TimeSpan.FromSeconds(2));
         var driveTrain = new HolonomicDriveTrain(frontLeftMotor, frontRightMotor, rearLeftMotor, rearRightMotor);
         return new GamePoint.Devices(driveTrain, yawGyro, positionTracker, motionLog);
      }
   }
}