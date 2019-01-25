/*
 Name:		SegmentedLed.ino
 Created:	1/25/2019 1:19:13 PM
 Author:	jerem
*/

//Wire library needed for I2C/QWIIC
#include <Wire.h>

//Needed for LED Segment Display
#include <Adafruit_LEDBackpack.h>
#include <gfxfont.h>
#include <Adafruit_SPITFT_Macros.h>
#include <Adafruit_SPITFT.h>
#include <Adafruit_GFX.h>

//Needed for SparkFun human presence indicator
#include <SparkFun_AK975X_Arduino_Library.h>


boolean i2c_scan_complete = false;
const int LOOP_DELAY_MS = 500;

#define QWIIC_LED_SEGMENT 0x70
boolean led_segment_found;
Adafruit_7segment led_matrix = Adafruit_7segment();

#define QWIIC_PRESENCE_SENSOR 0x64
boolean presence_sensor_found;
AK975X movementSensor = AK975X();
int ir1, ir2, ir3, ir4, temperature;

// the setup function runs once when you press reset or power the board
void setup() {
	//Initialize Wire library
	Wire.begin();

	Serial.begin(115200);
	Serial.println("Starting application...");

}

// the loop function runs over and over again until power down or reset
void loop() {
  
	if (!i2c_scan_complete) {
		byte error, address;
		int nDevices;

		Serial.println("Scanning for I2C devices...");

		nDevices = 0;
		for (address = 1; address < 127; address++)
		{
			// The i2c_scanner uses the return value of
			// the Write.endTransmission to see if
			// a device did acknowledge to the address.
			Wire.beginTransmission(address);
			error = Wire.endTransmission();

			if (error == 0)
			{
				Serial.print("I2C device found at address 0x");
				if (address < 16) {
					Serial.print("0");
				}
				Serial.print(address, HEX);
				Serial.println("  !");

				if (address == QWIIC_LED_SEGMENT) {
					led_segment_found = true;
					Serial.println("Segmented LED display found.");
					led_matrix.begin(QWIIC_LED_SEGMENT);
				}

				if (address == QWIIC_PRESENCE_SENSOR) {
					if (movementSensor.begin()) {
						presence_sensor_found = true;
						Serial.println("Human presence sensor found.");
					}
				}

				nDevices++;
			}
			else if (error == 4)
			{
				Serial.print("Unknown error at address 0x");
				if (address < 16) {
					Serial.print("0");
				}
				Serial.println(address, HEX);
			}
		}
		if (nDevices == 0) {
			Serial.println("No I2C devices found\n");
		}
		else {
			Serial.println("done\n");
		}

		i2c_scan_complete = true;
	}

	//if (led_segment_found) {
	//	led_matrix.print(0xBEEF, HEX);
	//	led_matrix.writeDisplay();
	//}

	if (presence_sensor_found) {
		if (movementSensor.available())
		{
			ir1 = movementSensor.getIR1();
			ir2 = movementSensor.getIR2();
			ir3 = movementSensor.getIR3();
			ir4 = movementSensor.getIR4();
			float tempF = movementSensor.getTemperatureF();

			movementSensor.refresh(); //Read dummy register after new data is read

			//Note: The observable area is shown in the silkscreen.
			//If sensor 2 increases first, the human is on the left
			Serial.print("1:DWN[");
			Serial.print(ir1);
			Serial.print("]\t2:LFT[");
			Serial.print(ir2);
			Serial.print("]\t3:UP[");
			Serial.print(ir3);
			Serial.print("]\t4:RGH[");
			Serial.print(ir4);
			Serial.print("]\ttempF[");
			Serial.print(tempF);
			Serial.print("]\tmillis[");
			Serial.print(millis());
			Serial.print("]");
			Serial.println();

			if (led_segment_found) {
				if (ir3 > 3500 & ir3 <= 9999) {
					led_matrix.print(ir3, DEC);
					led_matrix.writeDisplay();
				}
				else {
					led_matrix.clear();
					led_matrix.writeDisplay();
				}
			}
		}
	}

	delay(LOOP_DELAY_MS); //loop delay
}
