#region Byline & Disclaimer
//
//  Author(s):
//
//      Atif Aziz (http://www.raboof.com)
//
//      Portion Copyright (c) 2001 Douglas Crockford
//      http://www.crockford.com
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
// 
#endregion

namespace Sprocket.Web
{
    #region Imports

    using System;
    using System.IO;
    using System.Runtime.Serialization;

    #endregion

    #region About JavaScriptCondenser (JSMin)
    
    /*

    JavaScriptCondenser is a C# port of jsmin.c that was originally written
    by Douglas Crockford.
    
        jsmin.c
        04-Dec-2003
        (c) 2001 Douglas Crockford
        http://www.crockford.com

        C# port written by Atif Aziz
        12-Apr-2005
        http://www.raboof.com
    
    The following documentation is a minimal adaption from the original
    found at:
    
        http://www.crockford.com/javascript/jsmin.html

    The documentation therefore still refers to JSMin, but equally 
    applies to this C# port since the code implementation has not been
    changed or enhanced in any way. Some passages have been omitted
    since they don't apply. For example, the original documentation has
    a comment about character set. This does not apply to this port
    since JavaScriptCondenser works with TextReader and TextWriter
    from the Base Class Library (BCL). The character set responsibility
    is therefore pushed back to the user of this class.



    What JSMin Does
    ---------------
    
    JSMin is a filter that omits or modifies some characters. This does 
    not change the behavior of the program that it is minifying. The 
    result may be harder to debug. It will definitely be harder to read.

    JSMin first replaces carriage returns ('\r') with linefeeds ('\n'). 
    It replaces all other control characters (including tab) with spaces. 
    It replaces comments in the // form with linefeeds. It replaces 
    comments with spaces. All runs of spaces are replaced with a single 
    space. All runs of linefeeds are replaced with a single linefeed.

    It omits spaces except when a space is preceded or followed by a 
    non-ASCII character or by an ASCII letter or digit, or by one of 
    these characters:

    \ $ _

    It is more conservative in omitting linefeeds, because linefeeds are 
    sometimes treated as semicolons. A linefeed is not omitted if it 
    precedes a non-ASCII character or an ASCII letter or digit or one of 
    these characters:

    \ $ _ { [ ( + -

    and if it follows a non-ASCII character or an ASCII letter or digit 
    or one of these characters:

    \ $ _ } ] ) + - " '

    No other characters are omitted or modified.

    JSMin knows to not modify quoted strings and regular expression 
    literals.

    JSMin does not obfuscate, but it does uglify.

    Before:

        // is.js

        // (c) 2001 Douglas Crockford
        // 2001 June 3


        // is

        // The -is- object is used to identify the browser.  Every browser edition
        // identifies itself, but there is no standard way of doing it, and some of
        // the identification is deceptive. This is because the authors of web
        // browsers are liars. For example, Microsoft's IE browsers claim to be
        // Mozilla 4. Netscape 6 claims to be version 5.

        var is = {
            ie:      navigator.appName == 'Microsoft Internet Explorer',
            java:    navigator.javaEnabled(),
            ns:      navigator.appName == 'Netscape',
            ua:      navigator.userAgent.toLowerCase(),
            version: parseFloat(navigator.appVersion.substr(21)) ||
                    parseFloat(navigator.appVersion),
            win:     navigator.platform == 'Win32'
        }
        is.mac = is.ua.indexOf('mac') >= 0;
        if (is.ua.indexOf('opera') >= 0) {
            is.ie = is.ns = false;
            is.opera = true;
        }
        if (is.ua.indexOf('gecko') >= 0) {
            is.ie = is.ns = false;
            is.gecko = true;
        }

    After:

        var is={ie:navigator.appName=='MicrosoftInternetExplorer',java:navigator.javaEnabled(),ns:navigator.appName=='Netscape',ua:navigator.userAgent.toLowerCase(),version:parseFloat(navigator.appVersion.substr(21))||parseFloat(navigator.appVersion),win:navigator.platform=='Win32'}
        is.mac=is.ua.indexOf('mac')>=0;if(is.ua.indexOf('opera')>=0){is.ie=is.ns=false;is.opera=true;}
        if(is.ua.indexOf('gecko')>=0){is.ie=is.ns=false;is.gecko=true;}

    
    
    Caution
    -------
    
    Do not put raw control characters inside a quoted string. That is an 
    extremely bad practice. Use \xhh notation instead. JSMin will replace 
    control characters with spaces or linefeeds.

    Use parens with confusing sequences of + or -. For example, minification 
    changes

        a + ++b
    
    into

        a+++b 
        
    which is interpreted as

        a++ + b
    
    which is wrong. You can avoid this by using parens:

        a + (++b)
    
    JSLint (http://www.jslint.com/) checks for all of these problems. It is 
    suggested that JSLint be used before using JSMin.
    
    
    
    Errors
    ------
    
    JSMin can detect and produce three error messages:

        - Unterminated comment. 
        - Unterminated string constant.
        - Unterminated regular expression.

    It ignores all other errors that may be present in your source program.

    */

    #endregion

    public sealed class JavaScriptCondenser
    {
        private JavaScriptCondenser() {}

        public static string Condense(string source)
        {
            StringWriter writer = new StringWriter();
            Condense(source, writer);
            return writer.ToString();
        }

        public static void Condense(string source, TextWriter writer)
        {
            Condense(new StringReader(source), writer);
        }

        public static void Condense(TextReader reader, TextWriter writer)
        {
            Condenser theCondenser = new Condenser(reader, writer);
            theCondenser.Condense();
        }

        private sealed class Condenser
        {
            private int theA;
            private int theB;
            private int theLookahead = eof;
            private TextReader reader = Console.In;
            private TextWriter writer = Console.Out;

            private const int eof = -1;

            public Condenser(TextReader reader, TextWriter writer)
            {
                this.reader = reader;
                this.writer = writer;
            }

            /* Condense -- Copy the input to the output, deleting the characters which are
                    insignificant to JavaScript. Comments will be removed. Tabs will be
                    replaced with spaces. Carriage returns will be replaced with linefeeds.
                    Most spaces and linefeeds will be removed. 
            */
        
            public void Condense()
            {
                theA = '\n';
                Action(3);
                while (theA != eof)
                {
                    switch (theA)
                    {
                        case ' ':
                            if (IsAlphanum(theB))
                            {
                                Action(1);
                            }
                            else
                            {
                                Action(2);
                            }
                            break;
                        case '\n':
                        switch (theB)
                        {
                            case '{':
                            case '[':
                            case '(':
                            case '+':
                            case '-':
                                Action(1);
                                break;
                            case ' ':
                                Action(3);
                                break;
                            default:
                                if (IsAlphanum(theB))
                                {
                                    Action(1);
                                }
                                else
                                {
                                    Action(2);
                                }
                                break;
                        }
                            break;
                        default:
                        switch (theB)
                        {
                            case ' ':
                                if (IsAlphanum(theA))
                                {
                                    Action(1);
                                    break;
                                }
                                Action(3);
                                break;
                            case '\n':
                            switch (theA)
                            {
                                case '}':
                                case ']':
                                case ')':
                                case '+':
                                case '-':
                                case '"':
                                case '\'':
                                    Action(1);
                                    break;
                                default:
                                    if (IsAlphanum(theA))
                                    {
                                        Action(1);
                                    }
                                    else
                                    {
                                        Action(3);
                                    }
                                    break;
                            }
                                break;
                            default:
                                Action(1);
                                break;
                        }
                            break;
                    }
                }
            }

            /* Get -- return the next character from stdin. Watch out for lookahead. If 
                    the character is a control character, translate it to a space or 
                    linefeed.
            */

            private int Get()
            {
                int c = theLookahead;
                theLookahead = eof;
                if (c == eof)
                {
                    c = reader.Read();
                }
                if (c >= ' ' || c == '\n' || c == eof)
                {
                    return c;
                }
                if (c == '\r')
                {
                    return '\n';
                }
                return ' ';
            }


            /* Peek -- get the next character without getting it.  
            */

            private int Peek()
            {
                theLookahead = Get();
                return theLookahead;
            }

            /* Next -- get the next character, excluding comments. Peek() is used to see 
                    if a '/' is followed by a '/' or '*'.
            */

            private int Next()
            {
                int c = Get();
                if (c == '/')
                {
                    switch (Peek())
                    {
                        case '/':
                            for (;; )
                            {
                                c = Get();
                                if (c <= '\n')
                                {
                                    return c;
                                }
                            }
                        case '*':
                            Get();
                            for (;; )
                            {
                                switch (Get())
                                {
                                    case '*':
                                        if (Peek() == '/')
                                        {
                                            Get();
                                            return ' ';
                                        }
                                        break;
                                    case eof:
                                        throw new Exception("Unterminated comment.");
                                }
                            }
                        default:
                            return c;
                    }
                }
                return c;
            }


            /* Action -- do something! What you do is determined by the argument:
                    1   Output A. Copy A to B. Get the next B.
                    2   Copy B to A. Get the next B. (Delete A).
                    3   Get the next B. (Delete B).
               Action treats a string as a single character. Wow!
               Action recognizes a regular expression if it is preceded by ( or , or =.
            */

            private void Action(int d)
            {
                switch (d)
                {
                    case 1:
                        Write(theA);
                        goto case 2;
                    case 2:
                        theA = theB;
                        if (theA == '\'' || theA == '"')
                        {
                            for (;; )
                            {
                                Write(theA);
                                theA = Get();
                                if (theA == theB)
                                {
                                    break;
                                }
                                if (theA <= '\n')
                                {
                                    string message = string.Format("Unterminated string literal: '{0}'.", theA);
                                    throw new Exception(message);
                                }
                                if (theA == '\\')
                                {
                                    Write(theA);
                                    theA = Get();
                                }
                            }
                        }
                        goto case 3;
                    case 3:
                        theB = Next();
                        if (theB == '/' && (theA == '(' || theA == ',' || theA == '='))
                        {
                            Write(theA);
                            Write(theB);
                            for (;; )
                            {
                                theA = Get();
                                if (theA == '/')
                                {
                                    break;
                                }
                                else if (theA == '\\')
                                {
                                    Write(theA);
                                    theA = Get();
                                }
                                else if (theA <= '\n')
                                {
                                    throw new Exception("Unterminated Regular Expression literal.");
                                }
                                Write(theA);
                            }
                            theB = Next();
                        }
                        break;
                }
            }

 
            private void Write(int ch)
            {
                writer.Write((char) ch);
            }

            /* IsAlphanum -- return true if the character is a letter, digit, underscore,
                    dollar sign, or non-ASCII character.
            */

            private static bool IsAlphanum(int c)
            {
                return ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') ||
                    (c >= 'A' && c <= 'Z') || c == '_' || c == '$' || c == '\\' ||
                    c > 126);
            }
        }

        [ Serializable ]
        public class Exception : System.ApplicationException
        {
            public Exception() {}

            public Exception(string message) : 
                base(message) {}

            public Exception(string message, Exception innerException) :
                base(message, innerException) {}

            protected Exception(SerializationInfo info, StreamingContext context) :
                base(info, context) {}
        }
    }
}