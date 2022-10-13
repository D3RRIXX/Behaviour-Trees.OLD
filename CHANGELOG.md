# Change Log
All notable changes will be listed in this file

## [0.3.0] - 2022-10-13
### Added
* Nodes now have `INode.OnNodeEnter()` and `INode.OnNodeExit()` methods, which get called when node a composite starts and stops executing this node.
### Changed
* Composites now have `dynamic` field. It defines whether the composite will run `INode.Execute()` on all children that come before the child that broke the execution loop.

## [0.2.0] - 2022-10-08
### Added
* The new base type for nodes is `INode`
* Added `NodeBehaviour` abstract type which allows to create custom nodes that derive from `MonoBehaviour`.
### Changed
* `INode.Execute()` no longer takes parametres, as now you should only call `INode.InjectBlackboard(IBlackboard blackboard)` once during initialization.
* Changed folder hierarchy and updated namespaces.

## [0.1.2] - 2022-09-01
### Fixed
* Restructured folder hierarchy so that git files aren't included in the package.

## [0.1.1] - 2022-08-15
### Added
* Minimal Unity version is 2020.
### Fixed
* `Conditional` now works in Unity 2020.

## [0.1.0] - 2022-08-13
### Added
* Core features of a Behaviour Tree.
* First premade node - `GoToPosition`.