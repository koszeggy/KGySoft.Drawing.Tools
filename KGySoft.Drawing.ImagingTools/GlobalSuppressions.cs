// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage("Style", "IDE0028:Use collection initializers", Justification = "Decided individually")]
[assembly: SuppressMessage("Style", "IDE0063:Use simple 'using' statement", Justification = "Decided individually")]
[assembly: SuppressMessage("Style", "IDE0066:Convert switch statement to expression", Justification = "Decided individually")]
[assembly: SuppressMessage("Style", "IDE0057:Use range operator", Justification = "Cannot be used because it is not supported in every targeted platform")]
[assembly: SuppressMessage("Style", "IDE0090:Use 'new(...)'", Justification = "Decided individually")]
[assembly: SuppressMessage("Style", "IDE0042:Deconstruct variable declaration", Justification = "Decided individually")]
[assembly: SuppressMessage("Style", "IDE0130:Namespace does not match folder structure", Justification = "False alarm, Namespace Provider property is set to false to for folders that are not namespace providers")]
[assembly: SuppressMessage("Style", "IDE0290:Use primary constructor", Justification = "Decided individually")]
[assembly: SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "False alarm, in this project System.Drawing types CAN be used on non-Windows platforms")]
