# Change Log
All notable changes will be listed in this file

## [1.3.3] - 2022-08-18
### Added
* Added custom asset icons
* Behaviour Tree Editor now displays an asterisk(*) if the tree is dirty

### Changed
* `Decorator` nodes now log a warning instead of throwing if they don't have a child
* Removed `.Runtime` from the namespace
* Renamed the Zenject define symbol to `BTREE_ADD_ZENJECT`

## [1.3.2] - 2022-05-05
### Added
* Added Zenject Support (Add `BTREE_INTEGRATE_ZENJECT` to define symbols)
* Blackboard Editor now displays inherited properties
* Added attribute for custom node creation paths

### Changed
* Search window is now like ShaderGraph's

## [1.3.1] - 2022-04-05
### Fixed
* Fixed property reference list filtering

## [1.3.0] - 2022-04-05
### Changed
* Reworked the Blackboard Editor. Now it shows more useful info.
* Blackboard Properties now have to be exposed in the Editor if you need to override them

### Fixed
* Fixed `OnCreate()` call execution order

## [1.2.0] - 2022-02-27
### Changed
* Changed the minimum required Unity version to 2021.*+

## [1.1.2] - 2022-02-27
### Added
* You can now override default values of BehaviourTreeRunners for every Runner instance
* Blackboard Editor now displays a message if a key is defined multiple times

## [1.1.1] - 2022-02-22
### Added
* Added `Wait` and `WaitBlackboardTime` nodes to the standard library

## [1.1.0] - 2022-02-22
### Added
* `BehaviourTreeRunner` can now be inherited from

### Changed
* Nodes now take `BehaviourTreeRunner` as a parameter

## [1.0.2] - 2022-02-21
### Added
* (Finally) added scroll view to Blackboard editor

## [1.0.1] - 2022-02-21
### Added
* Custom node names
* Code comments

## [1.0.0] - 2022-02-20
### Fixed
* Fixed all compilation errors
* Fixed UI Toolkit errors on package import

### Changed
* Removed UI Builder from dependencies

## [0.4.0] - 2022-02-19
### Added
* Visual Editor
* Conditional Nodes
* Blackboard System
* Blackboard Parenting System
* Blood, sweat and tears
### Changed
* **_Everything_**

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