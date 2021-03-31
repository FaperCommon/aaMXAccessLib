#  Wrapper for MXAccess Toolkit (Archestra, Wonderware).
Before starting work, you must have registered MXAccess.
Module is designed to read data from Archestra objects.
MXAccess does not have support for x64 architecture. For this reason, for use in non-x86 applications, you must use the service.
Any service implementation for deployment is possible (MXAccessMXServiceExample is a simple example of a service). For 

## Server initialization and registration

```C#
	var service = new aaMXManager("TestServer");
	service.Register();
```
## Add Items
After registration, to start listening for objects, you need to initialize instances of aaAttribute (or IEnumerable for multiple objects) with a filled TagName prop (objects with missing TagName will not be added).

```C#	
	var item = new aaAttribute(){TagName = "aaobject.Attr"};
	AddItem(item);
	AdviseAll(); //Advise(item.TagName); For one object
```
## GetData
The data is updated directly in the registered objects.
To receive data, it is possible to implement INotifyPropertyChanged for VTQ (Value, TimeStamp, Quality) properties of aaAttribute.
For remote services, you must use the method 
```C#	
	GetCollection();
```
## Poke
```C#	
	Poke(item.TagName, 22);
```	
After completing the work, you need to call Unregister();

## Troubleshooting
In Visual Studio, set your project to use "Prefer 32-bit". Project Properties → Build → "Prefer 32-bit" in Platform target. The code should be compiled as 32-bit.

