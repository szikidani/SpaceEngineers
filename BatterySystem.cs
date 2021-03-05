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

        //SETTINGS-START
        String BatteryGroupName = "Battery_Group_Name";//Battery
        String ReactorGroupName = "Reactor_Group_Name";//Reactor
        String GeneratorGroupName = "Generator_Group_Name";//O2/H2 Generator

        float MAX = 90.0f;//90%
        float MIN = 20.0f;//20%
                          //SETTINGS-END


        IMyBlockGroup batteryGroup;
        IMyBlockGroup reactorGroup;
        IMyBlockGroup generatorGroup;


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

            batteryGroup = GridTerminalSystem.GetBlockGroupWithName(BatteryGroupName) as IMyBlockGroup;
            reactorGroup = GridTerminalSystem.GetBlockGroupWithName(ReactorGroupName) as IMyBlockGroup;
            generatorGroup = GridTerminalSystem.GetBlockGroupWithName(GeneratorGroupName) as IMyBlockGroup;
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
            List<IMyBatteryBlock> batteryList = new List<IMyBatteryBlock>();
            batteryGroup.GetBlocksOfType(batteryList);

            List<IMyReactor> reactorList = new List<IMyReactor>();
            reactorGroup.GetBlocksOfType(reactorList);

            List<IMyBatteryBlock> generatorList = new List<IMyBatteryBlock>();
            generatorGroup.GetBlocksOfType(generatorList);

            float current = this.getMinValue(batteryList);
            if (current < MIN)
            {
                this.setReactorsStatus(true, reactorList);
                this.setGeneratorsStatus(true, generatorList);
                this.setBatteriesStatus(ChargeMode.Recharge, batteryList);
            }

            if (MAX <= current)
            {
                this.setReactorsStatus(true, reactorList);
                this.setGeneratorsStatus(false, generatorList);
                this.setBatteriesStatus(ChargeMode.Auto, batteryList);
            }

        }

        private float getMinValue(List<IMyBatteryBlock> batteryList)
        {
            float minValue = 100.0f;

            for (int i = 0; i < batteryList.Count; i++)
            {
                IMyBatteryBlock battery = batteryList[i] as IMyBatteryBlock;
                if (minValue > battery.CurrentStoredPower)
                {
                    minValue = battery.CurrentStoredPower;
                }
            }

            return minValue;
        }

        private void setBatteriesStatus(ChargeMode chargeMode, List<IMyBatteryBlock> batteryList)
        {
            for (int i = 0; i < batteryList.Count; i++)
            {
                IMyBatteryBlock battery = batteryList[i] as IMyBatteryBlock;
                battery.ChargeMode = chargeMode;
            }
        }

        private void setReactorsStatus(bool on, List<IMyReactor> reactorList)
        {
            for (int i = 0; i < reactorList.Count; i++)
            {
                IMyReactor reactor = reactorList[i] as IMyReactor;
                reactor.Enabled = on;
            }
        }

        private void setGeneratorsStatus(bool on, List<IMyBatteryBlock> reactorList)
        {
            for (int i = 0; i < reactorList.Count; i++)
            {
                IMyReactor reactor = reactorList[i] as IMyReactor;
                reactor.Enabled = on;
            }
        }
    }
}
