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
        //SETTINGS-START
        String BatteryGroupName = "BÁZIS - Akkupakk";//Batteries
        //String ReactorGroupName = "";//Reactors
        String h2ReactorGroupName = "BÁZIS_H2_Reactors";//H2 Engines(Reactors)
        String h2TanksName = "BÁZIS_H2_Tanks";//H2 Tanks
        String GasGeneratorGroupName = "BÁZIS_GasGens";//Gas Generators O2 + H2

        double H2MinPercent = 0.25f;//15%
        double H2MaxPercent = 0.9f;//95%

        float BatteryMinPercent = 0.25f;//15%
        float BatteryMaxPercent = 0.9f;//95%
        //SETTINGS-END

        List<IMyBatteryBlock> batteryList = new List<IMyBatteryBlock>();
        //List<IMyReactor> reactorList = new List<IMyReactor>();
        List<IMyGasTank> h2TankList = new List<IMyGasTank>();
        List<IMyPowerProducer> h2ReactorList = new List<IMyPowerProducer>();
        List<IMyGasGenerator> gasGeneratorList = new List<IMyGasGenerator>();

        public Program()
        {
            IMyBlockGroup batteryGroup = GridTerminalSystem.GetBlockGroupWithName(BatteryGroupName) as IMyBlockGroup;
            batteryGroup.GetBlocksOfType(batteryList);

            IMyBlockGroup h2TankGroup = GridTerminalSystem.GetBlockGroupWithName(h2TanksName) as IMyBlockGroup;
            h2TankGroup.GetBlocksOfType(h2TankList);

            //IMyBlockGroup reactorGroup = GridTerminalSystem.GetBlockGroupWithName(ReactorGroupName) as IMyBlockGroup;
            //reactorGroup.GetBlocksOfType(reactorList);

            IMyBlockGroup h2ReactorGroup = GridTerminalSystem.GetBlockGroupWithName(h2ReactorGroupName) as IMyBlockGroup;
            h2ReactorGroup.GetBlocksOfType(h2ReactorList);

            IMyBlockGroup gasGeneratorGroup = GridTerminalSystem.GetBlockGroupWithName(GasGeneratorGroupName) as IMyBlockGroup;
            gasGeneratorGroup.GetBlocksOfType(gasGeneratorList);
        }

        public void Save()
        {
        }

        public void Main(string argument, UpdateType updateSource)
        {
            switch (argument.ToUpper())
            {
                case "H2CHARGE":
                    this.setGasGeneratorsStatus(true);
                    break;
                case "H2DISCHARGE":
                    this.setGasGeneratorsStatus(true);
                    break;
                case "BATTERYCHARGE":
                    this.charge(true);
                    break;
                case "BATTERYDISCHARGE":
                    this.charge(false);
                    break;
                case "CHARGE":
                    this.setGasGeneratorsStatus(true);
                    this.charge(true);
                    break;
                case "DISCHARGE":
                    this.setGasGeneratorsStatus(false);
                    this.charge(false);
                    break;
                default:
                    double currentH2 = this.getH2TanksMinValue();
                    Echo("H2 állapot: " + (currentH2 * 100) + " %");

                    if (currentH2 < H2MinPercent)
                    {

                        this.setGasGeneratorsStatus(true);
                    }

                    if (H2MaxPercent <= currentH2)
                    {

                        this.setGasGeneratorsStatus(false);
                    }

                    float currentBatteries = this.getBatteriesMinValue();
                    Echo("Akkumulátor állapot: " + currentBatteries + " %");
                    if (currentBatteries < BatteryMinPercent)
                    {
                        this.charge(true);
                    }

                    if (BatteryMaxPercent <= currentBatteries)
                    {
                        this.charge(false);
                    }
                    break;
            }
        }

        private void charge(bool chargeBattery)
        {

            if (chargeBattery)
            {
                Echo("Töltés indítása...");
                this.setBatteriesStatus(ChargeMode.Recharge);
            } else
            {
                Echo("Töltés leállítása...");
                this.setBatteriesStatus(ChargeMode.Auto);
            }
            //this.setReactorsStatus(chargeBattery);
            this.setH2GeneratorsStatus(chargeBattery);
        }

        private double getH2TanksMinValue()
        {

            if (batteryList.Count < 1)
            {
                Echo("No battery found.");
                return 0.0d;
            }

            IMyGasTank lowestH2Tank = h2TankList[0];

            for (int i = 1; i < h2TankList.Count; i++)
            {
                IMyGasTank h2Tank = h2TankList[i] as IMyGasTank;
                if (lowestH2Tank.FilledRatio > h2Tank.FilledRatio)
                {
                    lowestH2Tank = h2Tank;
                }
            }

            Echo("Legkevesebb H2: " + lowestH2Tank.CustomName + "(" + lowestH2Tank.FilledRatio * 100 + " %)");

            return lowestH2Tank.FilledRatio;
        }

        private float getBatteriesMinValue()
        {

            if (batteryList.Count < 1)
            {
                Echo("No battery found.");
                return 0.0f;
            }

            IMyBatteryBlock lowestBattery = batteryList[0];

            for (int i = 1; i < batteryList.Count; i++)
            {
                IMyBatteryBlock battery = batteryList[i] as IMyBatteryBlock;
                if (this.getBatteryPercent(lowestBattery) > this.getBatteryPercent(battery))
                {
                    lowestBattery = battery;
                }
            }

            Echo("Leggyengébb akku: " + lowestBattery.CustomName + "(" + this.getBatteryPercent(lowestBattery) * 100 + " %)");

            return getBatteryPercent(lowestBattery);
        }


        private float getBatteryPercent(IMyBatteryBlock battery)
        {
            return battery.CurrentStoredPower / battery.MaxStoredPower;
        }

        private void setBatteriesStatus(ChargeMode chargeMode)
        {
            for (int i = 0; i < batteryList.Count; i++)
            {
                IMyBatteryBlock battery = batteryList[i] as IMyBatteryBlock;
                battery.ChargeMode = chargeMode;
                Echo("Battery " + battery.CustomName + " set" + chargeMode);
            }
        }
        /*
        private void setReactorsStatus(bool on)
        {
            for (int i = 0; i < reactorList.Count; i++)
            {
                IMyReactor reactor = reactorList[i] as IMyReactor;
                Echo("Reactor " + reactor.CustomName);
                if (on)
                {
                    reactor.ApplyAction("OnOff_On");
                    Echo("Reactor " + reactor.CustomName + " set ON.");
                }
                else
                {
                    reactor.ApplyAction("OnOff_Off");
                    Echo("Reactor " + reactor.CustomName + " set OFF.");
                }

            }
        }
        */
        private void setH2GeneratorsStatus(bool on)
        {
            for (int i = 0; i < h2ReactorList.Count; i++)
            {
                IMyPowerProducer h2reactor = h2ReactorList[i] as IMyPowerProducer;
                if (on)
                {
                    h2reactor.ApplyAction("OnOff_On");
                    Echo("Reactor " + h2reactor.CustomName + " set ON.");
                }
                else
                {
                    h2reactor.ApplyAction("OnOff_Off");
                    Echo("Reactor " + h2reactor.CustomName + " set OFF.");
                }

            }
        }

        private void setGasGeneratorsStatus(bool on)
        {
            for (int i = 0; i < gasGeneratorList.Count; i++)
            {
                IMyGasGenerator gasGenerator = gasGeneratorList[i] as IMyGasGenerator;
                if (on)
                {
                    gasGenerator.ApplyAction("OnOff_On");
                    Echo("GasGenerator " + gasGenerator.CustomName + " set ON.");
                }
                else
                {
                    gasGenerator.ApplyAction("OnOff_Off");
                    Echo("GasGenerator " + gasGenerator.CustomName + " set OFF.");
                }
            }
        }
    }
}
