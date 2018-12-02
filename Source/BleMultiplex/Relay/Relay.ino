/*
 Name:		Relay.ino
 Created:	12/1/2018 3:41:09 PM
 Author:	jerem
*/

//SoftwareSerial library needed for board-to-board communications
#include <SoftwareSerial.h>

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

const int LOOP_DELAY_MILLISECONDS = 1000;

//BEGIN SoftSerial on pins 10 & 11

SoftwareSerial inSerial(10, 11); // RX, TX
String messageDelimiter = "*";
String serialMessage;

//END SoftSerial on pins 10 & 11

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
// ----------------------------------------------------------------------------------------------
// The following macros declare the pins to use for HW and SW SPI communication.
// SCK, MISO and MOSI should be connected to the HW SPI pins on the Uno when
// using HW SPI.  This should be used with nRF51822 based Bluefruit LE modules
// that use SPI (Bluefruit LE SPI Friend).
// ----------------------------------------------------------------------------------------------
#define BLUEFRUIT_SPI_CS               8
#define BLUEFRUIT_SPI_IRQ              7
#define BLUEFRUIT_SPI_RST              4    // Optional but recommended, set to -1 if unused

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

//END Adafruit BLE settings

// the setup function runs once when you press reset or power the board
void setup() {
	//Serial output for debugging
	Serial.begin(115200);
	Serial.println("Starting application...");

	//SoftwareSerial out port- set the data rate for the port
	inSerial.begin(9600);

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
	if (inSerial.available()) {
		String incoming = inSerial.readStringUntil('\n');

		if (incoming.length() > 0) {
			Serial.println("Incoming: " + incoming);

			int charLength = incoming.length() + 1;
			sendBleMsg(incoming);
		}
	}

	//delay(LOOP_DELAY_MILLISECONDS); //loop delay
}
