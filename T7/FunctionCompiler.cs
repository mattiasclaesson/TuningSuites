using System;
using System.CodeDom.Compiler;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using Microsoft.CSharp;

namespace Plot3D
{
    /// <summary>Compiles functions as runtime. This class cannot be inherited.</summary>
    public static class FunctionCompiler
    {
        /// <summary>Compiles function at runtime.</summary>
        /// <param name="parametersCount">Number of input parameters.</param>
        /// <param name="functionBody">Function body.</param>
        /// <returns>Compiled function.</returns>
        /// <exception cref="LukeSw.Runtime.FunctionNotCompiledException">Thrown when function was not compiled because of some errors in functions body.</exception>
        /// <example>This sample shows how to call the Compile method.
        /// <code>
        /// CompiledFunction cf = FunctionCompiler.Compile(3, "y = x1 + x2 + x3;");
        /// double y = cf(2, 4, 7);
        /// </code>
        /// </example>
        public static CompiledFunction Compile(int parametersCount, string functionBody)
        {
            functionBody = functionBody.Trim().ToLower();
            if (functionBody.Contains(";"))
            {
                //throw new FunctionNotCompiledException(T7.Properties.Resources.FunctionCompiler_SemicolonException);
            }
            if (!functionBody.StartsWith("y"))
            {
                functionBody = string.Format("y = {0};", functionBody);
            }
            functionBody = functionBody.Replace("asin", "Math.Asin");
            functionBody = functionBody.Replace("acos", "Math.Acos");
            functionBody = functionBody.Replace("atan", "Math.Atan");
            functionBody = functionBody.Replace("sin", "Math.Sin");

            functionBody = functionBody.Replace("Math.AMath.Sin", "Math.Asin");
            functionBody = functionBody.Replace("cos", "Math.Cos"); functionBody = functionBody.Replace("Math.AMath.Cos", "Math.Acos");
            functionBody = functionBody.Replace("tan", "Math.Tan"); functionBody = functionBody.Replace("Math.AMath.Tan", "Math.Atan");
            functionBody = functionBody.Replace("cot", "1/Math.Tan"); functionBody = functionBody.Replace("A1/Math.Tan", "Acot");
            functionBody = functionBody.Replace("sqrt", "Math.Sqrt");
            functionBody = functionBody.Replace("exp", "Math.Exp");
            functionBody = functionBody.Replace("log10", "Math.Log10");
            functionBody = functionBody.Replace("log2", "(1/Log(2))*Math.Log");
            functionBody = functionBody.Replace("log", "Math.Log");
            functionBody = functionBody.Replace("ceil", "Math.Ceiling");
            functionBody = functionBody.Replace("floor", "Math.Floor");
            functionBody = functionBody.Replace("round", "Math.Round");
            functionBody = functionBody.Replace("pi", "Math.PI");
            functionBody = functionBody.Replace("pow", "Math.Pow");

            StringBuilder strb = new StringBuilder();
            for (int i = 0; i < parametersCount; i++)
            {
                strb.Append("double x").Append(i + 1).Append(" = __X[").Append(i).Append("];\n");
            }
            if (parametersCount == 1)
            {
                strb = strb.Append("double x = x1;\n");
            }
            strb.Append("double ").Append(functionBody).Append(";");

            //string str = string.Format(T7.Properties.Resources.FunctionCompiler_EvalClass, strb.ToString(), typeof(CompiledFunction).Namespace);
            string str = string.Empty;

            CSharpCodeProvider cscp = new CSharpCodeProvider();
            CompilerParameters cp = new CompilerParameters();
            cp.CompilerOptions = "/t:library";
            cp.GenerateInMemory = true;
            cp.ReferencedAssemblies.Add("mscorlib.dll");
            cp.ReferencedAssemblies.Add("System.dll");
            cp.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
            CompilerResults cr = cscp.CompileAssemblyFromSource(cp, str);
            if (cr.Errors.HasErrors)
            {
                StringBuilder err = new StringBuilder();
                err.Append("Compilation error (").Append(cr.Errors.Count).Append(" errors):\n");
                foreach (CompilerError ce in cr.Errors)
                {
                    err.Append("\n").Append(ce);
                }
                err.Append("\n\nCode:\n\n").Append(str);
                throw new FunctionNotCompiledException(err.ToString());
            }
            return cr.CompiledAssembly.GetType("Eval").GetMethod("__get").Invoke(null, null) as CompiledFunction;
        }
    }

    /// <summary>The exception is thrown when function was not compiled.</summary>
    [Serializable]
    public class FunctionNotCompiledException : ApplicationException
    {
        /// <summary>Initializes a new instance of the <see cref="T:LukeSw.Runtime.FunctionNotCompiledException" /> class.</summary>
        public FunctionNotCompiledException() : base() { }

        /// <summary>Initializes a new instance of the <see cref="T:LukeSw.Runtime.FunctionNotCompiledException" /> class with a specified error message.</summary>
        /// <param name="message">A message that describes the error.</param>
        public FunctionNotCompiledException(string message) : base(message) { }

        /// <summary>Initializes a new instance of the <see cref="T:LukeSw.Runtime.FunctionNotCompiledException" /> class with a specified error message and a reference to the inner exception that is the cause of this exception.</summary>
        /// <param name="message">The error message that explains the reason for the exception.</param>
        /// <param name="innerException">The exception that is the cause of the current exception. If the innerException parameter is not a null reference, the current exception is raised in a catch block that handles the inner exception.</param>
        public FunctionNotCompiledException(string message, Exception innerException) : base(message, innerException) { }

        /// <summary>Initializes a new instance of the <see cref="T:LukeSw.Runtime.FunctionNotCompiledException" /> class with serialized data.</summary>
        /// <param name="context">The contextual information about the source or destination.</param>
        /// <param name="info">The object that holds the serialized object data.</param>
        protected FunctionNotCompiledException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>Represents a function that was compiled at runtime.</summary>
    /// <param name="x">Function input parameter.</param>
    /// <returns>Result value of function.</returns>
    public delegate double CompiledFunction(params double[] x);
}
