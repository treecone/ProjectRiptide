# Project Riptide Style Guide 

Adapted from The Official raywenderlich.com C# Style Guide
https://github.com/raywenderlich/c-sharp-style-guide

## Table of Contents

- [Unity Habits](#unity-habits)
- [Nomenclature](#nomenclature)
  + [Namespaces](#namespaces)
  + [Classes & Interfaces](#classes--interfaces)
  + [Methods](#methods)
  + [Fields](#fields)
  + [Parameters](#parameters--parameters)
  + [Delegates](#delegates--delegates)
  + [Events](#events--events)
  + [Misc](#misc)
- [Declarations](#declarations)
  + [Access Level Modifiers](#access-level-modifiers)
  + [Fields & Variables](#fields--variables)
  + [Classes](#classes)
  + [Interfaces](#interfaces)
- [Spacing](#spacing)
  + [Indentation](#indentation)
  + [Line Length](#line-length)
  + [Vertical Spacing](#vertical-spacing)
- [Brace Style](#brace-style)
- [Switch Statements](#switch-statements)
- [Language](#language)
- [Emoji](#emoji)
- [Credit](#credits)

## Unity Habits

### Access Level Modifiers

Fields should only be `public` if they need to be referenced in other scripts.
If a variable is to be changed in the Unity Editor, use `[SerializeField]` rather than `public`

### Properties

Public `get` and `set` properties should be used in place of public fields.

### Script naming

Scripts should not contain "Script" in their name.

**AVOID**:

```csharp
ShipMovementScript.cs
```

**PREFER**:

```csharp
ShipMovement.cs
```

### Find

The `GameObject.Find` and `GameObject.FindWithTag` functions should be avoided when possible.
When they must be used, care should be taken to call them only during `Start()` or `Awake()`, to improve performance.
This also applies to `GetComponent()`, which should not be called every frame.

### Scenes

Temporary personal scenes are acceptable to work and test new features in.
An updated, functional common scene should be maintained to demonstrate the game in, or to copy to create new temporary scenes.

## Nomenclature

On the whole, naming should follow C# standards, unless otherwise stated here.

### Namespaces

Namespaces are all **PascalCase**, multiple words concatenated together, without hyphens ( - ) or underscores ( \_ ). The exception to this rule are acronyms like GUI or HUD, which can be uppercase:

**AVOID**:

```csharp
com.raywenderlich.fpsgame.hud.healthbar
```

**PREFER**:

```csharp
RayWenderlich.FPSGame.HUD.Healthbar
```

### Classes & Interfaces

Classes and interfaces are written in **PascalCase**. For example `RadialSlider`. 

### Methods

Methods are written in **PascalCase**. For example `DoSomething()`. 

### Fields

All private and protected fields are written **camelCase**, beginning with an underscore. Public fields  are written in **camelCase** as well, with no underscore.

For example:

```csharp
public class MyClass 
{
    public int publicField;
    int packagePrivate;
    private int _myPrivate;
    protected int _myProtected;
}
```

**AVOID:**

```csharp
private int myPrivateVariable
```

**PREFER:**

```csharp
private int _myPrivateVariable
```

Static fields should be written in **PascalCase**:

```csharp
public static int TheAnswer = 42;
```

Constant fields should be written in **ALL_UPPERCASE**
```csharp
public constant int MAX_SPEED = 20;
```

### Properties

All properties are written in **PascalCase**. For example:

```csharp
public int PageNumber 
{
    get { return pageNumber; }
    set { pageNumber = value; }
}
```

### Parameters

Parameters are written in **camelCase**.

**AVOID:**

```csharp
void DoSomething(Vector3 Location)
```

**PREFER:**

```csharp
void DoSomething(Vector3 location)
```

Single character values are to be avoided except for temporary looping variables.

### Actions

Actions are written in **PascalCase**. For example:

```csharp
public event Action<int> ValueChanged;
```

### Enums

Enum members are written in **PascalCase**.

### Misc

In code, acronyms should be treated as words. For example:

**AVOID:**

```csharp
XMLHTTPRequest
String URL
findPostByID
```  

**PREFER:**

```csharp
XmlHttpRequest
String url
findPostById
```

## Declarations

### Access Level Modifiers

Access level modifiers should be explicitly defined for classes, methods and member variables.

### Fields & Variables

Prefer single declaration per line.

**AVOID:**

```csharp
string username, twitterHandle;
```

**PREFER:**

```csharp
string username;
string twitterHandle;
```

### Classes

Exactly one class per source file, although inner classes are encouraged where scoping appropriate.

### Interfaces

All interfaces should be prefaced with the letter **I**. 

**AVOID:**

```csharp
RadialSlider
```

**PREFER:**

```csharp
IRadialSlider
```

### Type Parameters

Type parameters should be prefaced with the letter **T**.

## Spacing

### Indentation

Indentation should be done using **tabs** — never spaces.  

#### Blocks

Indentation for blocks uses **1 tab** for optimal readability:

**AVOID:**

```csharp
for (int i = 0; i < 10; i++) 
{
  Debug.Log("index=" + i);
}
```

**PREFER:**

```csharp
for (int i = 0; i < 10; i++) 
{
    Debug.Log("index=" + i);
}
```

#### Line Wraps

Indentation for line wraps should use **1 tab** (not the default 8 spaces):

**AVOID:**

```csharp
CoolUiWidget widget =
        someIncrediblyLongExpression(that, reallyWouldNotFit, on, aSingle, line);
```

**PREFER:**

```csharp
CoolUiWidget widget =
    someIncrediblyLongExpression(that, reallyWouldNotFit, on, aSingle, line);
```

### Line Length

Lines should be no longer than **100** characters long.

### Vertical Spacing

There should be exactly one blank line between methods to aid in visual clarity 
and organization. Whitespace within methods should separate functionality, but 
having too many sections in a method often means you should refactor into
several methods.


## Brace Style

All braces get their own line as it is a C# convention:

**AVOID:**

```csharp
class MyClass {
    void DoSomething() {
        if (someTest) {
          // ...
        } else {
          // ...
        }
    }
}
```

**PREFER:**

```csharp
class MyClass
{
    void DoSomething()
    {
        if (someTest)
        {
          // ...
        }
        else
        {
          // ...
        }
    }
}
```

Conditional statements are always required to be enclosed with braces,
irrespective of the number of lines required.

**AVOID:**

```csharp
if (someTest)
    doSomething();  

if (someTest) doSomethingElse();
```

**PREFER:**

```csharp
if (someTest) 
{
    DoSomething();
}  

if (someTest)
{
    DoSomethingElse();
}
```
## Switch Statements

Switch-statements come with `default` case by default (heh). If the `default` case is never reached, be sure to remove it.

**AVOID:**  
  
```csharp
switch (variable) 
{
    case 1:
        break;
    case 2:
        break;
    default:
        break;
}
```

**PREFER:**  
  
```csharp
switch (variable) 
{
    case 1:
        break;
    case 2:
        break;
}
```

## Language

Use US English spelling.

**AVOID:**

```csharp
string colour = "red";
```

**PREFER:**

```csharp
string color = "red";
```

The exception here is `MonoBehaviour` as that's what the class is actually called.

## Emoji 

The proper emoji for signifying approval in Discord is `:thumbsup:` (👍)

## Credits

This style guide was shamelessly stolen from raywenderlich.com, with changes by Jimmie Harkins.

raywenderlich.com credits are shown below:

This style guide is a collaborative effort from the most stylish
raywenderlich.com team members:

- [Darryl Bayliss](https://github.com/DarrylBayliss)
- [Sam Davies](https://github.com/sammyd)
- [Mic Pringle](https://github.com/micpringle)
- [Brian Moakley](https://github.com/VegetarianZombie)
- [Ray Wenderlich](https://github.com/rwenderlich)
- [Eric Van de Kerckhove](https://github.com/BlackDragonBE)

