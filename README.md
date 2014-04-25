USB_Voltmeter
=============

USB Voltmeter / USB DC Energy Meter

** Introduction **

To conduct experiments on DC engine recuperation, it was required to take series of measurements of consumed/produced energy in multiple parts of the electrical schema. For this purposes it was decided to make basic USB voltmeters and calculate other readings in real-time in this DC Energy Meter application. As a basis for the schema and firmware another USB Frequency Counter & Voltmeter (http://www.moty22.co.uk/usb_counter.php) was taken.

DC Energy Meter was developed from scratch, using data parsing algorithms from the basic USB counter and some other articles on USB interop techniques.

** Background **

This application was developed using DDD & TDD and uses free modern technologies available on the web, like Fody.PropertyChanged, Fody.Costura. The challenge was to make application design suitable for both: WinForms and WPF. MVVM pattern choice was obvious.

For the time being only WinForms version of this application is available.

** Using the code ** 

To test application functionality it can be launched with “-e” command line switch to use wave signal emulator, instead of real device.

** Points of Interest **

Hopefully, this sample might be a good starting point for developing this sort of applications.

** History **

02-Apr-14 - ver. 1.0.11.0  - Initial application version using WinForms UI 

** License **

This article, along with any associated source code and files, is licensed under The MIT License