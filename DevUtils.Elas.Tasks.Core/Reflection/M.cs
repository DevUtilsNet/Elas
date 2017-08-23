using System;
using System.Linq.Expressions;
using System.Reflection;

namespace DevUtils.Elas.Tasks.Core.Reflection
{
	sealed class M<T>
	{
		private static MethodInfo GetMethod(LambdaExpression expression)
		{
			var methodCallExp = expression.Body as MethodCallExpression;
			if (methodCallExp == null)
			{
				throw new ArgumentException("The expression's body must be a MethodCallExpression. The code block supplied should invoke a method.\nExample: x => x.Foo().", "expression");
			}

			var method = methodCallExp.Method;
			return method;
		}
		/// <summary>
		/// Selects the given method selector.
		/// </summary>
		/// <param name="methodSelector"> The method selector. </param>
		/// <returns>
		/// A MethodInfo.
		/// </returns>
		public static MethodInfo SelectMethod(Expression<Action<T>> methodSelector)
		{
			return GetMethod(methodSelector);
		}
		/// <summary>
		/// Selects the given method selector.
		/// </summary>
		/// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or
		/// 																		 illegal values. </exception>
		/// <typeparam name="TResult"> Type of the result. </typeparam>
		/// <param name="methodSelector"> The method selector. </param>
		/// <returns>
		/// A MethodInfo.
		/// </returns>
		public static MethodInfo SelectMethod<TResult>(Expression<Func<T, TResult>> methodSelector)
		{
			return GetMethod(methodSelector);
		}
		/// <summary>
		/// Select property.
		/// </summary>
		/// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or
		/// 																		 illegal values. </exception>
		/// <typeparam name="TResult"> Type of the result. </typeparam>
		/// <param name="propertySelector"> The property selector. </param>
		/// <returns>
		/// A PropertyInfo.
		/// </returns>
		public static PropertyInfo SelectProperty<TResult>(Expression<Func<T, TResult>> propertySelector)
		{
			var memberExp = propertySelector.Body as MemberExpression;
			if (memberExp == null)
			{
				// ReSharper disable LocalizableElement
				throw new ArgumentException("The expression's body must be a MemberExpression. The code block supplied should identify a property.\nExample: x => x.Bar.", "propertySelector");
				// ReSharper restore LocalizableElement
			}

			var pi = memberExp.Member as PropertyInfo;
			if (pi == null)
			{
				// ReSharper disable LocalizableElement
				throw new ArgumentException("The expression's body must identify a property, not a field or other member.", "propertySelector");
				// ReSharper restore LocalizableElement
			}

			return pi.ReflectedType == typeof(T) ? pi : typeof(T).GetProperty(pi.Name);
		}
		/// <summary>
		/// Select field.
		/// </summary>
		/// <exception cref="ArgumentException"> Thrown when one or more arguments have unsupported or
		/// 																		 illegal values. </exception>
		/// <typeparam name="TResult"> Type of the result. </typeparam>
		/// <param name="fieldSelector"> The field selector. </param>
		/// <returns>
		/// A FieldInfo.
		/// </returns>
		public static FieldInfo SelectField<TResult>(Expression<Func<T, TResult>> fieldSelector)
		{
			var memberExp = fieldSelector.Body as MemberExpression;
			if (memberExp == null)
			{
				// ReSharper disable LocalizableElement
				throw new ArgumentException("The expression's body must be a MemberExpression. The code block supplied should identify a field.\nExample: x => x.Bar.", "fieldSelector");
				// ReSharper restore LocalizableElement
			}

			var fi = memberExp.Member as FieldInfo;
			if (fi == null)
			{
				// ReSharper disable LocalizableElement
				throw new ArgumentException("The expression's body must identify a field, not a property or other member.", "fieldSelector");
				// ReSharper restore LocalizableElement
			}

			return fi.ReflectedType == typeof(T) ? fi : typeof(T).GetField(fi.Name);
		}
	}
}
