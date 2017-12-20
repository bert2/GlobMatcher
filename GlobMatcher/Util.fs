﻿namespace GlobMatcher

module List =

    let inline foldBack' folder state list = List.foldBack folder list state

    let inline front list = list |> List.truncate (list.Length - 1)

