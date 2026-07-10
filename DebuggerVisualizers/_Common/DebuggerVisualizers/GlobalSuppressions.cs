// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "False alarm, netxxx-windows is specified in TargetFrameworks")]
[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure", Justification = "False alarm, NamespaceProvider is set to false for all non-namespace folders.")]
[assembly: SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Decided individually")]
