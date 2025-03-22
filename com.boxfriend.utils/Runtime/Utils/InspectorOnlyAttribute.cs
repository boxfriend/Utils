using System;

///<summary>
/// Indicates a variable should only be assigned to in the inspector. Also allowed are field initializers and assignment in unity's Reset method. 
/// Requires Boxfriend.Analyzers to function.
///</summary>
public class InspectorOnlyAttribute : Attribute { }
