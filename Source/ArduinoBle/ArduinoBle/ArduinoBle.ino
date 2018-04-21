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

#include <Wire.h>

#define JOYSTICK 0x20
#define KEYPAD 0x4B

const int ONBOARD_LED = 13;

const int GREEN_BUTTON = 3;
const int BLUE_BUTTON = 4;
const int RED_BUTTON = 5;
const int YELLOW_BUTTON = 6;

const byte KEY_STAR = 0x2A;
const byte KEY_HASH = 0x23;
const byte KEY_0 = 0x30;
const byte KEY_1 = 0x31;
const byte KEY_2 = 0x32;
const byte KEY_3 = 0x33;
const byte KEY_4 = 0x34;
const byte KEY_5 = 0x35;
const byte KEY_6 = 0x36;
const byte KEY_7 = 0x37;
const byte KEY_8 = 0x38;
const byte KEY_9 = 0x39;

const byte JOY_CENTER = 0x01;
const byte JOY_N = 0x02;
const byte JOY_NE = 0x03;
const byte JOY_E = 0x04;
const byte JOY_SE = 0x05;
const byte JOY_S = 0x06;
const byte JOY_SW = 0x07;
const byte JOY_W = 0x08;
const byte JOY_NW = 0x09;

const byte JOY_CENTER_H = 0x01;
const byte JOY_LEFT = 0x02;
const byte JOY_RIGHT = 0x03;

const byte JOY_CENTER_V = 0x01;
const byte JOY_UP = 0x02;
const byte JOY_DOWN = 0x03;

const byte JOY_BUTTON_DOWN = 0x00;

const int CYCLE_MILLISECONDS = 100;

int green_button_state = LOW;
int blue_button_state = LOW;
int red_button_state = LOW;
int yellow_button_state = LOW;

int prev_green_button_state = LOW;
int prev_blue_button_state = LOW;
int prev_red_button_state = LOW;
int prev_yellow_button_state = LOW;

byte prev_joystick_direction = JOY_CENTER;
boolean prev_joystick_button_state = false;

boolean scan_complete = false;

//BEGIN joystick functions

int reading_joystick = 0;

// getJoystickHorizontal() returns an int between 0 and 1023
// representing the Joystiic's horizontal position
// (axis indicated by silkscreen on the board)
// centered roughly on 512
int getJoystickHorizontal() {
	Wire.beginTransmission(JOYSTICK);
	Wire.write(0x00);
	Wire.endTransmission(false);
	Wire.requestFrom(JOYSTICK, 2);

	while (Wire.available()) {
		uint8_t msb = Wire.read();
		uint8_t lsb = Wire.read();
		reading_joystick = (uint16_t)msb << 8 | lsb;
	}

	Wire.endTransmission();
	return reading_joystick;
}

// getJoystickVertical() returns an int between 0 and 1023
// representing the Joystiic's vertical position
// (axis indicated by silkscreen on the board)
// centered roughly on 512
int getJoystickVertical() {
	Wire.beginTransmission(JOYSTICK);
	Wire.write(0x02);
	Wire.endTransmission(false);
	Wire.requestFrom(JOYSTICK, 2);

	while (Wire.available()) {
		uint8_t msb = Wire.read();
		uint8_t lsb = Wire.read();
		reading_joystick = (uint16_t)msb << 8 | lsb;
	}

	Wire.endTransmission();
	return reading_joystick;
}

// getJoystickButton() returns a byte indicating the
// position of the button where a 0b1 is not
// pressed and 0b0 is pressed
byte getJoystickButton() {
	byte btnbyte = 0b00000000;

	Wire.beginTransmission(JOYSTICK);
	Wire.write(0x04);
	Wire.endTransmission(false);
	Wire.requestFrom(JOYSTICK, 1);    // request 1 byte

	while (Wire.available()) {
		btnbyte = Wire.read();
	}

	Wire.endTransmission();
	return btnbyte;
}

//END joystick functions

void setup() {
	//Initialize Wire library
	Wire.begin();

	Serial.begin(115200);
	Serial.println("Starting application...");

	// initialize digital pin 13 as an output.
	pinMode(ONBOARD_LED, OUTPUT);

	// initialize digital pins 4-7 as inputs
	pinMode(GREEN_BUTTON, INPUT);
	pinMode(BLUE_BUTTON, INPUT);
	pinMode(RED_BUTTON, INPUT);
	pinMode(YELLOW_BUTTON, INPUT);
}

// the loop function runs over and over again forever
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

	Wire.requestFrom(KEYPAD, 1);
	byte keyboard_byte = Wire.read();
	switch (keyboard_byte)
	{
	case KEY_STAR:
		Serial.println("KEY * pressed.");
		break;
	case KEY_HASH:
		Serial.println("KEY # pressed.");
		break;
	case KEY_0:
		Serial.println("KEY 0 pressed.");
		break;
	case KEY_1:
		Serial.println("KEY 1 pressed.");
		break;
	case KEY_2:
		Serial.println("KEY 2 pressed.");
		break;
	case KEY_3:
		Serial.println("KEY 3 pressed.");
		break;
	case KEY_4:
		Serial.println("KEY 4 pressed.");
		break;
	case KEY_5:
		Serial.println("KEY 5 pressed.");
		break;
	case KEY_6:
		Serial.println("KEY 6 pressed.");
		break;
	case KEY_7:
		Serial.println("KEY 7 pressed.");
		break;
	case KEY_8:
		Serial.println("KEY 8 pressed.");
		break;
	case KEY_9:
		Serial.println("KEY 9 pressed.");
		break;
	default:
		break;
	}

	if (getJoystickButton() == JOY_BUTTON_DOWN) {
		if (!prev_joystick_button_state) {
			Serial.println("JOYSTICK button pressed.");
		}
		prev_joystick_button_state = true;
	}
	else {
		prev_joystick_button_state = false;
	}

	int joystickH = getJoystickHorizontal();
	int joystickV = getJoystickVertical();
	byte joystickButton = getJoystickButton();

	byte joystick_direction = JOY_CENTER;

	byte joystick_horiz = JOY_CENTER_H;
	if (joystickH < (512 - 40)) {
		joystick_horiz = JOY_LEFT;
	}
	else if (joystickH >(512 + 40)) {
		joystick_horiz = JOY_RIGHT;
	}

	byte joystick_vert = JOY_CENTER_H;
	if (joystickV < (512 - 40)) {
		joystick_vert = JOY_UP;
	}
	else if (joystickV >(512 + 40)) {
		joystick_vert = JOY_DOWN;
	}

	if (joystick_horiz == JOY_CENTER_H & joystick_vert == JOY_UP) {
		joystick_direction = JOY_N;
	}
	else if (joystick_horiz == JOY_RIGHT & joystick_vert == JOY_UP) {
		joystick_direction = JOY_NE;
	}
	else if (joystick_horiz == JOY_RIGHT & joystick_vert == JOY_CENTER_V) {
		joystick_direction = JOY_E;
	}
	else if (joystick_horiz == JOY_RIGHT & joystick_vert == JOY_DOWN) {
		joystick_direction = JOY_SE;
	}
	else if (joystick_horiz == JOY_CENTER_H & joystick_vert == JOY_DOWN) {
		joystick_direction = JOY_S;
	}
	else if (joystick_horiz == JOY_LEFT & joystick_vert == JOY_DOWN) {
		joystick_direction = JOY_SW;
	}
	else if (joystick_horiz == JOY_LEFT & joystick_vert == JOY_CENTER_V) {
		joystick_direction = JOY_W;
	}
	else if (joystick_horiz == JOY_LEFT & joystick_vert == JOY_UP) {
		joystick_direction = JOY_NW;
	}

	if (joystick_direction != prev_joystick_direction) {
		if (joystick_direction == JOY_CENTER) {
			Serial.println("JOYSTICK is CENTERED.");
		}
		else if (joystick_direction == JOY_N) {
			Serial.println("JOYSTICK is NORTH.");
		}
		else if (joystick_direction == JOY_NE) {
			Serial.println("JOYSTICK is NORTHEAST.");
		}
		else if (joystick_direction == JOY_E) {
			Serial.println("JOYSTICK is EAST.");
		}
		else if (joystick_direction == JOY_SE) {
			Serial.println("JOYSTICK is SOUTHEAST.");
		}
		else if (joystick_direction == JOY_S) {
			Serial.println("JOYSTICK is SOUTH.");
		}
		else if (joystick_direction == JOY_SW) {
			Serial.println("JOYSTICK is SOUTHWEST.");
		}
		else if (joystick_direction == JOY_W) {
			Serial.println("JOYSTICK is WEST.");
		}
		else if (joystick_direction == JOY_NW) {
			Serial.println("JOYSTICK is NORTHWEST.");
		}

		prev_joystick_direction = joystick_direction;
	}

	green_button_state = digitalRead(GREEN_BUTTON);
	blue_button_state = digitalRead(BLUE_BUTTON);
	red_button_state = digitalRead(RED_BUTTON);
	yellow_button_state = digitalRead(YELLOW_BUTTON);

	if (green_button_state != prev_green_button_state) {
		if (green_button_state == HIGH) {
			Serial.println("GREEN button pressed.");
		}
		prev_green_button_state = green_button_state;
	}

	if (blue_button_state != prev_blue_button_state) {
		if (blue_button_state == HIGH) {
			Serial.println("BLUE button pressed.");
		}
		prev_blue_button_state = blue_button_state;
	}

	if (red_button_state != prev_red_button_state) {
		if (red_button_state == HIGH) {
			Serial.println("RED button pressed.");
		}
		prev_red_button_state = red_button_state;
	}

	if (yellow_button_state != prev_yellow_button_state) {
		if (yellow_button_state == HIGH) {
			Serial.println("YELLOW button pressed.");
		}
		prev_yellow_button_state = yellow_button_state;
	}

	delay(CYCLE_MILLISECONDS); //loop delay
}
