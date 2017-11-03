﻿module BugFixedPatterns

open Xunit
open GlobMatcher

[<Theory>]
[<InlineData("abb")>]
[<InlineData("abbb")>]
[<InlineData("ababb")>]
[<InlineData("abbabb")>]
[<InlineData("abbbabb")>]
let ``two equal characters after a "*" should not break backtracking`` text =
    let start::_,transitions = Parser.toAcceptor "*bb"
    let result = Acceptor.run start transitions text
    Assert.True(result)