/******************************************
 * Author: CDiChain
 * Usage:
 *   Assert.AreEqual("奇数", "3".AsChoice()
 *      .When("1", "3", "5").Then("奇数")
 *      .When("2", "4", "6").Then("偶数")
 *      .Else(null));
 *   Assert.AreEqual("偶数", "6".AsChoice()
 *      .When("1", "3", "5").Then("奇数")
 *      .When("2", "4", "6").Then("偶数")
 *      .Else(null));
 *   Assert.AreEqual("不知道", "xx".AsChoice()
 *      .When("1", "3", "5").Then("奇数")
 *      .When("2", "4", "6").Then("偶数")
 *      .Else("不知道"));
 *   Assert.AreEqual("奇数", 1.AsChoice()
 *      .When(a => a % 2 == 0).Then("偶数")
 *      .Else("奇数"));
 *   Assert.AreEqual("偶数", 2.AsChoice()
 *      .When(a => a % 2 == 0).Then("偶数")
 *      .Else("奇数"));
 */
using System;
using System.Collections.Generic;
using System.Text;

namespace YourNamespace
{
    public static class ChoiceExtension
    {
        /// <summary>
        /// 转换为可选择的条件对象
        /// !!!切记, 前面的对象必须不能为null哦, 如果可能为null建议使用?.语法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target"></param>
        /// <returns></returns>
        public static Choice<T> AsChoice<T>(this T target)
        {
            return new Choice<T>(target);
        }

        public class Choice<T>
        {
            private T _target;

            internal Choice(T target)
            {
                _target = target;
            }

            public WhenCondition<T> When(params T[] expect)
            {
                return new WhenCondition<T>(_target, expect);
            }

            public WhenCondition<T> When(params Func<T, bool>[] conditions)
            {
                if (conditions == null || conditions.Length == 0)
                {
                    return new WhenCondition<T>(_target, null);
                }

                foreach (var c in conditions)
                {
                    if (c(_target))
                    {
                        return new WhenCondition<T>(_target, new[] { _target });
                    }
                }

                return new WhenCondition<T>(_target);
            }
        }

        public class WhenCondition<T>
        {
            private T _target { get; set; }
            private T[] _expect { get; set; }

            /// <summary>
            /// 强制跳过本次判端
            /// </summary>
            private bool _skip = false;

            internal WhenCondition(T target)
            {
                _target = target;
                _skip = true;
            }

            internal WhenCondition(T target, T[] expect)
            {
                _target = target;
                _expect = expect;
            }

            public ThenCondition<T, U> Then<U>(U returnValue)
            {
                if (_skip)
                {
                    return new ThenCondition<T, U>(_target, false, returnValue);
                }

                bool equals = false;
                if (_expect == null)
                {
                    equals = _target == null;
                }
                else
                {
                    foreach (var item in _expect)
                    {
                        if (item == null)
                        {
                            equals = _target == null;
                            if (equals)
                            {
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (item.Equals(_target))
                        {
                            equals = true;
                            break;
                        }
                    }
                }

                return new ThenCondition<T, U>(_target, equals, returnValue);
            }
        }

        public class WhenCondition<T, U>
        {
            private T _target;
            private T[] _expect;
            private bool _isMatch;
            private U _selectedValue;
            private bool skip = false;

            internal WhenCondition(T target, bool isMatch, U selectValue)
            {
                _target = target;
                _isMatch = isMatch;
                _selectedValue = selectValue;
            }

            internal WhenCondition(T target, T[] expect, bool isMatch, U selectValue)
            {
                _target = target;
                _expect = expect;
                _isMatch = isMatch;
                _selectedValue = selectValue;
            }

            public ThenCondition<T, U> Then(U returnValue)
            {

                if (skip || _isMatch)
                {
                    return new ThenCondition<T, U>(_target, _isMatch, _selectedValue);
                }

                bool equals = false;
                if (_expect == null)
                {
                    equals = _target == null;
                }
                else
                {
                    foreach (var item in _expect)
                    {
                        if (item == null)
                        {
                            equals = _target == null;
                            if (equals)
                            {
                                break;
                            }
                            else
                            {
                                continue;
                            }
                        }

                        if (item.Equals(_target))
                        {
                            equals = true;
                            break;
                        }
                    }
                }


                return new ThenCondition<T, U>(_target, equals, returnValue);
            }
        }

        public class ThenCondition<T, U>
        {
            private T _target;
            private bool _isMatched;
            private U _returnValue;

            public ThenCondition(T target, bool isMatched, U returnValue)
            {
                _target = target;
                _isMatched = isMatched;
                _returnValue = returnValue;
            }

            public WhenCondition<T, U> When(params T[] expect)
            {
                if (_isMatched)
                {
                    return new WhenCondition<T, U>(_target, expect, true, _returnValue);
                }

                return new WhenCondition<T, U>(_target, expect, false, default);
            }

            public WhenCondition<T, U> When(params Func<T, bool>[] conditions)
            {
                if (conditions == null || conditions.Length == 0)
                {
                    return new WhenCondition<T, U>(_target, null, _isMatched, _returnValue);
                }

                foreach (var c in conditions)
                {
                    if (c(_target))
                    {
                        return new WhenCondition<T, U>(_target, new[] { _target }, _isMatched, _returnValue);
                    }
                }

                return new WhenCondition<T, U>(_target, _isMatched, _returnValue);
            }


            public U Else(U defaultValue)
            {
                return _isMatched ? _returnValue : defaultValue;
            }

        }
    }
}
