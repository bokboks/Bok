/*
   Copyright 2023 bokboks

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

       http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
*/
using System;

namespace Bok.Tests;

public class MatcherMatches
{
    Matcher matcher = new Matcher();

    [Fact]
    public void MatchEmpty()
    {
        Assert.True(matcher.IsMatch(new object[] { }, new object[] { }));
    }

    [Fact]
    public void MatchLengthMismatch()
    {
        Assert.False(matcher.IsMatch(new object[] { }, new object[] { true }));
        Assert.False(matcher.IsMatch(new object[] { "", "d" }, new object[] { }));
    }

    [Fact]
    public void MatchBool()
    {
        Assert.True(matcher.IsMatch(new object[] { true }, new object[] { true }));
        Assert.False(matcher.IsMatch(new object[] { false }, new object[] { true }));
        Assert.False(matcher.IsMatch(new object[] { true }, new object[] { false }));
        Assert.True(matcher.IsMatch(new object[] { true }, new object[] { Any.Bool }));
        Assert.True(matcher.IsMatch(new object[] { true }, new object[] { Any.Thing }));
        Assert.True(matcher.IsMatch(new object[] { null }, new object[] { Any.Thing }));
    }

    [Fact]
    public void MatchString()
    {
        Assert.True(matcher.IsMatch(new object[] { null }, new object[] { null }));
        Assert.False(matcher.IsMatch(new object[] { "d" }, new object[] { null }));
        Assert.False(matcher.IsMatch(new object[] { null }, new object[] { "d" }));
        Assert.False(matcher.IsMatch(new object[] { "d" }, new object[] { "e" }));
        Assert.False(matcher.IsMatch(new object[] { "d" }, new object[] { "D" }));
        Assert.False(matcher.IsMatch(new object[] { "d" }, new object[] { " d " }));
        Assert.True(matcher.IsMatch(new object[] { "d" }, new object[] { Any.String }));
        Assert.True(matcher.IsMatch(new object[] { null }, new object[] { Any.String }));
        Assert.True(matcher.IsMatch(new object[] { "d" }, new object[] { Any.Thing }));
        Assert.True(matcher.IsMatch(new object[] { null }, new object[] { Any.Thing }));
    }
    [Fact]
    public void MatchInt32()
    {
        Assert.True(matcher.IsMatch(new object[] { 4 }, new object[] { 4 }));
        Assert.False(matcher.IsMatch(new object[] { 7 }, new object[] { 4 }));
        Assert.False(matcher.IsMatch(new object[] { 4 }, new object[] { 7 }));
        Assert.True(matcher.IsMatch(new object[] { 4 }, new object[] { Any.Int }));
        Assert.True(matcher.IsMatch(new object[] { 4 }, new object[] { Any.Thing }));
        Assert.True(matcher.IsMatch(new object[] { null }, new object[] { Any.Thing }));
    }
    [Fact]
    public void MatchInt64()
    {
        Assert.True(matcher.IsMatch(new object[] { (long)4 }, new object[] { (long)4 }));
        Assert.False(matcher.IsMatch(new object[] { (long)7 }, new object[] { (long)4 }));
        Assert.False(matcher.IsMatch(new object[] { (long)4 }, new object[] { (long)7 }));
        Assert.True(matcher.IsMatch(new object[] { (long)4 }, new object[] { Any.Long }));
        Assert.True(matcher.IsMatch(new object[] { (long)4 }, new object[] { Any.Thing }));
        Assert.True(matcher.IsMatch(new object[] { null }, new object[] { Any.Thing }));
    }
    [Fact]
    public void MatchDouble()
    {
        Assert.True(matcher.IsMatch(new object[] { 44.00 }, new object[] { 44.00 }));
        Assert.False(matcher.IsMatch(new object[] { 77.00 }, new object[] { 44.00 }));
        Assert.False(matcher.IsMatch(new object[] { 44.00 }, new object[] { 77.00 }));
        Assert.True(matcher.IsMatch(new object[] { 44.00 }, new object[] { Any.Double }));
        Assert.True(matcher.IsMatch(new object[] { 44.00 }, new object[] { Any.Thing }));
        Assert.True(matcher.IsMatch(new object[] { null }, new object[] { Any.Thing }));
    }
}


public class MatchMatches
{


    [Fact]
    public void MatchBool()
    {
        Assert.True(new Match<bool>(true).IsMatch(true));
        Assert.False(new Match<bool>(false).IsMatch(true));
        Assert.False(new Match<bool>(true).IsMatch(false));
    }

    [Fact]
    public void MatchString()
    {
        Assert.True(new Match<string>(null).IsMatch(null));
        Assert.False(new Match<string>("d").IsMatch(null));
        Assert.False(new Match<string>(null).IsMatch("d"));
        Assert.False(new Match<string>("d").IsMatch("e"));
        Assert.False(new Match<string>("d").IsMatch("D"));
        Assert.False(new Match<string>("d").IsMatch(" d "));
    }

    [Fact]
    public void MatchInt32()
    {
        Assert.True(new Match<int>(4).IsMatch(4));
        Assert.False(new Match<int>(7).IsMatch(4));
        Assert.False(new Match<int>(4).IsMatch(7));
    }
}

