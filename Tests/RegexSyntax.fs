﻿module RegexSyntax

open Xunit
open GlobMatcher

let private assertMatch pattern text isMatch =
    let a = RegexParser.toAutomaton' pattern
    let result = Automaton.run a text
    Assert.Equal (isMatch, result)

[<Theory>]
[<InlineData("b", "b", true)>]
[<InlineData("b", "a", false)>]
[<InlineData("b", "", false)>]
let ``matches literal characters`` p t r = assertMatch p t r

[<Theory>]
[<InlineData("(a)", "a", true)>]
[<InlineData("(a)", "b", false)>]
[<InlineData("(ab)", "ab", true)>]
[<InlineData("a(bc)", "abc", true)>]
[<InlineData("a(bc)d", "abcd", true)>]
[<InlineData("a(bc)d", "ad", false)>]
let ``parantheses form subexpressions`` p t r = assertMatch p t r

[<Theory>]
[<InlineData("a*", "", true)>]
[<InlineData("a*", "a", true)>]
[<InlineData("a*", "aa", true)>]
let ``Kleene star matches zero or more characters`` p t r = assertMatch p t r

[<Theory>]
[<InlineData("(ab)*", "", true)>]
[<InlineData("(ab)*", "ab", true)>]
[<InlineData("(ab)*", "abab", true)>]
[<InlineData("(ab)*", "abcabc", false)>]
let ``Kleene star matches zero or more subexpressions`` p t r = assertMatch p t r

[<Theory>]
[<InlineData("a+", "", false)>]
[<InlineData("a+", "a", true)>]
[<InlineData("a+", "aa", true)>]
let ``Kleene plus matches one or more characters`` p t r = assertMatch p t r

[<Theory>]
[<InlineData("(ab)+", "", false)>]
[<InlineData("(ab)+", "ab", true)>]
[<InlineData("(ab)+", "abab", true)>]
[<InlineData("(ab)+", "abcabc", false)>]
let ``Kleene plus matches one or more subexpressions`` p t r = assertMatch p t r

[<Theory>]
[<InlineData("a?", "", true)>]
[<InlineData("a?", "a", true)>]
[<InlineData("a?", "aa", false)>]
let ``option matches zero or one character`` p t r = assertMatch p t r

[<Theory>]
[<InlineData("(ab)?", "", true)>]
[<InlineData("(ab)?", "ab", true)>]
[<InlineData("(ab)?", "abab", false)>]
let ``option matches zero or one submatch expression`` p t r = assertMatch p t r

[<Theory>]
[<InlineData("(a(b)(c(d)))(e)", "abcde", true)>]
[<InlineData("a(b(c(d)+)*e)f", "abcddcdddef", true)>]
let ``submatch expressions can be nested`` p t r = assertMatch p t r

[<Theory>]
[<InlineData("a*b", "b", true)>]
[<InlineData("a*b", "ab", true)>]
[<InlineData("a*b", "aab", true)>]
[<InlineData("ab*c+d?e", "abbbcccde", true)>]
[<InlineData("ab*c+d?e", "acccde", true)>]
[<InlineData("ab*c+d?e", "abbbccce", true)>]
[<InlineData("ab*c+d?e", "accce", true)>]
let ``can combine automatons`` p t r = assertMatch p t r

[<Theory>]
[<InlineData("*")>]
[<InlineData("+")>]
[<InlineData("?")>]
[<InlineData("**")>]
[<InlineData("(")>]
[<InlineData(")")>]
[<InlineData("(*)")>]
let ``invalid pattern gives parser error`` pattern =
    match RegexParser.toAutomaton pattern with
    | Result.Failure _ -> ()
    | Result.Success _ -> failwith "Expected parser error."