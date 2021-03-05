# SpaceEngineers

## Scriptek

### BatterySystem.cs

 - Ha az akku `MIN` érték alá esik akkor bekapcsolja a generátorokat és az akku töltés üzemmódba kerül.
 - Ha az akku `MAX` érték szintre ér akkor lekapcsolja a generátorokat és az akku auto üzemmódba kerül.

### AutoBuilder1.cs

Az `AB_Pistons` csoportot mozgatja.
Megadható értékek:
 - MIN
   - Sebesség: -0.5f
 - MAX
   - Sebesség: 0.5f 
 - SLOW
   - Gyártáshoz
   - Sebesség: -0.02f
