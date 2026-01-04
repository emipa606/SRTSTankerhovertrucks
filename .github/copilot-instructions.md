# **GitHub Copilot Instructions for RimWorld Modding Project**

## **Mod Overview and Purpose**

This RimWorld mod provides an advanced tanker system that enhances resource management by allowing players to efficiently manage and utilize their resources. The mod integrates with other popular mods, such as Bad Hygiene and Rimefeller, providing overlap functionality that enriches the gameplay experience through new overlays and resource interactions.

## **Key Features and Systems**

- **Advanced Tanker System**: 
  - Offers functionalities for filling and draining resources in tanks with specific classes like `CompTanker` and `CompTankerMulti`.
  - Debugging tools for instant resource management (`DebugFill` and `DebugEmpty`).

- **Command Enhancements**:
  - Includes new right-click commands (`Command_ActionRightClick` and `Command_ToggleRightClick`) for quick access to tanker operations.

- **Compatibility Management**:
  - Compatible with other mods through compatibility classes (`BadHygieneCompat`, `RimefellerCompat`) and employs Harmony patches to ensure smooth integration and operations.

- **Dynamic Overlays**:
  - Integrates visual overlays to denote mod-specific functionalities and enhance user interaction (`Harmony_BadHygiene_DrawOverlay`, `Harmony_Rimefeller_DrawOverlay`).

## **Coding Patterns and Conventions**

- **Naming Conventions**: Class names use PascalCase for clarity and distinction of roles, such as `CompProperties_TankerBase`, `CompTanker`, and `Command_ActionRightClick`.

- **Inheritance Structure**: 
  - A clear inheritance pattern with abstract base classes (e.g., `CompTankerBase`, `CompProperties_TankerBase`) to define the foundational methods and attributes for derived classes.

- **Method Visibility**:
  - Most internal operations are encapsulated within methods marked `protected` or `internal`, supporting encapsulation and modular design.

## **XML Integration**

While the mod summary provided lacks specific XML structures, typical XML integration in RimWorld mods involves:

- **Defining Objects**: XML is often used to define in-game objects, items, and properties, such as tank capacities and functionalities.
  
- **XML Parsing**: This mod likely interacts with XML to determine configurations for its enhanced systems based on mod definitions loaded at the start of the game or through manifests.

## **Harmony Patching**

Harmony is utilized for patching the base game and other mods to introduce new functionalities and maintain compatibility:

- **Patch Classes Example**:
  - `Harmony_BadHygiene_DrawOverlay` and `Harmony_Rimefeller_DrawOverlay` are responsible for extending functionality and overlay features of base mods ensuring seamless integration.

- **Patch Methods**: 
  - Apply Prefix, Postfix, or Transpiler methods as needed to conditionally modify the behavior of original methods, ensuring no original functionalities are disrupted.

## **Suggestions for Copilot**

- **Contextual Code Suggestions**:
  - Providing in-context suggestions for new commands or overlays when editing similar classes and methods, using detected patterns.

- **Harmony Tools**:
  - Auto-suggest templates for common Harmony patching scenarios, streamlining the creation of Prefix and Postfix methods.

- **XML Dynamic Loading**:
  - Offer schemas for XML setup if applicable, assisting with quick object and property definitions.

- **Class and Method Definitions**:
  - Suggest commonly used RimWorld modding patterns for classes and when extending vanilla or modded game features.

By following these instructions and leveraging advanced tools like GitHub Copilot, developers can efficiently contribute towards enhancing this mod while maintaining seamless integration and compatibility with the RimWorld ecosystem and its myriad mod functionalities.
