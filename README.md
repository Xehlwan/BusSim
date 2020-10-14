# BusSim
A simple bus simulation. Solution for school exercise. Demonstrates the use of collections, encapsulation, interop with unmanaged code, and optimization of calls to the console host to optimize performance.

This simulation uses the Windows API to enable Virtual Terminal commands in the windows console. By utilizing VT commands, the console supports full RGB colors.

The program is configured through the use of constants in the Program-class, but it does't check for sane or supported values - use these with caution.
