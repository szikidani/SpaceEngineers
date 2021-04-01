# SpaceEngineers

## Scriptek

### BatterySystem.cs

 - A működéshez szükséges triggerelni a blokkot.
 - Az akku csoportból a legkisebb értéket veszi alapul.
 - Ha az akku `MIN` érték alá esik akkor bekapcsolja a generátorokat és az akku töltés üzemmódba kerül.
 - Ha az akku `MAX` érték szintre ér akkor lekapcsolja a generátorokat és az akku auto üzemmódba kerül.

### MAB_System.cs

 - A felfüggesztés forgatását és ki/be mozgatását végzi. Ez nem a teljes rendszer.

### AutoBuilder.cs

Az `AB_Pistons` csoportot mozgatja.
Megadható értékek:
 - `MIN`: Behúzza a pistonokat.
 - `MAX`: Kiereszti a pistonokat.
 - `SLOW`: Elindulnak a gyártás és a pistonokat lassan húzz be.
 - ``: Stop
