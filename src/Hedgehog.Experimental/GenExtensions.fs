namespace Hedgehog.Linq

open System
open System.Runtime.CompilerServices
open Hedgehog

[<AbstractClass; Sealed; Extension>]
type GenExtraExtensions() =

  [<Extension>]
  static member WithGenerator(self : AutoGenConfig, generator : Gen<'T>) =
      self |> AutoGenConfig.addGenerator generator

  /// Generates null part of the time.
  [<Extension>]
  static member WithNull(self : Gen<'T>) =
    GenX.withNull self

  /// Generates a value that is not null.
  [<Extension>]
  static member WithoutNull(self : Gen<'T>) =
    GenX.noNull self

  /// Generates a value that is not contained in the specified list.
  [<Extension>]
  static member NotIn(self : Gen<'T>, values : ResizeArray<'T>) =
    GenX.notIn (List.ofSeq values) self

  /// Generates a list that does not contain the specified element.
  /// Shortcut for Gen.filter (not << List.contains x)
  [<Extension>]
  static member NotContains(self : Gen<ResizeArray<'T>>, value: 'T) =
    GenX.notContains value (self |> Gen.map(List.ofSeq)) |> Gen.map ResizeArray

  // Inserts the given element at a random place in the list
  // Does not guarantee that the element is unique in the list
  [<Extension>]
  static member WithElement(self : Gen<ResizeArray<'T>>, x : 'T) =
    self |> Gen.map List.ofSeq |> GenX.addElement x |> Gen.map ResizeArray

  /// Sorts elements in a tuple.
  [<Extension>]
  static member Sorted(self : Gen<'T * 'T>) =
    GenX.sorted2 self

  /// Sorts elements in a tuple.
  [<Extension>]
  static member Sorted(self : Gen<'T * 'T * 'T>) =
    GenX.sorted3 self

  /// Sorts elements in a tuple.
  [<Extension>]
  static member Sorted(self : Gen<'T * 'T * 'T * 'T>) =
    GenX.sorted4 self

  // Generates a tuple with distinct elements.
  [<Extension>]
  static member Distinct(self : Gen<struct ('T * 'T)>) =
    self |> Gen.map (fun x -> x.ToTuple()) |> GenX.distinct2

  // Generates a tuple with distinct elements.
  [<Extension>]
  static member Distinct(self : Gen<struct ('T * 'T * 'T)>) =
    self |> Gen.map (fun x -> x.ToTuple()) |> GenX.distinct3

  // Generates a tuple with distinct elements.
  [<Extension>]
  static member Distinct(self : Gen<struct ('T * 'T * 'T * 'T)>) =
    self |> Gen.map (fun x -> x.ToTuple()) |> GenX.distinct4

  // Generates a tuple with strictly increasing elements.
  [<Extension>]
  static member Increasing(self : Gen<struct ('T * 'T)>) =
    self |> Gen.map (fun x -> x.ToTuple()) |> GenX.increasing2

  // Generates a tuple with strictly increasing elements.
  [<Extension>]
  static member Increasing(self : Gen<struct ('T * 'T * 'T)>) =
    self |> Gen.map (fun x -> x.ToTuple()) |> GenX.increasing3

  // Generates a tuple with strictly increasing elements.
  [<Extension>]
  static member Increasing(self : Gen<struct ('T * 'T * 'T * 'T)>) =
    self |> Gen.map (fun x -> x.ToTuple()) |> GenX.increasing4

  /// Generates a tuple of datetimes where dayRange determines the minimum
  /// and maximum number of days apart. Positive numbers means the datetimes
  /// will be in increasing order, and vice versa.
  [<Extension>]
  static member DateInterval(dayRange : Range<int>) =
    GenX.dateInterval dayRange

  /// Generates a list using inpGen together with a function that maps each
  /// of the distinct elements in the list to values generated by outGen.
  /// Distinct elements in the input list may map to the same output values.
  /// For example, [2; 3; 2] may map to ['A'; 'B'; 'A'] or ['A'; 'A'; 'A'],
  /// but never ['A'; 'B'; 'C']. The generated function throws if called with
  /// values not present in the input list.
  [<Extension>]
  static member WithMapTo(self : Gen<ResizeArray<'T>>, outGen : Gen<'R>) =
    self
    |> Gen.map List.ofSeq
    |> GenX.withMapTo outGen
    |> Gen.map (fun (values, f) -> (values, Func<'T, 'R>(f)))

  /// Generates a list using inpGen together with a function that maps each
  /// of the distinct elements in the list to values generated by outGen.
  /// Distinct elements in the input list are guaranteed to map to distinct
  /// output values. For example, [2; 3; 2] may map to ['A'; 'B'; 'A'], but
  /// never ['A'; 'A'; 'A'] or ['A'; 'B'; 'C']. Only use this if the output
  /// space is large enough that the required number of distinct output values
  /// are likely to be generated. The generated function throws if called with
  /// values not present in the input list.
  [<Extension>]
  static member WithDistinctMapTo(self : Gen<ResizeArray<'T>>, outGen : Gen<'R>) =
    self
    |> Gen.map List.ofSeq
    |> GenX.withDistinctMapTo outGen
    |> Gen.map (fun (values, f) -> (values, Func<'T, 'R>(f)))