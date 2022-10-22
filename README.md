# CASSEL
## Colourful Assembly Language

Colourful Assembly Language, CASSEL, is a simplistic transpiled programming language making use of only HTML colour codes to write functional, Turing-Complete* code.

### How does it work?
The compiler reads commands from colour codes in the pixels of a .PNG image and transpiles it into C++ code that you can compile and run yourself. The pixels are read from left to right, top to bottom. Any pixels that do not have an Alpha value of **FF** (= 255) will be ignored and treated like comments.

### Features of the language
1. Input and output
2. Addition and subtraction
3. Loops
4. Data storage in memory

### Hexadecimal register system
CASSEL features a three-way input system for calling registers to perform a certain function. The values in the Green and Blue channels are always automatically converted to int before use.
| Channel | Argument |
|--------|--------------|
| Red | Chosen register |
| Green | Variable address |
| Blue | Argument value |

### Registers
| **Register** | **Method**  |
|--------------|-------------|
| **FF**       | Assignment  |
| **FA**       | Addition    |
| **F0**       | Subtraction |
| **CA**       | Input       |
| **C0**       | Output      |
| **FE**       | Loop Begin      |
| **CE**       | Loop End      |

### Fixed Values
Due to CASSEL's limitation by the max number, **FF** (= 255), that can be presented in a single channel, CASSEL features a fixed value as seen below.

| **Hex Code** | **Behaviour** |
|--------|-------------|
|**CC**|Get value from next colour|

When the hex code **CC** is used in the Green or Blue channel, this tells CASSEL to look at the following colour code to get its information for that specific argument.
This means the limit of **FF** (= 255) is removed and instead can hold up to **FFFFFF** (= 16,777,215).

Therefore, if you have reached the soft limit of 255, you can replace the channel value with **CC** and specify a greater-length number/address in the following colour code. If you use **CC** in both the Green channel and the Blue channel, you must specify two following colour codes containing values to be read into the Green and Blue channel respectively.

Example:
```CASSEL
#FFCCCC
#000539
#0001A4
```

In the above example, the **FF** register is used in the Red channel to declare a variable. The **CC** in the Green channel specifies that the entire following colour code (0x539) will be used as a variable address. Because **CC** has been used in the Green channel already, **CC** in the Blue channel will now tell the compiler to use the second colour code specified (0x1A4) as a value to be set to the variable with the address specified in the Green channel.

## Documentation
This section will cover the usage and arguments of all 7 registers in the language.

### Assignment Register - **FF**
Declares a variable with a given address and an assigned integer value within the current scope.

| **Red** | **Green** | **Blue** |
|-------|-------|-------|
|**FF**|Variable Address|Value|

Variable Address - The name of the variable declared.
Value - The value assigned to the variable.

Example:
CASSEL (HTML Colour Code Translation)
```CASSEL
#FF010C
```
C++ (Generated)
```c++
int v_1 = 12;
```

### Addition Register - **FA**
Performs an addition operation on a variable with a given address using an assigned integer value.

| **Red** | **Green** | **Blue** |
|-------|-------|-------|
|**FA**|Variable Address|Value Added|

Variable Address - The name of the variable being added to.
Valued Added - The amount being added to the variable.

Example:
CASSEL (HTML Colour Code Translation)
```CASSEL
#FA0103
```
C++ (Generated)
```cpp
v_1 += 3;
```

### Subtraction Register - **F0**
Performs a subtraction operation on a variable with a given address using an assigned integer value.

| **Red** | **Green** | **Blue** |
|-------|-------|-------|
|**F0**|Variable Address|Value Subtracted|

Variable Address - The name of the variable being subtracted from.
Valued Added - The amount being subtracted from the variable.

Example:
CASSEL (HTML Colour Code Translation)
```CASSEL
#F00F0A
```
C++ (Generated)
```cpp
v_15 -= 10;
```

### Input Register - **CA**
Gets single-character input from the user with stdio, converts the character to an integer from UTF-8 and stores the value in a variable.

| **Red** | **Green** | **Blue** |
|-------|-------|-------|
|**CA**|Variable Address|-|

Variable Address - The name of the variable in which the user's input is stored.

Example:
CASSEL (HTML Colour Code Translation)
```CASSEL
#FF0100
#CA0100
```
C++ (Generated)
```cpp
int v_1 = 0;
char v_xxxx{};
std::cin.get(v_xxxx);
v_1 = int(v_xxxx);
```
_xxxx is a placeholder for a compiler-generated UUID_

### Output Register - **C0**
Prints out a single character from an integer stored in a variable with a given address. The integer is converted into a UTF-8 character before being outputted to the console.

| **Red** | **Green** | **Blue** |
|-------|-------|-------|
|**C0**|Variable Address|-|

Variable Address - The name of the variable in which the value that will be printed out is stored.

Example:
CASSEL (HTML Colour Code Translation)
```CASSEL
#FF0161
#C00100
```
C++ (Generated)
```cpp
int v_1 = 97;
std::cout << char(v_1);
```

### Loop Registers - **FE & CE**
Begins a loop over the code between this register and the closing register. Loops the number of times specified by the value in the variable with the given variable address.

| **Red** | **Green** | **Blue** |
|-------|-------|-------|
|**FE**|Variable Address|-|
|**CE**|-|-|

Variable Address - The variable containing the amount of times to run the loop.

Example:
CASSEL (HTML Colour Code Translation)
```CASSEL
#FF0061
#FF0105
#FE0100
#C00200
#FA0001
#CE0000
```
C++ (Generated)
```cpp
int v_0 = 97;
int v_1 = 5;
for (int v_xxxx = 0; v_xxxx < v_1; v_xxxx++)
{
	std::cout << char(v_0);
	v_0 += 1;
}
```
_xxxx is a placeholder for a compiler-generated UUID_
