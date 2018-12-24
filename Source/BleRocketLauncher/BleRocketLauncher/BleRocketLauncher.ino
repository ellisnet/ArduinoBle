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

//Wire library needed for I2C/QWIIC
#include <Wire.h>

//SPI library needed for BLE
#include <SPI.h>

//Adafruite BLE libraries
#include <Adafruit_BLE.h>
#include <Adafruit_BluefruitLE_UART.h>
#include <Adafruit_BluefruitLE_SPI.h>
//#include <Adafruit_BLEMIDI.h> //Not needed for this project
#include <Adafruit_BLEGatt.h>
//#include <Adafruit_BLEEddystone.h> //Not needed for this project
//#include <Adafruit_BLEBattery.h> //Not needed for this project
#include <Adafruit_ATParser.h>

//Required for LEDs
#include <Adafruit_NeoPixel.h>

//BEGIN Adafruit BLE stuff

#define FACTORYRESET_ENABLE         1
#define MINIMUM_FIRMWARE_VERSION    "0.6.6"
#define MODE_LED_BEHAVIOUR          "MODE"

// ----------------------------------------------------------------------------------------------
// These settings are used in both SW UART, HW UART and SPI mode
// ----------------------------------------------------------------------------------------------
#define BUFSIZE                        128   // Size of the read buffer for incoming data
#define VERBOSE_MODE                   true  // If set to 'true' enables debug output

// SHARED SPI SETTINGS
// ----------------------------------------------------------------------------------------------
// The following macros declare the pins to use for HW and SW SPI communication.
// SCK, MISO and MOSI should be connected to the HW SPI pins on the Uno when
// using HW SPI.  This should be used with nRF51822 based Bluefruit LE modules
// that use SPI (Bluefruit LE SPI Friend).
// ----------------------------------------------------------------------------------------------
#define BLUEFRUIT_SPI_CS               8
#define BLUEFRUIT_SPI_IRQ              7
#define BLUEFRUIT_SPI_RST              5    // Optional but recommended, set to -1 if unused

/* ...hardware SPI, using SCK/MOSI/MISO hardware SPI pins and then user selected CS/IRQ/RST */
Adafruit_BluefruitLE_SPI ble(BLUEFRUIT_SPI_CS, BLUEFRUIT_SPI_IRQ, BLUEFRUIT_SPI_RST);

//Error message helper - note that calling this will hang the application
void fatalError(const __FlashStringHelper*err) {
	Serial.println(err);
	while (1);
}

void sendBleMsg(char inputs[]) {
	if (ble.isConnected()) {
		ble.print("AT+BLEUARTTX=");
		ble.println(inputs);

		// check response status
		if (!ble.waitForOK()) {
			Serial.println(F("Failed to send?"));
		}
	}
	else {
		Serial.println(F("--BLE not connected"));
	}
}

void sendBleMsg(String input) {
	if (input.length() > 0) {
		int length = input.length() + 1;

		//The following line appears to be a bug, but it works fine with Arduino
		char buf[length];
		input.toCharArray(buf, length);

		sendBleMsg(buf);

		// Copy it over 
		//str.toCharArray(char_array, str_len);
	}
}

//END Adafruit BLE stuff

//BEGIN NeoPixel stuff

#define LED_PIN 4
#define LED_COUNT 10
Adafruit_NeoPixel pixels = Adafruit_NeoPixel(LED_COUNT, LED_PIN, NEO_GRB + NEO_KHZ800);

#define LED_CYCLE_NONE 0
#define LED_CYCLE_UP   1
#define LED_CYCLE_DOWN 2

int current_led_cycle = LED_CYCLE_NONE;

//BEGIN NeoPixel colors

uint32_t colorRed = pixels.Color(100, 0, 0);
uint32_t colorOrange = pixels.Color(140, 20, 0);
uint32_t colorYellow = pixels.Color(100, 100, 0);
uint32_t colorGreen = pixels.Color(0, 100, 0);
uint32_t colorBlue = pixels.Color(0, 0, 100);
uint32_t colorPurple = pixels.Color(100, 0, 100);
uint32_t colorOff = pixels.Color(0, 0, 0);

//END NeoPixel colors

uint32_t pixel0Color;
uint32_t pixel1Color;
uint32_t pixel2Color;
uint32_t pixel3Color;
uint32_t pixel4Color;
uint32_t pixel5Color;
uint32_t pixel6Color;
uint32_t pixel7Color;
uint32_t pixel8Color;
uint32_t pixel9Color;
uint32_t pixelTempColor;

void flashPixelBlue(int pixelIndex) {
	if (pixelIndex > -1 & pixelIndex < 10) {
		setSinglePixel(pixelIndex, colorBlue);

		delay(20);

		setPixelsOff();
	}
}

void cycleBlueUpCount(int count) {
	if (count > 0) {
		for (int j = 0; j < count; j++)
		{
			for (int i = 0; i < 10; i++)
			{
				flashPixelBlue(i);
			}
		}
	}
}

void setSinglePixel(int pixelIndex, uint32_t color) {
	if (pixelIndex > -1 & pixelIndex < 10) {
		if (pixelIndex == 0) {
			pixel0Color = color;
		}
		else {
			pixel0Color = colorOff;
		}

		if (pixelIndex == 1) {
			pixel1Color = color;
		}
		else {
			pixel1Color = colorOff;
		}

		if (pixelIndex == 2) {
			pixel2Color = color;
		}
		else {
			pixel2Color = colorOff;
		}

		if (pixelIndex == 3) {
			pixel3Color = color;
		}
		else {
			pixel3Color = colorOff;
		}

		if (pixelIndex == 4) {
			pixel4Color = color;
		}
		else {
			pixel4Color = colorOff;
		}

		if (pixelIndex == 5) {
			pixel5Color = color;
		}
		else {
			pixel5Color = colorOff;
		}

		if (pixelIndex == 6) {
			pixel6Color = color;
		}
		else {
			pixel6Color = colorOff;
		}

		if (pixelIndex == 7) {
			pixel7Color = color;
		}
		else {
			pixel7Color = colorOff;
		}

		if (pixelIndex == 8) {
			pixel8Color = color;
		}
		else {
			pixel8Color = colorOff;
		}

		if (pixelIndex == 9) {
			pixel9Color = color;
		}
		else {
			pixel9Color = colorOff;
		}

		setPixelColors();

		pixels.show();
	}
}

void setPixelColors() {
	pixels.setPixelColor(0, pixel0Color);
	pixels.setPixelColor(1, pixel1Color);
	pixels.setPixelColor(2, pixel2Color);
	pixels.setPixelColor(3, pixel3Color);
	pixels.setPixelColor(4, pixel4Color);
	pixels.setPixelColor(5, pixel5Color);
	pixels.setPixelColor(6, pixel6Color);
	pixels.setPixelColor(7, pixel7Color);
	pixels.setPixelColor(8, pixel8Color);
	pixels.setPixelColor(9, pixel9Color);
}

void setRainbow(boolean showInitialState) {
	pixel0Color = colorRed;
	pixel1Color = colorOrange;
	pixel2Color = colorYellow;
	pixel3Color = colorGreen;
	pixel4Color = colorBlue;
	pixel5Color = colorPurple;
	pixel6Color = colorRed;
	pixel7Color = colorOrange;
	pixel8Color = colorYellow;
	pixel9Color = colorGreen;

	setPixelColors();

	if (showInitialState) {
		pixels.show();
	}	
}

void setPixelsOff() {
	pixel0Color = colorOff;
	pixel1Color = colorOff;
	pixel2Color = colorOff;
	pixel3Color = colorOff;
	pixel4Color = colorOff;
	pixel5Color = colorOff;
	pixel6Color = colorOff;
	pixel7Color = colorOff;
	pixel8Color = colorOff;
	pixel9Color = colorOff;

	setPixelColors();

	pixels.show();
}

void cycleRainbowDown() {
	pixelTempColor = pixel0Color;
	pixel0Color = pixel1Color;
	pixel1Color = pixel2Color;
	pixel2Color = pixel3Color;
	pixel3Color = pixel4Color;
	pixel4Color = pixel5Color;
	pixel5Color = pixel6Color;
	pixel6Color = pixel7Color;
	pixel7Color = pixel8Color;
	pixel8Color = pixel9Color;
	pixel9Color = pixelTempColor;

	setPixelColors();

	pixels.show();
}

void cycleRainbowUp() {
	pixelTempColor = pixel9Color;
	pixel9Color = pixel8Color;
	pixel8Color = pixel7Color;
	pixel7Color = pixel6Color;
	pixel6Color = pixel5Color;
	pixel5Color = pixel4Color;
	pixel4Color = pixel3Color;
	pixel3Color = pixel2Color;
	pixel2Color = pixel1Color;
	pixel1Color = pixel0Color;
	pixel0Color = pixelTempColor;

	setPixelColors();

	pixels.show();
}

//END NeoPixel stuff

//BEGIN QWIIC Multiplexer stuff

boolean i2c_scan_complete = false;

#define QWIIC_MULTIPLEXER 0x70
boolean multiplexer_found = false;

#define QWIIC_RELAY				0x18
#define COMMAND_RELAY_OPEN		0x00
#define COMMAND_RELAY_CLOSED	0x01

#define LAUNCHER_0_PORT 7
#define LAUNCHER_1_PORT 6
#define LAUNCHER_2_PORT 5
#define LAUNCHER_3_PORT 4
#define LAUNCHER_4_PORT 3
#define LAUNCHER_5_PORT 2
#define LAUNCHER_6_PORT 1
#define LAUNCHER_7_PORT 0

#define LAUNCHER_NONE 10

//Enables a specific port number
boolean enableMuxPort(byte portNumber)
{
	if (portNumber > 7) portNumber = 7;

	//Read the current mux settings
	Wire.requestFrom(QWIIC_MULTIPLEXER, 1);
	if (!Wire.available()) return(false); //Error
	byte settings = Wire.read();

	//Set the wanted bit to enable the port
	settings |= (1 << portNumber);

	Wire.beginTransmission(QWIIC_MULTIPLEXER);
	Wire.write(settings);
	Wire.endTransmission();

	return(true);
}

//Disables a specific port number
boolean disableMuxPort(byte portNumber)
{
	if (portNumber > 7) portNumber = 7;

	//Read the current mux settings
	Wire.requestFrom(QWIIC_MULTIPLEXER, 1);
	if (!Wire.available()) return(false); //Error
	byte settings = Wire.read();

	//Clear the wanted bit to disable the port
	settings &= ~(1 << portNumber);

	Wire.beginTransmission(QWIIC_MULTIPLEXER);
	Wire.write(settings);
	Wire.endTransmission();

	return(true);
}

// RelayClose() turns on the relay at the qwiicRelayAddress
void relayClose() {
	Wire.beginTransmission(QWIIC_RELAY);
	Wire.write(COMMAND_RELAY_CLOSED);
	Wire.endTransmission();
}


// RelayOpen() turns off the relay at the qwiicRelayAddress
void relayOpen() {
	Wire.beginTransmission(QWIIC_RELAY);
	Wire.write(COMMAND_RELAY_OPEN);
	Wire.endTransmission();
}
//END QWIIC Multiplexer stuff

const int LOOP_DELAY_MS = 50;
int selectLauncher;

boolean launcher0Burnt = false;
boolean launcher1Burnt = false;
boolean launcher2Burnt = false;
boolean launcher3Burnt = false;
boolean launcher4Burnt = false;
boolean launcher5Burnt = false;
boolean launcher6Burnt = false;
boolean launcher7Burnt = false;

byte selectLauncherPort(int launcher) {
	byte result = LAUNCHER_NONE;

	if (launcher == 0) {
		result = LAUNCHER_0_PORT;
	}
	else if (launcher == 1) {
		result = LAUNCHER_1_PORT;
	}
	else if (launcher == 2) {
		result = LAUNCHER_2_PORT;
	}
	else if (launcher == 3) {
		result = LAUNCHER_3_PORT;
	}
	else if (launcher == 4) {
		result = LAUNCHER_4_PORT;
	}
	else if (launcher == 5) {
		result = LAUNCHER_5_PORT;
	}
	else if (launcher == 6) {
		result = LAUNCHER_6_PORT;
	}
	else if (launcher == 7) {
		result = LAUNCHER_7_PORT;
	}

	return result;
}

void closeLauncherRelay(int launcher) {
	byte port = selectLauncherPort(launcher);
	if (port < 8) {
		enableMuxPort(port);
		relayClose();
	}
}

void openLauncherRelay(int launcher) {
	byte port = selectLauncherPort(launcher);
	if (port < 8) {
		relayOpen();
		disableMuxPort(port);		
	}
}

void doLaunchSequence(int launcher) {
	if (launcher > -1 & launcher < 9) {
		//Do some warning rainbow flashing
		setRainbow(false);
		for (int i = 0; i < 30; i++)
		{
			cycleRainbowUp();
			delay(50);
		}

		setPixelsOff();
		delay(500);
		
		int colorDelay = 200;

		for (int i = 9; i > -1; i--)
		{
			setSinglePixel(i, colorRed);
			delay(colorDelay);
			setSinglePixel(i, colorOrange);
			delay(colorDelay);
			setSinglePixel(i, colorYellow);
			delay(colorDelay);
			setSinglePixel(i, colorGreen);
			delay(colorDelay);
			setSinglePixel(i, colorBlue);
			delay(colorDelay);
			setSinglePixel(i, colorPurple);
			delay(colorDelay);
			setPixelsOff();
			delay(colorDelay);
		}

		int closedRelayCount = 4;  //Adjust this count to keep relay open longer
		boolean launched = false;

		//Burn launcher 0 - if selected
		if ((!launcher0Burnt) & (launcher == 0 | launcher == 8)) {
			cycleBlueUpCount(2);
			closeLauncherRelay(0);
			launched = true;

			cycleBlueUpCount(closedRelayCount);
			openLauncherRelay(0);

			launcher0Burnt = true;
		}

		//Burn launcher 1 - if selected
		if ((!launcher1Burnt) & (launcher == 1 | launcher == 8)) {
			cycleBlueUpCount(2);
			closeLauncherRelay(1);
			launched = true;

			cycleBlueUpCount(closedRelayCount);
			openLauncherRelay(1);

			launcher1Burnt = true;
		}

		//Burn launcher 2 - if selected
		if ((!launcher2Burnt) & (launcher == 2 | launcher == 8)) {
			cycleBlueUpCount(2);
			closeLauncherRelay(2);
			launched = true;

			cycleBlueUpCount(closedRelayCount);
			openLauncherRelay(2);

			launcher2Burnt = true;
		}

		//Burn launcher 3 - if selected
		if ((!launcher3Burnt) & (launcher == 3 | launcher == 8)) {
			cycleBlueUpCount(2);
			closeLauncherRelay(3);
			launched = true;

			cycleBlueUpCount(closedRelayCount);
			openLauncherRelay(3);

			launcher3Burnt = true;
		}

		//Burn launcher 4 - if selected
		if ((!launcher4Burnt) & (launcher == 4 | launcher == 8)) {
			cycleBlueUpCount(2);
			closeLauncherRelay(4);
			launched = true;

			cycleBlueUpCount(closedRelayCount);
			openLauncherRelay(4);

			launcher4Burnt = true;
		}

		//Burn launcher 5 - if selected
		if ((!launcher5Burnt) & (launcher == 5 | launcher == 8)) {
			cycleBlueUpCount(2);
			closeLauncherRelay(5);
			launched = true;

			cycleBlueUpCount(closedRelayCount);
			openLauncherRelay(5);

			launcher5Burnt = true;
		}

		//Burn launcher 6 - if selected
		if ((!launcher6Burnt) & (launcher == 6 | launcher == 8)) {
			cycleBlueUpCount(2);
			closeLauncherRelay(6);
			launched = true;

			cycleBlueUpCount(closedRelayCount);
			openLauncherRelay(6);

			launcher6Burnt = true;
		}

		//Burn launcher 7 - if selected
		if ((!launcher7Burnt) & (launcher == 7 | launcher == 8)) {
			cycleBlueUpCount(2);
			closeLauncherRelay(7);
			launched = true;

			cycleBlueUpCount(closedRelayCount);
			openLauncherRelay(7);

			launcher7Burnt = true;
		}

		if (launched) {
			cycleBlueUpCount(2);
		}

		sendBleMsg("LC");
	}
}

// the setup function runs once when you press reset or power the board
void setup() {
	//Initialize NeoPixels
	pixels.begin();

	//Initialize Wire library
	Wire.begin();

	Serial.begin(115200);
	Serial.println("Starting application...");

	setPixelsOff();

	//BEGIN Setup code for Adafruit BLE

/* Initialise the module */
	Serial.print(F("Initialising the Bluefruit LE module: "));

	if (!ble.begin(VERBOSE_MODE))
	{
		fatalError(F("Couldn't find Bluefruit, make sure it's in CoMmanD mode & check wiring?"));
	}
	Serial.println(F("OK!"));

	if (FACTORYRESET_ENABLE)
	{
		/* Perform a factory reset to make sure everything is in a known state */
		Serial.println(F("Performing a factory reset: "));
		if (!ble.factoryReset()) {
			fatalError(F("Couldn't factory reset"));
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

	/* NOT DOING THIS - Wait for connection */
	//while (!ble.isConnected()) {
	//	delay(500);
	//}

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

				if (address == QWIIC_MULTIPLEXER) {
					multiplexer_found = true;
					Serial.println("QWIIC Multiplexer found.");
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

		if (multiplexer_found) {
			//BEGIN Setup code for SparkFun QWIIC/I2C Multiplexer

			for (byte i = 0; i < 8; i++)
			{
				disableMuxPort(i);
			}

			//END Setup code for SparkFun QWIIC/I2C Multiplexer
		}

		i2c_scan_complete = true;
	}

	// Check for incoming characters from Bluefruit
	if (ble.isConnected()) {
		ble.println("AT+BLEUARTRX");
		ble.readline();
		if (!(strcmp(ble.buffer, "OK") == 0))
		{
			if (ble.buffer[0] == 'C' & ble.buffer[1] == 'U') {
				Serial.println("LED cycling up...");
				current_led_cycle = LED_CYCLE_UP;
				setRainbow(false);
			}
			else if (ble.buffer[0] == 'C' & ble.buffer[1] == 'D') {
				Serial.println("LED cycling down...");
				current_led_cycle = LED_CYCLE_DOWN;
				setRainbow(false);
			}
			else if (ble.buffer[0] == 'C' & ble.buffer[1] == 'O') {
				Serial.println("Stopping LED cycle...");
				current_led_cycle = LED_CYCLE_NONE;
				setPixelsOff();
			}
			else if (ble.buffer[0] == 'R' & ble.buffer[1] == 'S') {
				Serial.println("Resetting launchers...");
				launcher0Burnt = false;
				launcher1Burnt = false;
				launcher2Burnt = false;
				launcher3Burnt = false;
				launcher4Burnt = false;
				launcher5Burnt = false;
				launcher6Burnt = false;
				launcher7Burnt = false;
				cycleBlueUpCount(1);
			}
			else if (ble.buffer[0] == 'L') {
				selectLauncher = -1;

				if (ble.buffer[1] == '0') {
					selectLauncher = 0;
				}
				else if (ble.buffer[1] == '1') {
					selectLauncher = 1;
				}
				else if (ble.buffer[1] == '2') {
					selectLauncher = 2;
				}
				else if (ble.buffer[1] == '3') {
					selectLauncher = 3;
				}
				else if (ble.buffer[1] == '4') {
					selectLauncher = 4;
				}
				else if (ble.buffer[1] == '5') {
					selectLauncher = 5;
				}
				else if (ble.buffer[1] == '6') {
					selectLauncher = 6;
				}
				else if (ble.buffer[1] == '7') {
					selectLauncher = 7;
				}
				else if (ble.buffer[1] == '8') {
					selectLauncher = 8;
				}

				if (selectLauncher > -1) {
					Serial.println("Launch sequence!");
					doLaunchSequence(selectLauncher);
				}
			}
			else {
				// Some data was found, its in the buffer
				Serial.print(F("[Recv] "));
				Serial.println(ble.buffer);
			}
			ble.waitForOK();
		}
	}

	if (current_led_cycle == LED_CYCLE_UP) {
		cycleRainbowUp();
	}
	else if (current_led_cycle == LED_CYCLE_DOWN) {
		cycleRainbowDown();
	}

	delay(LOOP_DELAY_MS); //loop delay
}
