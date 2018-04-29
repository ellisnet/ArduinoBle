/* Copyright 2018 Ellisnet - Jeremy Ellis (jeremy@ellisnet.com)
Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at
http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

#include <SPI.h>
#include <Wire.h>
#include <Adafruit_BLE.h>
#include <Adafruit_BluefruitLE_UART.h>
#include <Adafruit_BluefruitLE_SPI.h>
#include <Adafruit_BLEGatt.h>
#include <Adafruit_ATParser.h>
#include <SparkFun_MMA8452Q.h>

//BEGIN Adafruit BLE settings

#define FACTORYRESET_ENABLE         1
#define MINIMUM_FIRMWARE_VERSION    "0.6.6"
#define MODE_LED_BEHAVIOUR          "MODE"

// ----------------------------------------------------------------------------------------------
// These settings are used in both SW UART, HW UART and SPI mode
// ----------------------------------------------------------------------------------------------
#define BUFSIZE                        128   // Size of the read buffer for incoming data
#define VERBOSE_MODE                   true  // If set to 'true' enables debug output

// SHARED SPI SETTINGS
#define BLUEFRUIT_SPI_CS               8
#define BLUEFRUIT_SPI_IRQ              7
#define BLUEFRUIT_SPI_RST              4    // Optional but recommended, set to -1 if unused

// SOFTWARE SPI SETTINGS
#define BLUEFRUIT_SPI_SCK              13
#define BLUEFRUIT_SPI_MISO             12
#define BLUEFRUIT_SPI_MOSI             11

//END Adafruit BLE settings

#define ACCELEROMETER 0x1D

const int RED_BUTTON = 3;

const int CYCLE_MILLISECONDS = 50;

#define INTERMEDIATE_IND			"INT"
#define FINAL_IND					"FINAL"

#define CALIBRATE_MSG				"CA"
#define START_MSG					"ST"

int red_button_state = LOW;
int prev_red_button_state = LOW;
boolean is_started = false;

boolean initial_calibration_set = false;
int calibrated_value = 0;
int x_axis_value = 0;
int prev_x_axis_value = 0;

boolean scan_complete = false;

// A small helper
void error(const __FlashStringHelper*err) {
	Serial.println(err);
	while (1);
}

//BEGIN Adafruit BLE functions

//Initialize the Arduino BLE library
// Option 1 - hardware SPI
/* ...hardware SPI, using SCK/MOSI/MISO hardware SPI pins and then user selected CS/IRQ/RST */
//Adafruit_BluefruitLE_SPI ble(BLUEFRUIT_SPI_CS, BLUEFRUIT_SPI_IRQ, BLUEFRUIT_SPI_RST);

//Option 2 - software SPI
/* ...software SPI, using SCK/MOSI/MISO user-defined SPI pins and then user selected CS/IRQ/RST */
Adafruit_BluefruitLE_SPI ble(BLUEFRUIT_SPI_SCK, BLUEFRUIT_SPI_MISO,
	BLUEFRUIT_SPI_MOSI, BLUEFRUIT_SPI_CS,
	BLUEFRUIT_SPI_IRQ, BLUEFRUIT_SPI_RST);

/**************************************************************************/
/*!
@brief  Checks for user input (via the Serial Monitor)
*/
/**************************************************************************/
bool getUserInput(char buffer[], uint8_t maxSize)
{
	// timeout in 100 milliseconds
	TimeoutTimer timeout(100);

	memset(buffer, 0, maxSize);
	while ((!Serial.available()) && !timeout.expired()) { delay(1); }

	if (timeout.expired()) return false;

	delay(2);
	uint8_t count = 0;
	do
	{
		count += Serial.readBytes(buffer + count, maxSize);
		delay(2);
	} while ((count < maxSize) && (Serial.available()));

	return true;
}

void sendBleMsg(char inputs[]) {
	ble.print("AT+BLEUARTTX=");
	ble.println(inputs);

	// check response stastus
	if (!ble.waitForOK()) {
		Serial.println(F("Failed to send?"));
	}
}

//END Adafruit BLE functions

//BEGIN Sparkfun Qwiic Accelerometer functions

MMA8452Q accel(ACCELEROMETER);

//END Sparkfun Qwiic Accelerometer functions

void setup() {
	//Initialize Wire library
	Wire.begin();

	Serial.begin(115200);
	Serial.println("Starting application...");

	//BEGIN Setup code for Adafruit BLE

	/* Initialise the module */
	Serial.print(F("Initialising the Bluefruit LE module: "));

	if (!ble.begin(VERBOSE_MODE))
	{
		error(F("Couldn't find Bluefruit, make sure it's in CoMmanD mode & check wiring?"));
	}
	Serial.println(F("OK!"));

	if (FACTORYRESET_ENABLE)
	{
		/* Perform a factory reset to make sure everything is in a known state */
		Serial.println(F("Performing a factory reset: "));
		if (!ble.factoryReset()) {
			error(F("Couldn't factory reset"));
		}
	}

	/* Disable command echo from Bluefruit */
	ble.echo(false);

	Serial.println("Requesting Bluefruit info:");
	/* Print Bluefruit information */
	ble.info();

	//Serial.println(F("Please use Adafruit Bluefruit LE app to connect in UART mode"));
	//Serial.println(F("Then Enter characters to send to Bluefruit"));
	//Serial.println();

	ble.verbose(false);  // debug info is a little annoying after this point!

						 /* Wait for connection */
	while (!ble.isConnected()) {
		delay(500);
	}

	// LED Activity command is only supported from 0.6.6
	if (ble.isVersionAtLeast(MINIMUM_FIRMWARE_VERSION))
	{
		// Change Mode LED Activity
		Serial.println(F("******************************"));
		Serial.println(F("Change LED activity to " MODE_LED_BEHAVIOUR));
		ble.sendCommandCheckOK("AT+HWModeLED=" MODE_LED_BEHAVIOUR);
		Serial.println(F("******************************"));
	}

	//END Setup code for Adafruit BLE
	
	//BEGIN Setup code for Sparkfun Qwiic Accelerometer
	
	accel.init(); // Default init: +/-2g and 800Hz ODR

	/*
	//Can also use:
	accel.init([scale], [odr]); // Init and customize the FSR and ODR
	// Scale can be either SCALE_2G, SCALE_4G, or SCALE_8G. 
	// The “odr” variable can be either ODR_800, ODR_400, ODR_200, ODR_100, ODR_50, ODR_12, ODR_6, or ODR_1, 
	//   respectively setting the data rate to 800, 400, 200, 100, 50, 12.5, 6.25, or 1.56 Hz.	
	*/
	
	//END Setup code for Sparkfun Qwiic Accelerometer

	//Initialize the red button pin for input
	pinMode(RED_BUTTON, INPUT);
}

void loop() {
	if (!scan_complete) {
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

		scan_complete = true;
	}

	// BEGIN Read and report accelerometer values

	accel.read(); // Update accelerometer data

	//For this level project, we only care about the x-axis

	short xAcceleration = accel.x; // Read in raw x-axis acceleration data
	//Serial.print("Acceleration on the x-axis is ");
	//Serial.println(xAcceleration);

	if (!initial_calibration_set) {
		calibrated_value = xAcceleration;
		initial_calibration_set = true;
	}

	x_axis_value = xAcceleration - calibrated_value;
	Serial.print("Current x-axis value is ");
	Serial.println(x_axis_value);

	//short yAcceleration = accel.y; // Read in raw y-axis acceleration data
	//Serial.print("Acceleration on the y-axis is ");
	//Serial.println(yAcceleration);

	//short zAcceleration = accel.z; // Read in raw z-axis acceleration data
	//Serial.print("Acceleration on the z-axis is ");
	//Serial.println(zAcceleration);

/*
	//Sample code for checking calculated values - calculated to g-units

	xAcceleration = accel.cx; // Read in calculated x-axis acceleration
	Serial.print("Calculated acceleration on the x-axis is: ");
	Serial.print(xAcceleration);
	Serial.println(" g's");

	yAcceleration = accel.cy; // Read in calculated y-axis acceleration
	Serial.print("Calculated acceleration on the y-axis is: ");
	Serial.print(yAcceleration);
	Serial.println(" g's");

	zAcceleration = accel.cz; // Read in calculated z-axis acceleration
	Serial.print("Calculated acceleration on the z-axis is: ");
	Serial.print(zAcceleration);
	Serial.println(" g's");
*/

	//END Read and report accelerometer values
	
	boolean is_final_data = false;
	String message_type = INTERMEDIATE_IND;

	red_button_state = digitalRead(RED_BUTTON);

	if (red_button_state != prev_red_button_state) {
		if (red_button_state == HIGH & is_started) {
			Serial.println("RED button pressed.");
			//sendBleMsg("BR");
			is_final_data = true;
			message_type = FINAL_IND;
		}
		prev_red_button_state = red_button_state;
	}

	// Check for user input
	char inputs[BUFSIZE + 1];

	if (getUserInput(inputs, BUFSIZE))
	{
		// Send characters to Bluefruit
		Serial.print("[Send] ");
		Serial.println(inputs);

		sendBleMsg(inputs);
	}

	// Check for incoming characters from Bluefruit
	ble.println("AT+BLEUARTRX");
	ble.readline();
	if (!(strcmp(ble.buffer, "OK") == 0)) 
	{
		if (ble.buffer[0] == 'C' & ble.buffer[1] == 'A') {
			Serial.println("Calibrate message received.");
			calibrated_value = xAcceleration;
		}
		else if (ble.buffer[0] == 'S' & ble.buffer[1] == 'T') {
			Serial.println("Start message received.");
			is_started = true;
		}
		else {
			// Some data was found, its in the buffer
			Serial.print(F("[Recv] "));
			Serial.println(ble.buffer);
		}
		ble.waitForOK();
	}

	if (is_started) {
		String message = message_type + ',' + x_axis_value + ':';
		int msgBufferLength = message.length() + 1;
		char messageBuffer[msgBufferLength]; //This is legal, even though Visual Studio doesn't like it
		message.toCharArray(messageBuffer, msgBufferLength);
		sendBleMsg(messageBuffer);
		if (is_final_data) {
			is_started = false;
		}
	}

	delay(CYCLE_MILLISECONDS); //loop delay
}
