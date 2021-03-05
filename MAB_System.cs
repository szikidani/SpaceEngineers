using Sandbox.Game.EntityComponents;
using Sandbox.ModAPI.Ingame;
using Sandbox.ModAPI.Interfaces;
using SpaceEngineers.Game.ModAPI.Ingame;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using VRage;
using VRage.Collections;
using VRage.Game;
using VRage.Game.Components;
using VRage.Game.GUI.TextPanel;
using VRage.Game.ModAPI.Ingame;
using VRage.Game.ModAPI.Ingame.Utilities;
using VRage.Game.ObjectBuilders.Definitions;
using VRageMath;

namespace IngameScript
{
    partial class Program : MyGridProgram
    {
        // This file contains your actual script.
        //
        // You can either keep all your code here, or you can create separate
        // code files to make your program easier to navigate while coding.
        //
        // In order to add a new utility class, right-click on your project, 
        // select 'New' then 'Add Item...'. Now find the 'Space Engineers'
        // category under 'Visual C# Items' on the left hand side, and select
        // 'Utility Class' in the main area. Name it in the box below, and
        // press OK. This utility class will be merged in with your code when
        // deploying your final script.
        //
        // You can also simply create a new utility class manually, you don't
        // have to use the template if you don't want to. Just do so the first
        // time to see what a utility class looks like.
        // 
        // Go to:
        // https://github.com/malware-dev/MDK-SE/wiki/Quick-Introduction-to-Space-Engineers-Ingame-Scripts
        //
        // to learn more about ingame scripts.


        IMyBlockGroup forwardPistonsBlockGroup;
        IMyBlockGroup backPistonsBlockGroup;
        IMyMotorAdvancedStator endRotor;

        public Program()
        {
            // The constructor, called only once every session and
            // always before any other method is called. Use it to
            // initialize your script. 
            //     
            // The constructor is optional and can be removed if not
            // needed.
            // 
            // It's recommended to set Runtime.UpdateFrequency 
            // here, which will allow your script to run itself without a 
            // timer block.

            forwardPistonsBlockGroup = GridTerminalSystem.GetBlockGroupWithName("MAB_Pistons_END_FORWARD") as IMyBlockGroup;
            backPistonsBlockGroup = GridTerminalSystem.GetBlockGroupWithName("MAB_Pistons_END_BACK") as IMyBlockGroup;
            endRotor = GridTerminalSystem.GetBlockWithName("MAB_Fejlesztett Rotor") as IMyMotorAdvancedStator;
        }

        public void Save()
        {
            // Called when the program needs to save its state. Use
            // this method to save your state to the Storage field
            // or some other means. 
            // 
            // This method is optional and can be removed if not
            // needed.
        }

        public void Main(string argument, UpdateType updateSource)
        {
            // The main entry point of the script, invoked every time
            // one of the programmable block's Run actions are invoked,
            // or the script updates itself. The updateSource argument
            // describes where the update came from. Be aware that the
            // updateSource is a  bitfield  and might contain more than 
            // one update type.
            // 
            // The method itself is required, but the arguments above
            // can be removed if not needed.

            endRotor.RotorLock = true;
            switch (argument.ToUpper())
            {
                case "MIN":
                    this.setSebessegek(-0.2f);
                    break;
                case "MAX":
                    this.setSebessegek(0.2f);
                    break;
                case "ROTOR":
                    endRotor.RotorLock = false;
                    endRotor.LowerLimitDeg = 0;
                    endRotor.UpperLimitDeg = 180;
                    endRotor.TargetVelocityRPM = -endRotor.TargetVelocityRPM;
                    break;
                default:
                    this.setSebessegek(0.0f);
                    break;
            }
        }

        private void setSebessegek(float v)
        {
            List<IMyFunctionalBlock> forwardPistons = new List<IMyFunctionalBlock>();
            forwardPistonsBlockGroup.GetBlocksOfType(forwardPistons);
            for (int i = 0; i < forwardPistons.Count; i++)
            {
                IMyPistonBase pistons = forwardPistons[i] as IMyPistonBase;
                pistons.Velocity = v;
            }

            List<IMyFunctionalBlock> backPistons = new List<IMyFunctionalBlock>();
            backPistonsBlockGroup.GetBlocksOfType(backPistons);
            for (int i = 0; i < backPistons.Count; i++)
            {
                IMyPistonBase pistons = backPistons[i] as IMyPistonBase;
                pistons.Velocity = -v;
            }
        }
    }
}
