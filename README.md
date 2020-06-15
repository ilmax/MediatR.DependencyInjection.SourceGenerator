# Experimental build time generation of MediatR registrations
This is an experimental project to remove the runtime reflection used by *MediatR.Extensions.Microsoft.DependencyInjection* taking advantage of the new Roslyn source generator.

> This is just an experiment

## Features
- Build time discovery and generation of MediatR registrations
- Allow usage of a partial class with a well known name of **MediatRServiceExtension** (method name stillyet not decided)
- Allow easy debugging of generated source code via an environment variable named **DEBUG_SOURCE_GENERATOR** with value **1**

## Limitations
- Only takes into account the project where the analyzer has been added
- Configuration of generated code is very limited yet (class name and method name are hardcoded)

## TODOs
- Overcome limitations
- Ensure discovery logic is in sync with MediatR.Extensions.Microsoft.DependencyInjection
- Investigate how to unit test the source generator
- Investigate performance hit in building large projects
- Investigate performance win at statup in large projects

## Poke around with the source generator
In order to poke around with this source generator just clone the repo and werite code in the *ConsoleApp1* project

## Debug
In order to debug the source generator uncomment the ```Debugger.Launch()``` in the ```MediatRSourceGenerator```