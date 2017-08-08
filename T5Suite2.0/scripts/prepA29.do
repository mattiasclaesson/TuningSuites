* Prepare SAAB T5 ECU for flash management (erase and program).
* This script needs to be executed before flash can be erased or programmed.
* Created by Patrik Servin
* Version 1.0
*
* Reset ECU
*reset
* Stop execution
stop
* Switch off software watchdog, enable busmonitor
mm $fffa20
$c.
* enable busmonitor during freeze
mm $fffa00
$40cf.
*Set up for flashing
mm 0xfffa04
0x7f00.
*
mm 0xfffa21
0x0000.
*
mm 0xfffa44
0x3fff.
*
mm 0xfffa4a
0x6b70.
*
mm 0xfffa50
0x0005.
*
mm 0xfffa52
0x3370.
*
mm 0xfffa54
0x0005.
*
mm 0xfffa56
0x5370.
*
* enable internal 2kByte RAM of 68332
* and map it to address $100000
mm $fffb04
$1000.
* tell BD32 to use internal RAM $100000 for target resident driver
driver $100000
*

* ECU is now prepared for flash programming or erasing.

