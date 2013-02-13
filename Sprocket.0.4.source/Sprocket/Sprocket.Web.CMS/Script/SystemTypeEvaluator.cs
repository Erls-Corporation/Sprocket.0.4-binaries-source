using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sprocket.Web.CMS.Script
{
	internal static class SystemTypeEvaluator
	{
		public static object EvaluateProperty(object o, string propertyName, Token propertyNameToken)
		{
			if (o == null)
				throw new InstructionExecutionException("The value here is null, which can't have a property.", propertyNameToken);
			if (o is string)
			{
				switch (propertyName)
				{
					case "length": return ((string)o).Length;
				}
			}
			else if(o is IList)
			{
				switch (propertyName)
				{
					case "count": return ((IList)o).Count;
				}
			}
			throw new InstructionExecutionException("I can't evaluate this property because the value that it pertains to does not have script support for this property name. (Underlying type: " + o.GetType().Name + ")", propertyNameToken);
		}

		public static object EvaluateArguments(ExecutionState state, object o, List<ExpressionArgument> args, Token contextToken)
		{
			if(o == null)
				throw new InstructionExecutionException("The value here is null, which isn't able to process an argument list.", contextToken);
			if (o is IList)
			{
				if (args.Count > 1)
					throw new InstructionExecutionException("I can't evaluate the arguments for this list because you've specified more than one argument. The only argument you can specify for a list is a numeric expression indicating which list item you're referring to.", contextToken);
				object n = TokenParser.VerifyUnderlyingType(args[0].Expression.Evaluate(state, args[0].Token));
				if (!(n is decimal))
					throw new InstructionExecutionException("The argument you specified doesn't equate to a number, which is required to specify which list item you're referring to.", args[0].Token);
				int index = Convert.ToInt32(n);
				if(index >= ((IList)o).Count)
					throw new InstructionExecutionException("The index specified here is higher than the highest index in the list. Remember, the lowest index is 0 and the highest is one less than the total number of items in the list.", args[0].Token);
				return ((IList)o)[index];
			}
			throw new InstructionExecutionException("This type of object isn't able to process an argument list. (Underlying type: " + o.GetType().Name, contextToken);
		}
	}
}
