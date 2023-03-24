

ADS 1115 Converter


https://learn.adafruit.com/raspberry-pi-analog-to-digital-converters/ads1015-slash-ads1115


// Great Resource


http://openlabtools.eng.cam.ac.uk/Resources/Datalog/RPi_ADS1115/




If you use a gain of 1x, you will get a range of +/- 4.096v. 
Since it is a 12 bit ADC, the raw range will be 0-4096. -4.096v will be 0
0v will be at mid-scale (2048)  and +4.096v will be 4096


A0-A3 = Connect to Source

The single-ended signal ranges from 0 V up
to positive supply or +FS, whichever is lower. Negative voltages cannot be applied to these devices because the
ADS101x can only accept positive voltages with respect to ground.

V_in,0 = Input voltage to A0, represented as 2's complement 16-bit
V_in,1 = Input voltage to A1, represented as 2's complement 16-bit
FS = full-scale range, in volts


For ADS1115, when taking a single-ended measurement:
Vin,0 = x / 2^15 * FS

For ADS1115, when taking a differential measurement and MUX[2:0] == 000:
Vin,0 - Vin,1 = x / 2^15 * FS

Anything >= FS will read FS.




32768

6.44

1.96533203125×10−4

.0001965


0  MSB
1  LSB

51      1010001        01010001  01011001   ->  10101110   10100110
59		1011001

51		 1010001 
124	   100100100
