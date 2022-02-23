# Introduction
`NArgs` is an easy and extensible way to handle arguments (e.g. Command line parameters) in .NET.

# Extensibility
Command line arguments could be parsed by specialized parsers to support different argument sources. There will be a small number of basic parsers at the beginning. If you need more please do not hesitate to contact the owners of this project.

# Argument Parsers
Parsers are used to analyze the given arguments

The following parsers are currently available

- ConsoleCommandLineParser

# Getting Started
The concept of `NArgs` is based on the idea of having a configuration instance which can be enhanced with attributes to be mapped to given arguments (e.g. from the command line). With this you can reuse a given configuration class and enrich this with additional features without writing additional code logic.

## Decorators - Attributes for Options and Parameters
The concept is to add decorators for a public property and map this property to a specific argument. There are two kind of attributes provided: Options and Parameters. Options are not restricted to a specific position and identified by there names.

Options are the most used variant of parsing arguments. They can be used to handle switched (e.g. turn on/off an option like "verbosity" of messages or very specific one like referring to an existing file). An Option attribute has some properties which can be used to map an argument to a configuration property.

Assume you want an option "help" (a very common option to show a help page). Assume also that you want to provide an option for this and want to access it with the following arguments: -h, /h, --help, -?, /?

_Note_: Arguments will automatically be processed with either "-" or "/" (windows style) or the long-name variant of "--".

For this you can use the following attribute properties:

|Property        | Description                                                                        |
|----------------|------------------------------------------------------------------------------------|
|Name            | This name is the default name of an attribute and identifies the option (e.g. "h") |
|LongName        | This name is the verbose variant of a name (e.g. "help")                           |
|AlternativeName | This name is used if an option shall support multiple name variants (e.g. "?")     |

_Code Example_:

```C#
 public class ExampleConfig
  {
    [Option(Name = "h", LongName = "help", AlternativeName = "?")]
    public bool ShowHelp
    {
      get;
      set;
    }
  }
```

In this case you want a simple switch which is represented by a `bool`data type. The following sections will show what kind of .NET standard data types are also built-in by default. I you need more support of standard types just drop a note to the project owners. If you need support of complex/user-defined data types see the example below. 

Parameters are a list of values passed to the application. They do have a strict sequence (parameter #1, parameter #2 etc.). Because of this parameters have always be added AFTER all options. Otherwise it would not be possible to differentiate between option values and parameters.

## Values of Option Arguments
An option has always a value that comes along with it. If you do not specify a value it is handled as if it was set to blank - normally this would lead to an error, but for a boolean option is is treated as if it was set to `true`.

A value can be provided with the following notations

- \<option\> \<value\>
- \<option\>:\<value\>

Examples:

#### Setting an option "Verbose" (bool)
You can use `--verbose false` or `--verbose:false` to set a bool option to `false`. If you just want to set its value to `true` you can simply use `--verbose`.

In addition you can use alternative bool expressions like: yes, y, no, n, on, off, 1 or 0.

For this you can also use `--verbose yes` or `--verbose:on`

#### Setting an option "Number" (integer) to a valueS
You can either use `--number 123` or `--number:123`. If you just simply use `--number` it would fail because an empty value is not a valid integer value.

#### Setting an option "Name" (string) to a value
You can either use `--name John` or `--name:John`. If you just simply use `--name` it would set the value to `String.Empty`. In case you want to support complex string values that contains spaces you have to use quotes. With that use an argument expression like `--name:"John Doe"`.

## Supported Build-In Types
There are some built-in .NET data types which are supported directly by `NArgs`.

|.NET Data Type   |
|-----------------|
| bool            |
| Char            |
| DateTime        |
| DirectoryInfo   |
| Double          |
| FileInfo        |
| Int16 / short   |
| Int32 / int     |
| Int64 / long    |
| SecureString    |
| Single / float  |
| String          |
| UInt16 / ushort |
| UInt32 / uint   |
| UInt64 / ulong  |
| Uri             |

## User-Defined Types
If you need to map arguments to a complex type (e.g. class) you can do this by registering a user-defined type for that.

Here is a brief example of how to use an enumeration `Color` for configuration:

1. Define the enumeration
```C#
  public enum Color
  {
    None,
    Red,
    Green,
    Yellow
  }
```

2. Add decorators to a configuration property of data type `Color`
```C#
  public class Config
  {
    [Option(LongName = "color-name", Name = "color")]
    public Color Color
    {
      get;
      set;
    }
  }
```

3. Register the user-defined type handler for mapping and validation
```C#
  ConsoleCommandLineParser oParser = new ConsoleCommandLineParser();
  
  oParser.RegisterCustomDataTypeHandler(typeof(Color), (name, value) =>
    {
      switch (value)
      {
        case "Red":
          return Color.Red;
          
         case "Yellow":
          return Color.Yellow;

        case "Green":
          return Color.Green;

        default:
          return Color.None;
      }
    },

    (name, value, required) =>
    {
      return new string[] { "None", "Red", "Yellow", "Green" }.Contains(value);
    });
```
## Show Usage
Any application will need to show the supported arguments. For this the argument parser is able to generate a usage information with all parameters and options (optional and required) and how they can be passed to the application. This method is called `GetUsage()` and will generate a `string` which can be post-processed by the application.

## IoC Support
`NArgs` is designed for the use in an IoC (Inversion of Control scenario) environment. For that it provides an interface `IArgumentParser`. All provided parsers of `NArgs` implement this interface.
