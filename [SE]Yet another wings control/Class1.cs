using Sandbox.ModAPI.Ingame;
using VRageMath;

namespace ScriptingClass
{
    public sealed class Program : MyGridProgram
    {
        //--------------------------------------------------------------------------------------
        private List<IMyMotorAdvancedStator> wings = new List<IMyMotorAdvancedStator>();
        private List<IMyMotorAdvancedStator> wings_updown = new List<IMyMotorAdvancedStator>();
        private IMyCockpit cockpit;
        private IMyGyro gyro;


        public Program()
        {
            Runtime.UpdateFrequency = UpdateFrequency.Update1;
            GridTerminalSystem.GetBlockGroupWithName("wings").GetBlocksOfType(wings);
            GridTerminalSystem.GetBlockGroupWithName("wings updown").GetBlocksOfType(wings_updown);
            cockpit = GridTerminalSystem.GetBlockWithName("cockpit") as IMyCockpit;
            gyro = GridTerminalSystem.GetBlockWithName("gyro") as IMyGyro;
        }

        public void Main(string argument, UpdateType updateSource)
        {
            Vector3D LinearVelocity = cockpit.GetShipVelocities().LinearVelocity;
            float speed = (float)Math.Abs(LinearVelocity.X+LinearVelocity.Y+LinearVelocity.Z);
            if (speed < 10)
            {
                foreach (var hinge in wings)
                {
                    if (hinge.CustomName != "wings up")
                    {
                        hinge.LowerLimitDeg = Math.Max(
                            speed * -1 / 2, -30);
                    }
                    else
                    {
                        hinge.UpperLimitDeg = Math.Min(
                            (speed / 2) - 20, 45);
                    }
                }
            }

            Vector3D AngularVelocity = cockpit.GetShipVelocities().AngularVelocity;
            double rotate = (float)(AngularVelocity.X+AngularVelocity.Y+AngularVelocity.Z);
            if (Math.Abs(rotate) > 1)
            {
                foreach (var hinge in wings_updown)
                {
                    hinge.LowerLimitDeg = Math.Max((float)rotate * 100, -25);
                }
            }
        }
        //--------------------------------------------------------------------------------------

    }
}