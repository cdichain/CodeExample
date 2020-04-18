# 实用代码集合

一部分是我自己写的, 一部分是从网上收集的, 凡是Author: CDiChain的都是自己写的

## ChoiceExtension.cs
方便的在一句代码中实现switch...case,并且更加简洁, 以下是用法.

```C#
Assert.AreEqual("奇数", "3".AsChoice().When("1", "3", "5").Then("奇数").When("2", "4", "6").Then("偶数").Else(null));
Assert.AreEqual("偶数", "6".AsChoice().When("1", "3", "5").Then("奇数").When("2", "4", "6").Then("偶数").Else(null));
Assert.AreEqual("不知道", "xx".AsChoice().When("1", "3", "5").Then("奇数").When("2", "4", "6").Then("偶数").Else("不知道"));
Assert.AreEqual("奇数", 1.AsChoice().When(a => a % 2 == 0).Then("偶数").Else("奇数"));
Assert.AreEqual("偶数", 2.AsChoice().When(a => a % 2 == 0).Then("偶数").Else("奇数"));
```
