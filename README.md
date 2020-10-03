# Experimental build time generation of MediatR registrations
This is an experimental project to remove the runtime reflection used by *MediatR.Extensions.Microsoft.DependencyInjection* taking advantage of the new Roslyn source generator.

> This is in alpha stage at the moment.

## Features
- Build time discovery and generation of MediatR registrations for the Microsoft built in DI container
- Allow usage of a partial class with a configurable name that defaults to `MediatRServiceExtension`, in this case it infers the method name if it's there by searching an extension method on `IServiceCollection`

## Limitations
- Only registers services in the current compilation because currently the analisys is run only on the source files part of the current compilation (i.e. no reflection inspection is done on 3rd party assemblies)
- Configuration of generated code is a bit limited yet, the only supported configurations are namespace, class name and method name for the generated extension method class
- Partial class support matches only extension methods without any additional parameter, this is an unnecessary restriction and can be removed

## TODOs
- Overcome limitations
- Ensure discovery logic is in sync with MediatR.Extensions.Microsoft.DependencyInjection
- Investigate how to unit test the source generator
- Investigate performance in building large projects
- Investigate performance win at statup in large projects

## Poke around with the source generator
In order to poke around with this source generator just clone the repo and werite code in the *ConsoleApp1* project

## Debug
In order to debug the source generator uncomment the `Debugger.Launch()` in the `MediatRSourceGenerator`

## Getting started
1. Install the package T.B.D.
2. Replace the call to `services.AddMediatR()` with `services.AddMediatRRegistrations()`

> In order to use a partial class, you can define something that looks like the following:
```csharp
static partial class MediatRServiceExtension
{
    public static partial void AddMediatRRegistrations(this IServiceCollection services);
}
```
The name of the method can be changed as well as the name of the parameter, no additional parameters are supported at the moment and the extension method can be just `public` or `internal`, no others modifiers are supported as of now.
